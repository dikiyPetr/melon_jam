using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class AIMotivationPathController : MonoBehaviour
{
    [Inject] private AIMotivationData _motivationData;

    private MapData _mapData;
    private AIPathfinder _pathfinder;
    private List<string> _currentPath;
    private HashSet<string> _validNextNodes;

    public List<string> CurrentPath => _currentPath;
    public HashSet<string> ValidNextNodes => _validNextNodes;

    private void Awake()
    {
        DI.Inject(this);
        _currentPath = new List<string>();
        _validNextNodes = new HashSet<string>();
    }

    public void Initialize(MapData mapData)
    {
        _mapData = mapData;
        _pathfinder = new AIPathfinder(mapData);
    }

    public void UpdatePath(string currentNodeId)
    {
        if (_motivationData == null || _pathfinder == null)
        {
            _currentPath.Clear();
            _validNextNodes.Clear();
            return;
        }

        var desiredNodeType = GetMostDesiredNodeType();
        _currentPath = _pathfinder.FindPathToDesiredNode(currentNodeId, desiredNodeType);
        _validNextNodes = _pathfinder.FindValidNextNodes(currentNodeId, desiredNodeType);
    }

    private MapNodeType GetMostDesiredNodeType()
    {
        var motivations = new List<MotivationStat>
        {
            _motivationData.Aggression,
            _motivationData.Wealth,
            _motivationData.Adventure
        };

        var maxMotivation = motivations[0];
        foreach (var motivation in motivations)
        {
            if (motivation.CurrentValue > maxMotivation.CurrentValue)
            {
                maxMotivation = motivation;
            }
        }

        return maxMotivation.GetDesiredNodeType();
    }
}
