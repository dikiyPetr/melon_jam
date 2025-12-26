using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapData GenerateMap(MapConfig config)
    {
        var mapData = ScriptableObject.CreateInstance<MapData>();
        mapData.Reset();

        var layoutBuilder = new MapLayoutBuilder(config);
        mapData.Rows = layoutBuilder.Build();

        var startNode = mapData.Nodes.Find(n => n.NodeType == MapNodeType.Start);
        if (startNode != null)
        {
            mapData.CurrentNodeId = startNode.Id;
            startNode.IsAvailable = true;
            startNode.IsVisited = true;
            mapData.VisitedNodeIds.Add(startNode.Id);

            UpdateAvailableNodes(mapData, startNode);
        }

        return mapData;
    }

    private void UpdateAvailableNodes(MapData mapData, MapNodeData currentNode)
    {
        foreach (var connectedNodeId in currentNode.ConnectedNodeIds)
        {
            var connectedNode = mapData.GetNodeById(connectedNodeId);
            if (connectedNode != null && !connectedNode.IsVisited)
            {
                connectedNode.IsAvailable = true;
            }
        }
    }
}