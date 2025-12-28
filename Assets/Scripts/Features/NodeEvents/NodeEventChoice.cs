using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeEventChoice
{
    public string ButtonText;
    public List<EventReward> Rewards = new List<EventReward>();
    [SerializeReference] public EventIntent Intent;
    [SerializeReference] public EventCondition Condition;

    public bool CanChoose(PlayerHolder playerHolder)
    {
        if (Condition == null)
            return true;

        return Condition.Check(playerHolder);
    }
}

[Serializable]
public class EventReward
{
    public RewardType RewardType;
    public ChangeAmount Amount;
}

public enum RewardType
{
    Aggression,
    Wealth,
    Adventure,
    Heal,
    AttackIncrease,
    Gold,
}

public enum ChangeAmount
{
    SmallPositive,
    MediumPositive,
    LargePositive,
    SmallNegative,
    MediumNegative,
    LargeNegative,
}