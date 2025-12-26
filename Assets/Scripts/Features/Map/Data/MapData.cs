using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Map Data", fileName = "MapData")]
public class MapData : ScriptableObject
{
    public Dictionary<int, MapRowData> Rows = new Dictionary<int, MapRowData>();
    public string CurrentNodeId;
    public HashSet<string> VisitedNodeIds = new HashSet<string>();

    public List<MapNodeData> Nodes
    {
        get
        {
            List<MapNodeData> allNodes = new List<MapNodeData>();
            foreach (var row in Rows.Values)
            {
                allNodes.AddRange(row.GetAllNodes());
            }
            return allNodes;
        }
    }

    public MapNodeData GetNodeById(string nodeId)
    {
        foreach (var row in Rows.Values)
        {
            foreach (var node in row.GetAllNodes())
            {
                if (node.Id == nodeId)
                    return node;
            }
        }
        return null;
    }

    public MapNodeData GetCurrentNode()
    {
        return GetNodeById(CurrentNodeId);
    }

    public MapRowData GetRow(int rowIndex)
    {
        return Rows.ContainsKey(rowIndex) ? Rows[rowIndex] : null;
    }

    public void Reset()
    {
        Rows.Clear();
        CurrentNodeId = null;
        VisitedNodeIds.Clear();
    }
}
