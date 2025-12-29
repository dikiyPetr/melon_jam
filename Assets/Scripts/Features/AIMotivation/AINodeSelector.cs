using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AINodeSelector
{
    private AIMotivationData _motivationData;
    private MapData _mapData;
    private AIMotivationConfig _config;

    public bool UseRandomSelection = true;

    public AINodeSelector(AIMotivationData motivationData, MapData mapData, AIMotivationConfig config)
    {
        _motivationData = motivationData;
        _mapData = mapData;
        _config = config;
    }

    public string SelectNextNode(HashSet<string> validNextNodes)
    {
        if (validNextNodes == null || validNextNodes.Count == 0)
        {
            return null;
        }

        var nodeMotivationPairs = new List<(string nodeId, float motivationValue)>();

        foreach (var nodeId in validNextNodes)
        {
            var node = _mapData.GetNodeById(nodeId);
            if (node == null) continue;

            float maxMotivation = GetMaxMotivationForNode(node);
            nodeMotivationPairs.Add((nodeId, maxMotivation));
        }

        if (nodeMotivationPairs.Count == 0)
        {
            return validNextNodes.First();
        }

        var sortedPairs = nodeMotivationPairs.OrderByDescending(p => p.motivationValue).ToList();

        if (sortedPairs.Count == 1)
        {
            return sortedPairs[0].nodeId;
        }

        float maxValue = sortedPairs[0].motivationValue;
        float secondValue = sortedPairs[1].motivationValue;
        float difference = maxValue - secondValue;

        float threshold = _config != null ? _config.MotivationDifferenceThreshold : 2f;

        string nodesInfo = string.Join(", ", sortedPairs.Take(2).Select(p =>
        {
            var node = _mapData.GetNodeById(p.nodeId);
            return $"{node?.NodeType}:{p.motivationValue:F0}";
        }));
        Debug.Log($"AI Node: T={threshold:F1}, D={difference:F1} | {nodesInfo}");

        if (difference > threshold)
        {
            return sortedPairs[0].nodeId;
        }
        else if (UseRandomSelection)
        {
            float totalWeight = maxValue + secondValue;
            if (totalWeight <= 0f)
            {
                return sortedPairs[Random.Range(0, 2)].nodeId;
            }

            float randomValue = Random.Range(0f, totalWeight);
            return randomValue <= maxValue ? sortedPairs[0].nodeId : sortedPairs[1].nodeId;
        }
        else
        {
            return sortedPairs[0].nodeId;
        }
    }

    public HashSet<string> GetSelectableNodes(HashSet<string> validNextNodes)
    {
        var selectableNodes = new HashSet<string>();

        if (validNextNodes == null || validNextNodes.Count == 0)
        {
            return selectableNodes;
        }

        var sortedMotivations = GetSortedMotivations(validNextNodes);
        if (sortedMotivations.Count == 0)
        {
            return new HashSet<string>(validNextNodes);
        }

        foreach (var motivation in sortedMotivations)
        {
            var desiredType = motivation.GetDesiredNodeType();

            foreach (var nodeId in validNextNodes)
            {
                var node = _mapData.GetNodeById(nodeId);
                if (node != null)
                {
                    if (node.NodeType == desiredType)
                    {
                        selectableNodes.Add(nodeId);
                    }
                    else if (node.AvailableNodeTypes != null && node.AvailableNodeTypes.Contains(desiredType))
                    {
                        selectableNodes.Add(nodeId);
                    }
                }
            }
        }

        return selectableNodes;
    }

    private float GetMaxMotivationForNode(MapNodeData node)
    {
        var motivations = new List<MotivationStat>
        {
            _motivationData.Aggression,
            _motivationData.Wealth,
            _motivationData.Adventure
        };

        float maxMotivation = 0f;

        foreach (var motivation in motivations)
        {
            if (node.NodeType == motivation.HighValueNodeType)
            {
                float weight = motivation.CurrentValue;
                if (weight > maxMotivation)
                {
                    maxMotivation = weight;
                }
            }

            if (node.NodeType == motivation.LowValueNodeType)
            {
                float weight = motivation.MaxValue - motivation.CurrentValue;
                if (weight > maxMotivation)
                {
                    maxMotivation = weight;
                }
            }
        }

        return maxMotivation;
    }

    private List<MotivationStat> GetSortedMotivations(HashSet<string> validNextNodes)
    {
        var motivations = new List<MotivationStat>
        {
            _motivationData.Aggression,
            _motivationData.Wealth,
            _motivationData.Adventure
        };

        var availableMotivations =
            motivations.Where(m => HasNodeWithDesiredType(validNextNodes, m.GetDesiredNodeType())).ToList();

        if (availableMotivations.Count == 0)
        {
            return motivations
                .OrderByDescending(m => GetMotivationWeight(m))
                .ToList();
        }

        return availableMotivations
            .OrderByDescending(m => GetMotivationWeight(m))
            .ToList();
    }

    private bool HasNodeWithDesiredType(HashSet<string> validNextNodes, MapNodeType desiredType)
    {
        foreach (var nodeId in validNextNodes)
        {
            var node = _mapData.GetNodeById(nodeId);
            if (node != null)
            {
                if (node.NodeType == desiredType)
                {
                    return true;
                }

                if (node.AvailableNodeTypes != null && node.AvailableNodeTypes.Contains(desiredType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private float GetMotivationWeight(MotivationStat motivation)
    {
        var desiredType = motivation.GetDesiredNodeType();

        if (desiredType == motivation.HighValueNodeType)
        {
            return motivation.CurrentValue;
        }
        else
        {
            return motivation.MaxValue - motivation.CurrentValue;
        }
    }

    private MotivationStat SelectMotivationRandomly(List<MotivationStat> motivations)
    {
        if (motivations.Count == 0)
        {
            return null;
        }

        if (motivations.Count == 1)
        {
            return motivations[0];
        }

        float totalWeight = motivations.Sum(m => GetMotivationWeight(m));
        if (totalWeight <= 0f)
        {
            return motivations[Random.Range(0, motivations.Count)];
        }

        var sortedMotivations = motivations.OrderByDescending(m => GetMotivationWeight(m)).ToList();
        float maxWeight = GetMotivationWeight(sortedMotivations[0]);
        float secondWeight = sortedMotivations.Count > 1 ? GetMotivationWeight(sortedMotivations[1]) : 0f;
        float difference = maxWeight - secondWeight;

        float threshold = _config != null ? _config.MotivationDifferenceThreshold : 2f;
        bool useMaxWeight = difference < threshold;

        string motivationsInfo = string.Join(", ", motivations.Select(m =>
        {
            float weight = GetMotivationWeight(m);
            float chance = (weight / totalWeight) * 100f;
            string type = m.GetDesiredNodeType() == m.HighValueNodeType ? "H" : "L";
            return $"{m.GetDesiredNodeType()}({type}):{chance:F0}%";
        }));
        Debug.Log($"AI: T={threshold:F1}, D={difference:F1}, Max={useMaxWeight} | {motivationsInfo}");

        MotivationStat selectedMotivation;

        if (useMaxWeight)
        {
            selectedMotivation = sortedMotivations[0];
        }
        else if (UseRandomSelection)
        {
            float randomValue = Random.Range(0f, totalWeight);
            float currentSum = 0f;

            selectedMotivation = motivations[motivations.Count - 1];
            foreach (var motivation in motivations)
            {
                currentSum += GetMotivationWeight(motivation);
                if (randomValue <= currentSum)
                {
                    selectedMotivation = motivation;
                    break;
                }
            }
        }
        else
        {
            selectedMotivation = sortedMotivations[0];
        }

        Debug.Log($"Selected: {selectedMotivation.GetDesiredNodeType()}");

        return selectedMotivation;
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