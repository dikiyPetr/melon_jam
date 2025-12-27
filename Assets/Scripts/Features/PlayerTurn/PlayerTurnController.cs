using System;
using UnityEngine;
using VContainer;

public class PlayerTurnController : MonoBehaviour
{
    [Inject] private MapNavigationController _navigationController;
    [Inject] private AIMotivationPathController _aiPathController;

    private MapData _mapData;

    public event Action OnTurnStarted;
    public event Action OnTurnCompleted;
    public event Action OnTurnStateChanged;
    public event Action<MapNodeData, MapNodeData> OnPlayerMovementRequested;

    private void Awake()
    {
        DI.Inject(this);
    }

    public void Initialize(MapData mapData)
    {
        _mapData = mapData;

        if (_navigationController != null)
        {
            _navigationController.OnNodeSelected += HandleNodeSelected;
            _navigationController.OnNodeReached += HandleNodeReached;
        }
        OnTurnStateChanged?.Invoke();
    }

    public bool CanMakeNextTurn()
    {
        if (_aiPathController == null || _navigationController == null) return false;

        var path = _aiPathController.CurrentPath;
        if (path == null || path.Count < 2) return false;

        var currentNodeId = _mapData?.CurrentNodeId;
        if (string.IsNullOrEmpty(currentNodeId)) return false;

        if (!AllValidNodesHaveSelectedType())
        {
            return false;
        }

        return true;
    }

    private bool AllValidNodesHaveSelectedType()
    {
        if (_aiPathController == null || _mapData == null) return false;

        var validNextNodes = _aiPathController.ValidNextNodes;
        if (validNextNodes == null || validNextNodes.Count == 0) return true;

        foreach (var nodeId in validNextNodes)
        {
            var node = _mapData.GetNodeById(nodeId);
            if (node != null && !node.HasSelectedType)
            {
                return false;
            }
        }

        return true;
    }

    public void MakeNextTurn()
    {
        if (!CanMakeNextTurn()) return;

        OnTurnStarted?.Invoke();

        var path = _aiPathController.CurrentPath;
        var nextNodeId = path[1];
        var currentNodeId = _mapData.CurrentNodeId;

        var fromNode = _mapData.GetNodeById(currentNodeId);
        var toNode = _mapData.GetNodeById(nextNodeId);

        if (fromNode != null && toNode != null)
        {
            OnPlayerMovementRequested?.Invoke(fromNode, toNode);
        }
    }

    public void NotifyTurnStateChanged()
    {
        OnTurnStateChanged?.Invoke();
    }

    private void HandleNodeSelected(MapNodeData nodeData)
    {
        OnTurnStateChanged?.Invoke();
    }

    private void HandleNodeReached(MapNodeData nodeData)
    {
        OnTurnCompleted?.Invoke();
        OnTurnStateChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (_navigationController != null)
        {
            _navigationController.OnNodeSelected -= HandleNodeSelected;
            _navigationController.OnNodeReached -= HandleNodeReached;
        }
    }
}
