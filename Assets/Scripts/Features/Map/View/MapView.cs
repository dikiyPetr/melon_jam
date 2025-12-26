using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(MapAnimationController))]
public class MapView : MonoBehaviour
{
    [SerializeField] private MapConfig _mapConfig;
    [SerializeField] private Transform _nodesContainer;
    [SerializeField] private Transform _connectionsContainer;
    [SerializeField] private Transform _playerIcon;
    [SerializeField] private MapNodeView _nodeViewPrefab;
    [SerializeField] private MapConnectionView _connectionViewPrefab;
    [SerializeField] private Vector2 _nodeSpacing = new Vector2(2f, 1.5f);
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private float _cameraScrollSpeed = 5f;
    [SerializeField] private float _cameraBorderPadding = 1f;

    private MapData _mapData;
    private MapGenerator _mapGenerator;
    private MapAnimationController _animationController;
    private Dictionary<string, MapNodeView> _nodeViews;
    private List<MapConnectionView> _connectionViews;
    private MapNodeView _hoveredNode;

    private float _minCameraY;
    private float _maxCameraY;
    private bool _mapBoundsCalculated;

    [Inject] private MapNavigationController _navigationController;

    private void Awake()
    {
        DI.Inject(this);

        _nodeViews = new Dictionary<string, MapNodeView>();
        _connectionViews = new List<MapConnectionView>();

        _mapGenerator = GetComponent<MapGenerator>();
        _animationController = GetComponent<MapAnimationController>();

        if (_mapCamera == null)
        {
            _mapCamera = Camera.main;
        }
    }

    private void Start()
    {
        GenerateAndDisplayMap();
    }

    private void Update()
    {
        HandleMouseInput();
        HandleCameraScroll();
    }

    private void HandleMouseInput()
    {
        if (_mapCamera == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = _mapCamera.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, LayerMask.GetMask("MapNode"));

        if (hit.collider != null)
        {
            var nodeView = hit.collider.GetComponent<MapNodeView>();
            if (nodeView != null)
            {
                if (_hoveredNode != nodeView)
                {
                    if (_hoveredNode != null)
                    {
                        _hoveredNode.HandleMouseExit();
                    }

                    _hoveredNode = nodeView;
                    _hoveredNode.HandleMouseEnter();
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    PrintNodeInfo(nodeView.NodeData);
                }

                // todo пока дебаг 
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    nodeView.HandleClick();
                }
            }
        }
        else
        {
            if (_hoveredNode != null)
            {
                _hoveredNode.HandleMouseExit();
                _hoveredNode = null;
            }
        }
    }

    public void GenerateAndDisplayMap()
    {
        ClearMap();

        _mapData = _mapGenerator.GenerateMap(_mapConfig);

        CreateConnections();
        CreateNodes();

        if (_navigationController != null)
        {
            _navigationController.Initialize(_mapData);
            _navigationController.OnNodeSelected += HandleNodeSelected;
        }

        if (_playerIcon != null)
        {
            var startNode = _mapData.Nodes.Find(n => n.NodeType == MapNodeType.Start);
            if (startNode != null)
            {
                _playerIcon.position = CalculateNodePosition(startNode.GridPosition);
            }
        }

        CalculateMapBounds();
        UpdateAllVisuals();
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _mapData.Nodes)
        {
            Vector3 position = CalculateNodePosition(nodeData.GridPosition);

            var nodeView = Instantiate(_nodeViewPrefab, _nodesContainer);
            nodeView.transform.position = position;
            nodeView.Initialize(nodeData, _mapConfig.NodeVisualData);

            nodeView.OnNodeClicked += HandleNodeClicked;

            _nodeViews[nodeData.Id] = nodeView;
        }
    }

    private void CreateConnections()
    {
        foreach (var nodeData in _mapData.Nodes)
        {
            Vector3 fromPosition = CalculateNodePosition(nodeData.GridPosition);

            foreach (var connectedNodeId in nodeData.ConnectedNodeIds)
            {
                var connectedNode = _mapData.GetNodeById(connectedNodeId);
                if (connectedNode != null)
                {
                    Vector3 toPosition = CalculateNodePosition(connectedNode.GridPosition);

                    var connectionView = Instantiate(_connectionViewPrefab, _connectionsContainer);
                    connectionView.Initialize(nodeData, connectedNode, fromPosition, toPosition);

                    _connectionViews.Add(connectionView);
                }
            }
        }
    }

    private Vector3 CalculateNodePosition(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * _nodeSpacing.x,
            gridPosition.y * _nodeSpacing.y,
            0f
        );
    }

    private void HandleNodeClicked(MapNodeData nodeData)
    {
        if (_navigationController != null)
        {
            _navigationController.SelectNode(nodeData.Id);
        }
    }

    private void HandleNodeSelected(MapNodeData nodeData)
    {
        if (_playerIcon != null && _animationController != null)
        {
            var currentNode = _mapData.GetCurrentNode();
            if (currentNode != null)
            {
                AnimatePathConnection(currentNode.Id, nodeData.Id);
            }

            Vector3 targetPosition = CalculateNodePosition(nodeData.GridPosition);
            _animationController.AnimatePlayerMovement(
                _playerIcon,
                _playerIcon.position,
                targetPosition,
                () => OnPlayerMovementComplete(nodeData)
            );
        }
        else
        {
            OnPlayerMovementComplete(nodeData);
        }
    }

    private void AnimatePathConnection(string fromNodeId, string toNodeId)
    {
        foreach (var connectionView in _connectionViews)
        {
            if (connectionView.IsConnectionBetween(fromNodeId, toNodeId))
            {
                _animationController.AnimateConnectionVisited(
                    connectionView.GetLineRenderer(),
                    connectionView.VisitedColor,
                    () => connectionView.MarkAsVisited()
                );
                break;
            }
        }
    }

    private void OnPlayerMovementComplete(MapNodeData nodeData)
    {
        if (_navigationController != null)
        {
            _navigationController.MoveToNode(nodeData.Id);
        }

        UpdateAllVisuals();
    }

    private void UpdateAllVisuals()
    {
        foreach (var nodeView in _nodeViews.Values)
        {
            nodeView.UpdateVisuals();
        }

        string currentNodeId = _mapData?.CurrentNodeId;
        foreach (var connectionView in _connectionViews)
        {
            connectionView.UpdateVisuals(currentNodeId);
        }
    }

    private void HandleCameraScroll()
    {
        if (_mapCamera == null || !_mapBoundsCalculated) return;

        var look = Mouse.current?.scroll;
        if (look == null) return;

        Vector2 scrollDelta = look.ReadValue();

        if (Mathf.Abs(scrollDelta.y) > 0.01f)
        {
            Vector3 currentPos = _mapCamera.transform.position;
            float scrollDirection = Mathf.Sign(scrollDelta.y);
            float newY = currentPos.y + scrollDirection * _cameraScrollSpeed * Time.deltaTime;

            newY = Mathf.Clamp(newY, _minCameraY, _maxCameraY);

            _mapCamera.transform.position = new Vector3(currentPos.x, newY, currentPos.z);
        }
    }

    private void CalculateMapBounds()
    {
        if (_mapData == null || _mapData.Nodes.Count == 0 || _mapCamera == null)
        {
            _mapBoundsCalculated = false;
            return;
        }

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var nodeData in _mapData.Nodes)
        {
            Vector3 nodePos = CalculateNodePosition(nodeData.GridPosition);
            minY = Mathf.Min(minY, nodePos.y);
            maxY = Mathf.Max(maxY, nodePos.y);
        }

        float cameraHeight = _mapCamera.orthographicSize;

        _minCameraY = minY - _cameraBorderPadding + cameraHeight;
        _maxCameraY = maxY + _cameraBorderPadding - cameraHeight;

        _minCameraY = Mathf.Min(_minCameraY, _maxCameraY);

        _mapBoundsCalculated = true;
    }

    private void PrintNodeInfo(MapNodeData nodeData)
    {
        if (nodeData == null) return;

        Debug.Log($"=== Node Info ===\n" +
                  $"ID: {nodeData.Id}\n" +
                  $"Type: {nodeData.NodeType}\n" +
                  $"Position: ({nodeData.GridPosition.x}, {nodeData.GridPosition.y})\n" +
                  $"Is Available: {nodeData.IsAvailable}\n" +
                  $"Is Visited: {nodeData.IsVisited}\n" +
                  $"Connected Nodes: {string.Join(", ", nodeData.ConnectedNodeIds)}\n" +
                  $"================");
    }

    private void ClearMap()
    {
        foreach (var nodeView in _nodeViews.Values)
        {
            if (nodeView != null)
            {
                nodeView.OnNodeClicked -= HandleNodeClicked;
                Destroy(nodeView.gameObject);
            }
        }

        _nodeViews.Clear();

        foreach (var connectionView in _connectionViews)
        {
            if (connectionView != null)
            {
                Destroy(connectionView.gameObject);
            }
        }

        _connectionViews.Clear();

        _mapBoundsCalculated = false;
    }

    private void OnDestroy()
    {
        if (_navigationController != null)
        {
            _navigationController.OnNodeSelected -= HandleNodeSelected;
        }

        ClearMap();
    }
}