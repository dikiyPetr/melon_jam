using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class NodeEventManager : MonoBehaviour
{
    [SerializeField] private NodeEventsConfig _eventsConfig;
    [SerializeField] private EventIntentHandler _intentHandler;
    [SerializeField] private RewardConfig _rewardConfig;
    [SerializeField] private DialogManager _dialogManager;

    [Inject] private AIMotivationData _motivationData;
    [Inject] private PlayerHolder _playerHolder;

    private List<NodeTypeEventPool> _availableEvents;
    private List<NodeEvent> _usedEvents;

    public event Action<NodeEvent> OnEventTriggered;

    private void Awake()
    {
        DI.Inject(this);
        _availableEvents = new List<NodeTypeEventPool>();
        _usedEvents = new List<NodeEvent>();
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
                _availableEvents.Add(new NodeTypeEventPool
                {
                    NodeType = nodeType,
                    Events = new List<NodeEvent>(events)
                });
            }
        }
    }

    public NodeEvent GetEventForNode(MapNodeType nodeType)
    {
        var eventPool = _availableEvents.Find(pool => pool.NodeType == nodeType);
        if (eventPool == null || eventPool.Events.Count == 0)
        {
            return null;
        }

        var unusedEvents = eventPool.Events.FindAll(e => !_usedEvents.Contains(e));

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

    public void ShowEvent(NodeEvent nodeEvent)
    {
        if (_dialogManager != null && nodeEvent != null)
        {
            _dialogManager.ShowNodeEvent(nodeEvent);
        }
    }

    public void ShowEventForNode(MapNodeType nodeType)
    {
        NodeEvent nodeEvent = GetEventForNode(nodeType);
        if (nodeEvent != null)
        {
            ShowEvent(nodeEvent);
        }
    }

    public void ApplyEventChoice(NodeEventChoice choice)
    {
        if (choice == null) return;

        foreach (var reward in choice.Rewards)
        {
            ApplyReward(reward);
        }

        if (_intentHandler != null && choice.Intent != null)
        {
            _intentHandler.HandleIntent(choice.Intent);
        }
    }

    private void ApplyReward(EventReward reward)
    {
        if (_rewardConfig == null) return;

        switch (reward.RewardType)
        {
            case RewardType.Aggression:
                if (_motivationData != null)
                {
                    float value = _rewardConfig.GetMotivationValue(reward.Amount);
                    _motivationData.Aggression.AddValue(value);
                }
                break;

            case RewardType.Wealth:
                if (_motivationData != null)
                {
                    float value = _rewardConfig.GetMotivationValue(reward.Amount);
                    _motivationData.Wealth.AddValue(value);
                }
                break;

            case RewardType.Adventure:
                if (_motivationData != null)
                {
                    float value = _rewardConfig.GetMotivationValue(reward.Amount);
                    _motivationData.Adventure.AddValue(value);
                }
                break;

            case RewardType.Heal:
                if (_playerHolder != null)
                {
                    int healValue = _rewardConfig.GetHealValue(reward.Amount);
                    _playerHolder.Heal(healValue);
                }
                break;

            case RewardType.AttackIncrease:
                if (_playerHolder != null)
                {
                    int attackIncrease = _rewardConfig.GetAttackIncreaseValue(reward.Amount);
                    _playerHolder.SetAttackDamage(_playerHolder.AttackDamage + attackIncrease);
                }
                break;
            case RewardType.Gold:
                if (_motivationData != null)
                {
                    int value = _rewardConfig.GetGoldValue(reward.Amount);
                    _playerHolder.AddGold(value);
                }
                break;

        }
    }
}
