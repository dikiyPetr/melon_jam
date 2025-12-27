using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Map Config", fileName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("Map Structure")] public int ColumnsPerRow = 3;

    public List<MapLineConfig> MapLines = new List<MapLineConfig> { };

    [Header("Checkpoint Settings")] public List<MapNodeType> CheckpointNodeTypes = new List<MapNodeType>
    {
        MapNodeType.Combat,
        MapNodeType.Shop,
        MapNodeType.Treasure
    };

    [Header("Node Selection Settings")] public List<MapNodeType> SelectableNodeTypes = new List<MapNodeType>
    {
        MapNodeType.Combat,
        MapNodeType.Event,
        MapNodeType.Shop,
        MapNodeType.Rest,
        MapNodeType.Treasure
    };

    public int NodeTypeChoicesCount = 2;

    [Header("Connections")] [Range(0f, 1f)]
    public float DiagonalConnectionProbability = 0.3f;

    [Header("Visual Settings")] public MapNodeVisualData NodeVisualData;

    [Header("Animation Settings")] public float NodeRevealDuration = 0.3f;

    public float ConnectionRevealDuration = 0.2f;
    public float PlayerMovementDuration = 0.7f;
}