using UnityEngine;

[CreateAssetMenu(menuName = "Items/Attack Multiplier Item", fileName = "AttackMultiplierItem")]
public class AttackMultiplierItem : PlayerItem
{
    public float AttackMultiplier;

    public override float ModifyAttackDamage(float baseAttack)
    {
        return baseAttack * AttackMultiplier;
    }
}
