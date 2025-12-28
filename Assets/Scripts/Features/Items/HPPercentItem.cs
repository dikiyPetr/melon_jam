using UnityEngine;

[CreateAssetMenu(menuName = "Items/HP Percent Item", fileName = "HPPercentItem")]
public class HPPercentItem : PlayerItem
{
    public float HPPercentBonus;

    public override void OnItemAdded(PlayerHolder playerHolder)
    {
        int bonusHP = Mathf.RoundToInt(playerHolder.MaxHP * HPPercentBonus / 100f);
        playerHolder.MaxHP += bonusHP;
        playerHolder.CurrentHP += bonusHP;
    }
}
