using System.Collections.Generic;
using UnityEngine;

public class NodeTypeSelector
{
    private MapConfig _config;

    public NodeTypeSelector(MapConfig config)
    {
        _config = config;
    }

    public List<MapNodeType> GetRandomNodeTypes(int count)
    {
        if (_config == null || _config.SelectableNodeTypes == null || _config.SelectableNodeTypes.Count == 0)
        {
            return new List<MapNodeType>();
        }

        var availableTypes = new List<MapNodeType>(_config.SelectableNodeTypes);
        var result = new List<MapNodeType>();

        int typesToSelect = Mathf.Min(count, availableTypes.Count);

        for (int i = 0; i < typesToSelect; i++)
        {
            int randomIndex = Random.Range(0, availableTypes.Count);
            result.Add(availableTypes[randomIndex]);
            availableTypes.RemoveAt(randomIndex);
        }

        return result;
    }

    public void GenerateAvailableTypesForNodes(List<MapNodeData> nodes)
    {
        if (_config == null) return;

        foreach (var node in nodes)
        {
            if (!node.IsVisited && !node.HasSelectedType)
            {
                var types = GetRandomNodeTypes(_config.NodeTypeChoicesCount);
                node.SetAvailableTypes(types);
            }
        }
    }
}
