using System;

[Serializable]
public class BattleCharacterData
{
    public int MaxHP;
    public int AttackDamage;
    public VictoryIntent VictoryIntent;
    public DefeatIntent DefeatIntent;

    public BattleCharacterData(int maxHP, int attackDamage, VictoryIntent victoryIntent,
        DefeatIntent defeatIntent)
    {
        MaxHP = maxHP;
        AttackDamage = attackDamage;
        VictoryIntent = victoryIntent;
        DefeatIntent = defeatIntent;
    }
}