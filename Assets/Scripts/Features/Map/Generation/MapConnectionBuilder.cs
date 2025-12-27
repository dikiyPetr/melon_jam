using System.Collections.Generic;
using UnityEngine;

public class MapConnectionBuilder
{
    private MapConfig _config;

    public MapConnectionBuilder(MapConfig config)
    {
        _config = config;
    }

    public void ConnectRows(List<MapNodeData> previousRow, List<MapNodeData> currentRow)
    {
        bool forceAllDiagonals = previousRow.Count == 1 &&
                                 previousRow[0].NodeType == MapNodeType.Start;

        foreach (var prevNode in previousRow)
        {
            int prevColumn = prevNode.GridPosition.x;
            var nodeAbove = currentRow.Find(n => n.GridPosition.x == prevColumn);
            if (nodeAbove != null)
            {
                prevNode.ConnectedNodeIds.Add(nodeAbove.Id);
            }
            
            float probability = forceAllDiagonals ? 1f : _config.DiagonalConnectionProbability;

            if (prevColumn > 0 && Random.value < probability)
            {
                var nodeAboveLeft = currentRow.Find(n => n.GridPosition.x == prevColumn - 1);
                if (nodeAboveLeft != null)
                {
                    prevNode.ConnectedNodeIds.Add(nodeAboveLeft.Id);
                }
            }

            if (prevColumn < _config.ColumnsPerRow - 1 && Random.value < probability)
            {
                var nodeAboveRight = currentRow.Find(n => n.GridPosition.x == prevColumn + 1);
                if (nodeAboveRight != null)
                {
                    prevNode.ConnectedNodeIds.Add(nodeAboveRight.Id);
                }
            }
        }
    }

    public void ConnectToSingleNode(List<MapNodeData> previousRow, MapNodeData targetNode)
    {
        foreach (var prevNode in previousRow)
        {
            prevNode.ConnectedNodeIds.Add(targetNode.Id);
        }
    }

    public void ConnectGuaranteedConnections(List<MapNodeData> previousRow, List<MapNodeData> checkpointRow)
    {
        foreach (var checkpointNode in checkpointRow)
        {
            int checkpointColumn = checkpointNode.GridPosition.x;

            var nodeAbove = previousRow.Find(n => n.GridPosition.x == checkpointColumn);
            if (nodeAbove != null)
            {
                nodeAbove.ConnectedNodeIds.Add(checkpointNode.Id);
            }

            if (checkpointColumn > 0)
            {
                var nodeAboveLeft = previousRow.Find(n => n.GridPosition.x == checkpointColumn - 1);
                if (nodeAboveLeft != null)
                {
                    nodeAboveLeft.ConnectedNodeIds.Add(checkpointNode.Id);
                }
            }
            
            if (checkpointColumn < _config.ColumnsPerRow - 1)
            {
                var nodeAboveRight = previousRow.Find(n => n.GridPosition.x == checkpointColumn + 1);
                if (nodeAboveRight != null)
                {
                    nodeAboveRight.ConnectedNodeIds.Add(checkpointNode.Id);
                }
            }
        }
    }
}
