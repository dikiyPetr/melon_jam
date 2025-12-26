public class MapPathValidator
{
    private MapData _mapData;

    public MapPathValidator(MapData mapData)
    {
        _mapData = mapData;
    }

    public bool IsNodeAccessible(string nodeId)
    {
        var node = _mapData.GetNodeById(nodeId);
        if (node == null) return false;

        if (node.IsVisited) return false;

        if (!node.IsAvailable) return false;

        return true;
    }

    public bool IsConnectedToCurrentNode(string nodeId)
    {
        var currentNode = _mapData.GetCurrentNode();
        if (currentNode == null) return false;

        return currentNode.ConnectedNodeIds.Contains(nodeId);
    }

    public bool CanMoveToNode(string nodeId)
    {
        return IsNodeAccessible(nodeId) && IsConnectedToCurrentNode(nodeId);
    }
}
