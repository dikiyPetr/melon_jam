using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    [Header("Battle Characters")]
    [SerializeField] private BattleCharacter playerCharacter;
    [SerializeField] private BattleCharacter enemyCharacter;

    [Header("Character Views")]
    [SerializeField] private BattleCharacterView playerView;
    [SerializeField] private BattleCharacterView enemyView;

    [Header("UI")]
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI enemyHPText;
    [SerializeField] private TextMeshProUGUI battleLogText;

    [Header("Damage Numbers")]
    [SerializeField] private DamageNumberSpawner playerDamageSpawner;
    [SerializeField] private DamageNumberSpawner enemyDamageSpawner;

    [Header("Settings")]
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float turnDelay = 1f;
    [SerializeField] private GameObject battlePanel;

    private bool isPlayerTurn = true;
    private bool isBattleActive = false;

    public event Action<bool> OnBattleEnded;

    private void Awake()
    {
        if (startBattleButton != null)
        {
            startBattleButton.onClick.AddListener(OnStartBattleButtonClicked);
        }
    }

    private void Start()
    {
        if (playerCharacter != null)
        {
            playerCharacter.OnHealthChanged += OnPlayerHealthChanged;
            playerCharacter.OnDeath += OnPlayerDeath;
        }

        if (enemyCharacter != null)
        {
            enemyCharacter.OnHealthChanged += OnEnemyHealthChanged;
            enemyCharacter.OnDeath += OnEnemyDeath;
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (playerCharacter != null)
        {
            playerCharacter.OnHealthChanged -= OnPlayerHealthChanged;
            playerCharacter.OnDeath -= OnPlayerDeath;
        }

        if (enemyCharacter != null)
        {
            enemyCharacter.OnHealthChanged -= OnEnemyHealthChanged;
            enemyCharacter.OnDeath -= OnEnemyDeath;
        }

        if (startBattleButton != null)
        {
            startBattleButton.onClick.RemoveListener(OnStartBattleButtonClicked);
        }
    }

    private void OnStartBattleButtonClicked()
    {
        StartBattle();
    }

    public void InitializeBattle(BattleCharacterData playerData, BattleCharacterData enemyData)
    {
        if (playerCharacter != null)
        {
            playerCharacter.Initialize(playerData.MaxHP, playerData.AttackDamage);
        }

        if (enemyCharacter != null)
        {
            enemyCharacter.Initialize(enemyData.MaxHP, enemyData.AttackDamage);
        }

        UpdateUI();
    }

    public BattleCharacterData GetPlayerBattleDataAfterFight()
    {
        if (playerCharacter != null)
        {
            return new BattleCharacterData(playerCharacter.CurrentHP, playerCharacter.AttackDamage);
        }

        return null;
    }

    public void StartBattle()
    {
        if (isBattleActive) return;

        isBattleActive = true;
        isPlayerTurn = true;

        if (battlePanel != null)
        {
            battlePanel.SetActive(true);
        }

        if (startBattleButton != null)
        {
            startBattleButton.interactable = false;
        }

        UpdateUI();
        AddBattleLog("Бой начался!");

        StartCoroutine(AutoBattleLoop());
    }

    private IEnumerator AutoBattleLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (isBattleActive && playerCharacter.IsAlive && enemyCharacter.IsAlive)
        {
            if (isPlayerTurn)
            {
                yield return StartCoroutine(ExecutePlayerAttack());
            }
            else
            {
                yield return StartCoroutine(ExecuteEnemyAttack());
            }

            yield return new WaitForSeconds(turnDelay);
        }
    }

    private IEnumerator ExecutePlayerAttack()
    {
        playerView.PlayAttackAnimation();

        yield return new WaitForSeconds(attackDelay);

        int damage = playerCharacter.AttackDamage;
        enemyCharacter.TakeDamage(damage);

        if (enemyDamageSpawner != null && enemyCharacter != null)
        {
            enemyDamageSpawner.SpawnDamageNumber(enemyCharacter.transform.position, damage);
        }

        AddBattleLog($"Игрок атаковал! Урон: {damage}");

        isPlayerTurn = false;
    }

    private IEnumerator ExecuteEnemyAttack()
    {
        enemyView.PlayAttackAnimation();

        yield return new WaitForSeconds(attackDelay);

        int damage = enemyCharacter.AttackDamage;
        playerCharacter.TakeDamage(damage);

        if (playerDamageSpawner != null && playerCharacter != null)
        {
            playerDamageSpawner.SpawnDamageNumber(playerCharacter.transform.position, damage);
        }

        AddBattleLog($"Враг атаковал! Урон: {damage}");

        isPlayerTurn = true;
    }

    private void OnPlayerHealthChanged(int currentHP)
    {
        UpdateUI();
    }

    private void OnEnemyHealthChanged(int currentHP)
    {
        UpdateUI();
    }

    private void OnPlayerDeath()
    {
        EndBattle(false);
        AddBattleLog("Поражение!");
    }

    private void OnEnemyDeath()
    {
        EndBattle(true);
        AddBattleLog("Победа!");
    }

    private void EndBattle(bool playerWon)
    {
        isBattleActive = false;

        if (battlePanel != null)
        {
            battlePanel.SetActive(false);
        }

        OnBattleEnded?.Invoke(playerWon);
    }

    private void UpdateUI()
    {
        if (playerHPText != null && playerCharacter != null)
        {
            playerHPText.text = $"HP: {playerCharacter.CurrentHP}/{playerCharacter.MaxHP}";
        }

        if (enemyHPText != null && enemyCharacter != null)
        {
            enemyHPText.text = $"HP: {enemyCharacter.CurrentHP}/{enemyCharacter.MaxHP}";
        }
    }

    private void AddBattleLog(string message)
    {
        if (battleLogText != null)
        {
            battleLogText.text = message;
        }
    }
}
