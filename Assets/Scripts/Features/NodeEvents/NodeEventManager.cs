using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class NodeEventManager : MonoBehaviour
{
    [SerializeField] private NodeEventsConfig _eventsConfig;

    [Inject] private AIMotivationData _motivationData;

    private Dictionary<MapNodeType, List<NodeEvent>> _availableEvents;
    private HashSet<NodeEvent> _usedEvents;

    public event Action<NodeEvent> OnEventTriggered;

    private void Awake()
    {
        DI.Inject(this);
        _availableEvents = new Dictionary<MapNodeType, List<NodeEvent>>();
        _usedEvents = new HashSet<NodeEvent>();
    }

    public void Initialize()
    {
        if (_eventsConfig == null) return;

        _availableEvents.Clear();
        _usedEvents.Clear();

        foreach (MapNodeType nodeType in Enum.GetValues(typeof(MapNodeType)))
        {
            var events = _eventsConfig.GetEventsForNodeType(nodeType);
            if (events.Count > 0)
            {
                _availableEvents[nodeType] = new List<NodeEvent>(events);
            }
        }
    }

    public NodeEvent GetEventForNode(MapNodeType nodeType)
    {
        if (!_availableEvents.ContainsKey(nodeType) || _availableEvents[nodeType].Count == 0)
        {
            return null;
        }

        var availableEvents = _availableEvents[nodeType];
        var unusedEvents = availableEvents.FindAll(e => !_usedEvents.Contains(e));

        if (unusedEvents.Count == 0)
        {
            return null;
        }

        var selectedEvent = unusedEvents[UnityEngine.Random.Range(0, unusedEvents.Count)];

        if (selectedEvent.IsOneTime)
        {
            _usedEvents.Add(selectedEvent);
        }

        OnEventTriggered?.Invoke(selectedEvent);
        return selectedEvent;
    }

    public void ApplyEventChoice(NodeEventChoice choice)
    {
        if (choice == null || _motivationData == null) return;

        foreach (var change in choice.MotivationChanges)
        {
            ApplyMotivationChange(change);
        }
    }

    private void ApplyMotivationChange(MotivationChange change)
    {
        switch (change.MotivationType)
        {
            case MotivationType.Aggression:
                _motivationData.Aggression.AddValue(change.ChangeValue);
                break;
            case MotivationType.Wealth:
                _motivationData.Wealth.AddValue(change.ChangeValue);
                break;
            case MotivationType.Adventure:
                _motivationData.Adventure.AddValue(change.ChangeValue);
                break;
        }
    }
}
