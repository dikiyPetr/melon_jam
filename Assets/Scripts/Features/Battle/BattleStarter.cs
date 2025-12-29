using UnityEngine;
using VContainer;

public class BattleStarter : MonoBehaviour
{
    [SerializeField] private BattleController battleController;

    [Header("Enemy Setup")] [SerializeField]
    private int enemyMaxHP = 30;

    [SerializeField] private int enemyAttack = 3;

    [Inject] private PlayerHolder _playerHolder;

    private void Awake()
    {
        DI.Inject(this);
    }

    private void Start()
    {
        if (battleController != null && _playerHolder != null)
        {
            BattleCharacterData playerData = _playerHolder.GetBattleData();
            BattleCharacterData enemyData = new BattleCharacterData(enemyMaxHP, enemyAttack, null, null);

            battleController.InitializeBattle(playerData, enemyData);

            battleController.OnBattleEnded += OnBattleFinished;
        }
    }

    private void OnDestroy()
    {
        if (battleController != null)
        {
            battleController.OnBattleEnded -= OnBattleFinished;
        }
    }

    private void OnBattleFinished(bool playerWon)
    {
        BattleCharacterData playerDataAfterBattle = battleController.GetPlayerBattleDataAfterFight();

        if (playerDataAfterBattle != null)
        {
            _playerHolder.UpdateFromBattleData(playerDataAfterBattle);
        }

        if (playerWon)
        {
        }
    }
}