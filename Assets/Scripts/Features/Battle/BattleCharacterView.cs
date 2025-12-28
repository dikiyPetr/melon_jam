using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BattleCharacter))]
public class BattleCharacterView : MonoBehaviour
{
    private ShakeEffect shakeEffect;
    private FlashEffect flashEffect;
    private BattleCharacter battleCharacter;

    private void Awake()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        flashEffect = GetComponent<FlashEffect>();
        battleCharacter = GetComponent<BattleCharacter>();

        if (battleCharacter != null)
        {
            battleCharacter.OnDamageTaken += OnDamageTaken;
        }
    }

    private void OnDestroy()
    {
        if (battleCharacter != null)
        {
            battleCharacter.OnDamageTaken -= OnDamageTaken;
        }
    }

    private void OnDamageTaken(int damage)
    {
        PlayHitAnimation();
    }

    public void PlayAttackAnimation()
    {
        if (shakeEffect != null)
        {
            shakeEffect.Shake();
        }
    }

    public void PlayHitAnimation()
    {
        if (flashEffect != null)
        {
            flashEffect.Flash();
        }
    }
}
