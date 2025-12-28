using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Battle Config")]
public class BattleConfig : ScriptableObject
{
    [Header("Attack Shake Settings")]
    public float attackShakeDuration = 0.2f;
    public float attackShakeIntensity = 0.15f;

    [Header("Damage Flash Settings")]
    public float damageFlashDuration = 0.3f;
    public int damageFlashCount = 3;
    public Color damageFlashColor = Color.white;

    [Header("Damage Numbers Settings")]
    public float damageNumberLifetime = 1.5f;
    public float damageNumberMoveSpeed = 2f;
    public float damageNumberFadeDelay = 0.5f;

    [Header("Health Bar Settings")]
    public Color healthBarFullColor = Color.green;
    public Color healthBarLowColor = Color.red;
    public float healthBarLowThreshold = 0.3f;
}
