using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AINodeSelector
{
    private AIMotivationData _motivationData;
    private MapData _mapData;

    public AINodeSelector(AIMotivationData motivationData, MapData mapData)
    {
        _motivationData = motivationData;
        _mapData = mapData;
    }

    public string SelectNextNode(HashSet<string> validNextNodes)
    {
        if (validNextNodes == null || validNextNodes.Count == 0)
        {
            return null;
        }

        var top2Motivations = GetTop2Motivations();
        if (top2Motivations.Count == 0)
        {
            return validNextNodes.First();
        }

        var selectedMotivation = SelectMotivationRandomly(top2Motivations);
        return SelectNodeForMotivation(validNextNodes, selectedMotivation);
    }

    private List<MotivationStat> GetTop2Motivations()
    {
        var motivations = new List<MotivationStat>
        {
            _motivationData.Aggression,
            _motivationData.Wealth,
            _motivationData.Adventure
        };

        return motivations
            .OrderByDescending(m => m.CurrentValue)
            .Take(2)
            .ToList();
    }

    private MotivationStat SelectMotivationRandomly(List<MotivationStat> motivations)
    {
        if (motivations.Count == 1)
        {
            return motivations[0];
        }

        float totalWeight = motivations.Sum(m => m.CurrentValue);
        if (totalWeight <= 0f)
        {
            return motivations[Random.Range(0, motivations.Count)];
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (var motivation in motivations)
        {
            currentSum += motivation.CurrentValue;
            if (randomValue <= currentSum)
            {
                return motivation;
            }
        }

        return motivations[motivations.Count - 1];
    }

    private string SelectNodeForMotivation(HashSet<string> validNextNodes, MotivationStat motivation)
    {
        var desiredNodeType = motivation.GetDesiredNodeType();

        foreach (var nodeId in validNextNodes)
        {
            var node = _mapData.GetNodeById(nodeId);
            if (node != null && node.NodeType == desiredNodeType)
            {
                return nodeId;
            }
        }

        var nodesWithAvailableTypes = new List<(string nodeId, MapNodeType type)>();
        foreach (var nodeId in validNextNodes)
        {
            var node = _mapData.GetNodeById(nodeId);
            if (node != null && node.AvailableNodeTypes != null && node.AvailableNodeTypes.Contains(desiredNodeType))
            {
                nodesWithAvailableTypes.Add((nodeId, desiredNodeType));
            }
        }

        if (nodesWithAvailableTypes.Count > 0)
        {
            return nodesWithAvailableTypes[Random.Range(0, nodesWithAvailableTypes.Count)].nodeId;
        }

        return validNextNodes.First();
    }
}
