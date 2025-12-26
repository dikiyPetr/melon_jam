using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Map Config", fileName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("Map Structure")] public int ColumnsPerRow = 3;

    public List<MapLineConfig> MapLines = new List<MapLineConfig> { };

    [Header("Connections")] [Range(0f, 1f)]
    public float DiagonalConnectionProbability = 0.3f;

    [Header("Visual Settings")] public MapNodeVisualData NodeVisualData;

    [Header("Animation Settings")] public float NodeRevealDuration = 0.3f;

    public float ConnectionRevealDuration = 0.2f;
    public float PlayerMovementDuration = 0.7f;
}