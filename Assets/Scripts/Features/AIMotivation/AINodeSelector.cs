using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AINodeSelector
{
    private AIMotivationData _motivationData;
    private MapData _mapData;

    public bool UseRandomSelection = true;

    public AINodeSelector(AIMotivationData motivationData, MapData mapData)
    {
        _motivationData = motivationData;
        _mapData = mapData;
    }

    public string SelectNextNode(HashSet<string> validNextNodes)
    {
        if (validNextNodes == null || validNextNodes.Count == 0)
        {
            Debug.Log('d');
            return null;
        }

        var top2Motivations = GetTop2Motivations(validNextNodes);
        if (top2Motivations.Count == 0)
        {
            Debug.Log('d');
            return validNextNodes.First();
        }

        var selectedMotivation = SelectMotivationRandomly(top2Motivations);
        return SelectNodeForMotivation(validNextNodes, selectedMotivation);
    }

    public HashSet<string> GetSelectableNodes(HashSet<string> validNextNodes)
    {
        var selectableNodes = new HashSet<string>();

        if (validNextNodes == null || validNextNodes.Count == 0)
        {
            return selectableNodes;
        }

        var top2Motivations = GetTop2Motivations(validNextNodes);
        if (top2Motivations.Count == 0)
        {
            return new HashSet<string>(validNextNodes);
        }

        foreach (var motivation in top2Motivations)
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

    private List<MotivationStat> GetTop2Motivations(HashSet<string> validNextNodes)
    {
        var motivations = new List<MotivationStat>
        {
            _motivationData.Aggression,
            _motivationData.Wealth,
            _motivationData.Adventure
        };

        var availableMotivations = motivations.Where(m => HasNodeWithDesiredType(validNextNodes, m.GetDesiredNodeType())).ToList();

        if (availableMotivations.Count == 0)
        {
            return motivations
                .OrderByDescending(m => GetMotivationWeight(m))
                .Take(2)
                .ToList();
        }

        return availableMotivations
            .OrderByDescending(m => GetMotivationWeight(m))
            .Take(2)
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

        Debug.Log($"AI Motivation Selection ({(UseRandomSelection ? "Random" : "Max Weight")}) - Total Weight: {totalWeight:F2}");
        foreach (var m in motivations)
        {
            float weight = GetMotivationWeight(m);
            float chance = (weight / totalWeight) * 100f;
            string nodeTypeInfo = m.GetDesiredNodeType() == m.HighValueNodeType ? "High" : "Low";
            Debug.Log($"  {m.GetDesiredNodeType()} ({nodeTypeInfo}): weight={weight:F2}, chance={chance:F1}%");
        }

        MotivationStat selectedMotivation;

        if (UseRandomSelection)
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
            selectedMotivation = motivations[0];
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
