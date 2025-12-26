using System;
using UnityEngine;

public class MapNavigationController : MonoBehaviour
{
    private MapData _mapData;
    private MapPathValidator _pathValidator;

    public event Action<MapNodeData> OnNodeSelected;
    public event Action<MapNodeData> OnNodeReached;
    public event Action OnMapCompleted;

    public void Initialize(MapData mapData)
    {
        _mapData = mapData;
        _pathValidator = new MapPathValidator(_mapData);
    }

    public bool SelectNode(string nodeId)
    {
        if (!_pathValidator.CanMoveToNode(nodeId))
        {
            return false;
        }

        var selectedNode = _mapData.GetNodeById(nodeId);
        if (selectedNode == null) return false;

        OnNodeSelected?.Invoke(selectedNode);

        return true;
    }

    public void MoveToNode(string nodeId)
    {
        var node = _mapData.GetNodeById(nodeId);
        if (node == null) return;

        var previousNode = _mapData.GetCurrentNode();
        if (previousNode != null)
        {
            previousNode.IsAvailable = false;
        }

        _mapData.CurrentNodeId = nodeId;
        node.IsVisited = true;
        node.IsAvailable = false;
        _mapData.VisitedNodeIds.Add(nodeId);

        UpdateAvailableNodes(node);

        OnNodeReached?.Invoke(node);

        if (node.NodeType == MapNodeType.Boss)
        {
            OnMapCompleted?.Invoke();
        }
    }

    private void UpdateAvailableNodes(MapNodeData currentNode)
    {
        foreach (var nodeData in _mapData.Nodes)
        {
            if (!nodeData.IsVisited)
            {
                nodeData.IsAvailable = false;
            }
        }

        foreach (var connectedNodeId in currentNode.ConnectedNodeIds)
        {
            var connectedNode = _mapData.GetNodeById(connectedNodeId);
            if (connectedNode != null && !connectedNode.IsVisited)
            {
                connectedNode.IsAvailable = true;
            }
        }
    }

    public MapNodeData GetCurrentNode()
    {
        return _mapData.GetCurrentNode();
    }

    public bool IsNodeAvailable(string nodeId)
    {
        return _pathValidator.IsNodeAccessible(nodeId);
    }
}
