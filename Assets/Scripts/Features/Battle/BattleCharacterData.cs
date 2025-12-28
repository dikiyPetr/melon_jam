using System;

[Serializable]
public class BattleCharacterData
{
    public int MaxHP;
    public int AttackDamage;

    public BattleCharacterData(int maxHP, int attackDamage)
    {
        MaxHP = maxHP;
        AttackDamage = attackDamage;
    }
}
