using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/Reward Config", fileName = "RewardConfig")]
public class RewardConfig : ScriptableObject
{
    [Header("Motivation Change Values")] 
    public float SmallMotivation = 3f;
    public float MediumMotivation = 5f;
    public float LargeMotivation = 8f;

    [Header("Heal Values")] public int SmallHeal = 10;
    public int MediumHeal = 25;
    public int LargeHeal = 50;

    [Header("Attack Damage Increase Values")]
    public int SmallAttackIncrease = 1;
    public int MediumAttackIncrease = 3;
    public int LargeAttackIncrease = 5;

    [Header("Heal Values")] public int SmallGold = 30;
    public int MediumGold = 50;
    public int LargeGold = 100;

    public float GetMotivationValue(ChangeAmount amount)
    {
        switch (amount)
        {
            case ChangeAmount.SmallPositive: return SmallMotivation;
            case ChangeAmount.MediumPositive: return MediumMotivation;
            case ChangeAmount.LargePositive: return LargeMotivation;
            case ChangeAmount.SmallNegative: return -SmallMotivation;
            case ChangeAmount.MediumNegative: return -MediumMotivation;
            case ChangeAmount.LargeNegative: return -LargeMotivation;
            default: return 0f;
        }
    }

    public int GetHealValue(ChangeAmount amount)
    {
        switch (amount)
        {
            case ChangeAmount.SmallPositive: return SmallHeal;
            case ChangeAmount.MediumPositive: return MediumHeal;
            case ChangeAmount.LargePositive: return LargeHeal;
            default: return 0;
        }
    }

    public int GetAttackIncreaseValue(ChangeAmount amount)
    {
        switch (amount)
        {
            case ChangeAmount.SmallPositive: return SmallAttackIncrease;
            case ChangeAmount.MediumPositive: return MediumAttackIncrease;
            case ChangeAmount.LargePositive: return LargeAttackIncrease;
            default: return 0;
        }
    }

    public int GetGoldValue(ChangeAmount amount)
    {
        switch (amount)
        {
            case ChangeAmount.SmallPositive: return SmallGold;
            case ChangeAmount.MediumPositive: return MediumGold;
            case ChangeAmount.LargePositive: return LargeGold;
            case ChangeAmount.SmallNegative: return -SmallGold;
            case ChangeAmount.MediumNegative: return -MediumGold;
            case ChangeAmount.LargeNegative: return -LargeGold;
            default: return 0;
        }
    }
}