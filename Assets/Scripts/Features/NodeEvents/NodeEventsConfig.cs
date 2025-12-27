using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/Node Events Config", fileName = "NodeEventsConfig")]
public class NodeEventsConfig : ScriptableObject
{
    public List<NodeTypeEventPool> EventPools = new List<NodeTypeEventPool>();

    public List<NodeEvent> GetEventsForNodeType(MapNodeType nodeType)
    {
        var pool = EventPools.Find(p => p.NodeType == nodeType);
        return pool?.Events ?? new List<NodeEvent>();
    }
}

[Serializable]
public class NodeTypeEventPool
{
    public MapNodeType NodeType;
    public List<NodeEvent> Events = new List<NodeEvent>();
}
