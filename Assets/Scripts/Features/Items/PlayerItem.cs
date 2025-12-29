using System;
using UnityEngine;

public abstract class PlayerItem : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public virtual void OnItemAdded(PlayerHolder playerHolder) { }

    public virtual float ModifyGoldReward(float baseGold) { return baseGold; }

    public virtual float ModifyAttackDamage(float baseAttack) { return baseAttack; }
}