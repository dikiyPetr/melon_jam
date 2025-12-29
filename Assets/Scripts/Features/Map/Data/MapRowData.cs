using System.Collections.Generic;

public class MapRowData
{
    public int RowIndex;
    public MapRowType RowType;
    public Dictionary<int, MapNodeData> Nodes;

    public MapRowData(int rowIndex, MapRowType rowType)
    {
        RowIndex = rowIndex;
        RowType = rowType;
        Nodes = new Dictionary<int, MapNodeData>(IntEqualityComparer.Default);
    }

    public void AddNode(int columnIndex, MapNodeData node)
    {
        Nodes[columnIndex] = node;
    }

    public MapNodeData GetNode(int columnIndex)
    {
        return Nodes.ContainsKey(columnIndex) ? Nodes[columnIndex] : null;
    }

    public List<MapNodeData> GetAllNodes()
    {
        return new List<MapNodeData>(Nodes.Values);
    }
}
