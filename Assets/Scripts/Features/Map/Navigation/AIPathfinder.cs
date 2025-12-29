using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPathfinder
{
    private MapData _mapData;

    public AIPathfinder(MapData mapData)
    {
        _mapData = mapData;
    }

    public List<string> FindPathToDesiredNode(string currentNodeId, MapNodeType desiredNodeType)
    {
        var currentNode = _mapData.GetNodeById(currentNodeId);
        if (currentNode == null) return new List<string>();

        var targetCheckpointNode = FindDesiredNodeInNextCheckpoint(currentNode, desiredNodeType);
        if (targetCheckpointNode == null) return new List<string>();

        return BuildPath(currentNode, targetCheckpointNode);
    }

    public HashSet<string> FindValidNextNodes(string currentNodeId, MapNodeType desiredNodeType)
    {
        var validNodes = new HashSet<string>(StringEqualityComparer.Default);
        var currentNode = _mapData.GetNodeById(currentNodeId);
        if (currentNode == null) return validNodes;

        var targetCheckpointNode = FindDesiredNodeInNextCheckpoint(currentNode, desiredNodeType);
        if (targetCheckpointNode == null) return validNodes;

        foreach (var connectedNodeId in currentNode.ConnectedNodeIds)
        {
            var connectedNode = _mapData.GetNodeById(connectedNodeId);
            if (connectedNode == null) continue;

            var pathFromConnected = BuildPath(connectedNode, targetCheckpointNode);
            if (pathFromConnected.Count > 0)
            {
                validNodes.Add(connectedNodeId);
            }
        }

        return validNodes;
    }

    public HashSet<string> FindAllNodesLeadingToGoal(string currentNodeId, MapNodeType desiredNodeType)
    {
        var nodesLeadingToGoal = new HashSet<string>(StringEqualityComparer.Default);
        var currentNode = _mapData.GetNodeById(currentNodeId);
        if (currentNode == null) return nodesLeadingToGoal;

        var targetCheckpointNode = FindDesiredNodeInNextCheckpoint(currentNode, desiredNodeType);
        if (targetCheckpointNode == null) return nodesLeadingToGoal;

        var visited = new HashSet<string>(StringEqualityComparer.Default);
        var queue = new Queue<string>();
        queue.Enqueue(currentNodeId);

        while (queue.Count > 0)
        {
            var nodeId = queue.Dequeue();
            if (visited.Contains(nodeId)) continue;
            visited.Add(nodeId);

            var node = _mapData.GetNodeById(nodeId);
            if (node == null) continue;

            var pathToGoal = BuildPath(node, targetCheckpointNode);
            if (pathToGoal.Count > 0)
            {
                nodesLeadingToGoal.Add(nodeId);

                foreach (var connectedNodeId in node.ConnectedNodeIds)
                {
                    if (!visited.Contains(connectedNodeId))
                    {
                        queue.Enqueue(connectedNodeId);
                    }
                }
            }
        }

        return nodesLeadingToGoal;
    }

    private MapNodeData FindDesiredNodeInNextCheckpoint(MapNodeData currentNode, MapNodeType desiredNodeType)
    {
        int currentRow = currentNode.GridPosition.y;

        for (int row = currentRow + 1; row < _mapData.Rows.Count; row++)
        {
            var rowData = _mapData.GetRow(row);
            if (rowData == null) continue;

            if (rowData.RowType == MapRowType.Checkpoint || rowData.RowType == MapRowType.Boss)
            {
                var nodesInRow = rowData.GetAllNodes();
                var desiredNode = nodesInRow.FirstOrDefault(n => n.NodeType == desiredNodeType);

                if (desiredNode != null)
                {
                    return desiredNode;
                }

                return nodesInRow.FirstOrDefault();
            }
        }

        return null;
    }

    private List<string> BuildPath(MapNodeData startNode, MapNodeData targetNode)
    {
        var path = new List<string>();
        var visited = new HashSet<string>(StringEqualityComparer.Default);
        var queue = new Queue<List<string>>();

        queue.Enqueue(new List<string> { startNode.Id });

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            var currentNodeId = currentPath[currentPath.Count - 1];

            if (currentNodeId == targetNode.Id)
            {
                return currentPath;
            }

            if (visited.Contains(currentNodeId)) continue;
            visited.Add(currentNodeId);

            var currentNode = _mapData.GetNodeById(currentNodeId);
            if (currentNode == null) continue;

            foreach (var connectedNodeId in currentNode.ConnectedNodeIds)
            {
                if (!visited.Contains(connectedNodeId))
                {
                    var newPath = new List<string>(currentPath) { connectedNodeId };
                    queue.Enqueue(newPath);
                }
            }
        }

        return new List<string>();
    }
}