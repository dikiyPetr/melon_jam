using System;
using UnityEngine;

[Serializable]
public class MapNodeTypeWeight
{
    public MapNodeType NodeType;
    [Range(0f, 1f)]
    public float Weight = 1f;
}
