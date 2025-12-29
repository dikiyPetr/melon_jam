using System;

[Serializable]
public class BattleCharacterData
{
    public int MaxHP;
    public int AttackDamage;
    public EventIntent VictoryIntent;
    public DefeatIntent DefeatIntent;

    public BattleCharacterData(int maxHP, int attackDamage, EventIntent victoryIntent,
        DefeatIntent defeatIntent)
    {
        MaxHP = maxHP;
        AttackDamage = attackDamage;
        VictoryIntent = victoryIntent;
        DefeatIntent = defeatIntent;
    }
}