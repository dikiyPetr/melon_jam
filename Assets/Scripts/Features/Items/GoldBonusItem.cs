using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gold Bonus Item", fileName = "GoldBonusItem")]
public class GoldBonusItem : PlayerItem
{
    public float GoldBonusPercent;

    public override float ModifyGoldReward(float baseGold)
    {
        return baseGold * (1 + GoldBonusPercent / 100f);
    }
}
