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
    [SerializeField] private Vector2 _positionRandomOffset = new Vector2(0.3f, 0.3f);
    [SerializeField] private MapNodeTypeMenuController _menuController;
    [Inject] private NodeEventManager _nodeEventManager;
    [Inject] private DialogManager _dialogManager;
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private float _cameraScrollSpeed = 5f;
    [SerializeField] private float _cameraBorderPadding = 1f;

    private MapData _mapData;
    private MapGenerator _mapGenerator;
    private MapAnimationController _animationController;
    private Dictionary<string, MapNodeView> _nodeViews;
    private Dictionary<string, Vector3> _nodePositions;
    private List<MapConnectionView> _connectionViews;
    private MapNodeView _hoveredNode;

    private float _minCameraY;
    private float _maxCameraY;
    private bool _mapBoundsCalculated;

    [Inject] private MapNavigationController _navigationController;
    [Inject] private AIMotivationPathController _aiPathController;
    [Inject] private PlayerTurnController _playerTurnController;

    private void Awake()
    {
        DI.Inject(this);

        _nodeViews = new Dictionary<string, MapNodeView>();
        _nodePositions = new Dictionary<string, Vector3>();
        _connectionViews = new List<MapConnectionView>();

        _mapGenerator = GetComponent<MapGenerator>();
        _animationController = GetComponent<MapAnimationController>();

        if (_mapCamera == null)
        {
            _mapCamera = Camera.main;
        }

        if (_menuController != null)
        {
            _menuController.Initialize(_aiPathController, _mapConfig.NodeVisualData);
            _menuController.OnNodeTypeSelected += HandleNodeTypeSelected;
        }

        if (_nodeEventManager != null)
        {
            _nodeEventManager.Initialize();
        }

        if (_dialogManager != null)
        {
            _dialogManager.OnChoiceMade += HandleEventChoiceMade;
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
            _navigationController.Initialize(_mapData, _mapConfig);
        }

        if (_aiPathController != null)
        {
            _aiPathController.Initialize(_mapData);
        }

        if (_playerIcon != null)
        {
            var startNode = _mapData.Nodes.Find(n => n.NodeType == MapNodeType.Start);
            if (startNode != null)
            {
                _playerIcon.position = CalculateNodePosition(startNode);

                if (_aiPathController != null)
                {
                    _aiPathController.UpdatePath(startNode.Id);
                }
            }
        }

        CalculateMapBounds();
        UpdateAllVisuals();
        _playerTurnController.Initialize(_mapData);
        _playerTurnController.OnPlayerMovementRequested += HandlePlayerMovementRequested;
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _mapData.Nodes)
        {
            Vector3 position = CalculateNodePosition(nodeData);

            var nodeView = Instantiate(_nodeViewPrefab, _nodesContainer);
            nodeView.transform.position = position;
            nodeView.Initialize(nodeData, _mapConfig.NodeVisualData);

            nodeView.OnNodeClicked += HandleNodeClicked;

            if (_menuController != null)
            {
                _menuController.SubscribeToNodeEvents(nodeView);
            }

            _nodeViews[nodeData.Id] = nodeView;
        }
    }

    private void CreateConnections()
    {
        foreach (var nodeData in _mapData.Nodes)
        {
            Vector3 fromPosition = CalculateNodePosition(nodeData);

            foreach (var connectedNodeId in nodeData.ConnectedNodeIds)
            {
                var connectedNode = _mapData.GetNodeById(connectedNodeId);
                if (connectedNode != null)
                {
                    Vector3 toPosition = CalculateNodePosition(connectedNode);

                    var connectionView = Instantiate(_connectionViewPrefab, _connectionsContainer);
                    connectionView.Initialize(nodeData, connectedNode, fromPosition, toPosition);

                    _connectionViews.Add(connectionView);
                }
            }
        }
    }

    private Vector3 CalculateNodePosition(MapNodeData nodeData)
    {
        if (_nodePositions.ContainsKey(nodeData.Id))
        {
            return _nodePositions[nodeData.Id];
        }

        Vector3 basePosition = new Vector3(
            nodeData.GridPosition.x * _nodeSpacing.x,
            nodeData.GridPosition.y * _nodeSpacing.y,
            0f
        );

        Vector3 randomOffset = new Vector3(
            Random.Range(-_positionRandomOffset.x, _positionRandomOffset.x),
            Random.Range(-_positionRandomOffset.y, _positionRandomOffset.y),
            0f
        );

        Vector3 finalPosition = basePosition + randomOffset;
        _nodePositions[nodeData.Id] = finalPosition;

        return finalPosition;
    }

    private void HandleNodeClicked(MapNodeData nodeData)
    {
        if (_aiPathController != null)
        {
            var validNextNodes = _aiPathController.ValidNextNodes;
            if (validNextNodes != null && validNextNodes.Contains(nodeData.Id) && !nodeData.HasSelectedType)
            {
                return;
            }
        }

        if (_navigationController != null)
        {
            _navigationController.SelectNode(nodeData.Id);
        }
    }

    private void HandleNodeTypeSelected()
    {
        UpdateAllVisuals();

        if (_playerTurnController != null)
        {
            _playerTurnController.NotifyTurnStateChanged();
        }
    }

    private void HandlePlayerMovementRequested(MapNodeData fromNode, MapNodeData toNode)
    {
        if (_playerIcon == null || _animationController == null) return;

        Vector3 fromPosition = CalculateNodePosition(fromNode);
        Vector3 toPosition = CalculateNodePosition(toNode);

        AnimatePathConnection(fromNode.Id, toNode.Id);

        _animationController.AnimatePlayerMovement(
            _playerIcon,
            fromPosition,
            toPosition,
            () => OnPlayerMovementComplete(toNode)
        );
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

        if (_aiPathController != null)
        {
            _aiPathController.UpdatePath(nodeData.Id);
        }

        UpdateAllVisuals();

        if (_playerTurnController != null)
        {
            _playerTurnController.NotifyTurnStateChanged();
        }

        ShowNodeEvent(nodeData);
    }

    private void ShowNodeEvent(MapNodeData nodeData)
    {
        if (_nodeEventManager == null || _dialogManager == null || nodeData == null) return;

        var nodeEvent = _nodeEventManager.GetEventForNode(nodeData.NodeType);
        if (nodeEvent != null)
        {
            _dialogManager.ShowNodeEvent(nodeEvent);
        }
    }

    private void HandleEventChoiceMade(NodeEventChoice choice)
    {
        if (_playerTurnController != null)
        {
            _playerTurnController.NotifyTurnStateChanged();
        }
    }

    private void UpdateAllVisuals()
    {
        foreach (var nodeView in _nodeViews.Values)
        {
            nodeView.UpdateVisuals();
        }

        string currentNodeId = _mapData?.CurrentNodeId;
        var nodesLeadingToGoal = _aiPathController?.NodesLeadingToGoal;
        var validNextNodes = _aiPathController?.ValidNextNodes;

        foreach (var connectionView in _connectionViews)
        {
            bool leadsToGoal = IsConnectionLeadsToGoal(connectionView, nodesLeadingToGoal);
            connectionView.UpdateVisuals(currentNodeId, leadsToGoal, validNextNodes);
        }
    }

    private bool IsConnectionLeadsToGoal(MapConnectionView connection, HashSet<string> nodesLeadingToGoal)
    {
        if (nodesLeadingToGoal == null || nodesLeadingToGoal.Count == 0) return false;

        return connection.IsConnectionLeadingToGoal(nodesLeadingToGoal);
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
            Vector3 nodePos = CalculateNodePosition(nodeData);
            minY = Mathf.Min(minY, nodePos.y);
            maxY = Mathf.Max(maxY, nodePos.y);
        }

        float cameraHeight = _mapCamera.orthographicSize;

        _minCameraY = minY - _cameraBorderPadding + cameraHeight;
        _maxCameraY = maxY + _cameraBorderPadding - cameraHeight;

        _minCameraY = Mathf.Min(_minCameraY, _maxCameraY);
       
        Vector3 currentPos = _mapCamera.transform.position;
        _mapCamera.transform.position = new Vector3(currentPos.x, _minCameraY, currentPos.z);
        _mapBoundsCalculated = true;
    }

    private void ClearMap()
    {
        _nodePositions.Clear();

        foreach (var nodeView in _nodeViews.Values)
        {
            if (nodeView != null)
            {
                nodeView.OnNodeClicked -= HandleNodeClicked;

                if (_menuController != null)
                {
                    _menuController.UnsubscribeFromNodeEvents(nodeView);
                }

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
        if (_menuController != null)
        {
            _menuController.OnNodeTypeSelected -= HandleNodeTypeSelected;
        }

        if (_playerTurnController != null)
        {
            _playerTurnController.OnPlayerMovementRequested -= HandlePlayerMovementRequested;
        }

        if (_dialogManager != null)
        {
            _dialogManager.OnChoiceMade -= HandleEventChoiceMade;
        }

        ClearMap();
    }
}