using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/Conditions/Gold Condition", fileName = "GoldCondition")]
public class GoldCondition : EventCondition
{
    public int RequiredGold;

    public override bool Check(PlayerHolder playerHolder)
    {
        return playerHolder.Gold >= RequiredGold;
    }
}
