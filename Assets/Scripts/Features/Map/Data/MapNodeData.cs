using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapNodeData
{
    public string Id;
    public MapNodeType NodeType;
    public Vector2Int GridPosition;
    public List<string> ConnectedNodeIds;
    public bool IsVisited;
    public bool IsAvailable;

    public MapNodeData(string id, MapNodeType nodeType, Vector2Int gridPosition)
    {
        Id = id;
        NodeType = nodeType;
        GridPosition = gridPosition;
        ConnectedNodeIds = new List<string>();
        IsVisited = false;
        IsAvailable = false;
    }
}
