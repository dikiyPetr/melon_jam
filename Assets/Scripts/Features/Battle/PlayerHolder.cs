using System;
using System.Collections.Generic;

public class PlayerHolder
{
    public int MaxHP = 100;
    public int CurrentHP = 100;
    public int AttackDamage = 5;
    public int Gold = 50;

    public List<PlayerItem> Items = new List<PlayerItem>();

    public event Action OnStatsChanged;

    public void SetAttackDamage(int attackDamage)
    {
        AttackDamage = attackDamage;
        OnStatsChanged?.Invoke();
    }

    public BattleCharacterData GetBattleData()
    {
        return new BattleCharacterData(CurrentHP, GetModifiedAttackDamage());
    }

    public void UpdateFromBattleData(BattleCharacterData battleData)
    {
        CurrentHP = battleData.MaxHP;
        OnStatsChanged?.Invoke();
    }

    public void UpdateCurrentHP(int currentHP)
    {
        CurrentHP = Math.Max(0, Math.Min(currentHP, MaxHP));
        OnStatsChanged?.Invoke();
    }

    public void Heal(int amount)
    {
        CurrentHP = Math.Min(CurrentHP + amount, MaxHP);
        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Math.Max(0, CurrentHP - damage);
        OnStatsChanged?.Invoke();
    }

    public bool IsAlive => CurrentHP > 0;

    public void AddGold(int value)
    {
        Gold = Math.Max(0, Gold + GetModifiedGoldReward(value));
        OnStatsChanged?.Invoke();
    }

    public void AddItem(PlayerItem item)
    {
        Items.Add(item);
        item.OnItemAdded(this);
        OnStatsChanged?.Invoke();
    }

    public int GetModifiedAttackDamage()
    {
        float modifiedAttack = AttackDamage;
        foreach (var item in Items)
        {
            modifiedAttack = item.ModifyAttackDamage(modifiedAttack);
        }
        return (int)modifiedAttack;
    }

    public int GetModifiedGoldReward(int baseGold)
    {
        float modifiedGold = baseGold;
        foreach (var item in Items)
        {
            modifiedGold = item.ModifyGoldReward(modifiedGold);
        }
        return (int)modifiedGold;
    }
}