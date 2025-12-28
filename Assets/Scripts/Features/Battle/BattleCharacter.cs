using System;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int attackDamage = 5;

    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }
    public int AttackDamage => attackDamage;
    public bool IsAlive => CurrentHP > 0;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    public void Initialize(int hp, int attack)
    {
        maxHP = hp;
        attackDamage = attack;
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        OnHealthChanged?.Invoke(CurrentHP);
        OnDamageTaken?.Invoke(damage);

        if (!IsAlive)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        OnHealthChanged?.Invoke(CurrentHP);
    }
}
