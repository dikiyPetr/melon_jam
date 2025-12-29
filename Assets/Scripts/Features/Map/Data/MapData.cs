using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Map Data", fileName = "MapData")]
public class MapData : ScriptableObject
{
    [NonSerialized]
    public Dictionary<int, MapRowData> Rows;

    public string CurrentNodeId;

    [NonSerialized]
    public HashSet<string> VisitedNodeIds;

    private void OnEnable()
    {
        if (Rows == null)
        {
            Rows = new Dictionary<int, MapRowData>(IntEqualityComparer.Default);
        }
        if (VisitedNodeIds == null)
        {
            VisitedNodeIds = new HashSet<string>(StringEqualityComparer.Default);
        }
    }

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