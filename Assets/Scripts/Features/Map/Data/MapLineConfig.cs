using System;

[Serializable]
public class MapLineConfig
{
    public MapLineType LineType;
    public int NodeCount = 3;
    public bool HasGuaranteedConnections = false;

    public MapLineConfig(MapLineType lineType, int nodeCount = 3, bool hasGuaranteedConnections = false)
    {
        LineType = lineType;
        NodeCount = nodeCount;
        HasGuaranteedConnections = hasGuaranteedConnections;
    }
}
