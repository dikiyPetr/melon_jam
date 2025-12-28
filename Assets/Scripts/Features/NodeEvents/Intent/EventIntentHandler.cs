using UnityEngine;
using VContainer;

public class EventIntentHandler : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [Inject] private PlayerHolder _playerHolder;

    private void Awake()
    {
        DI.Inject(this);
    }

    public void HandleIntent(EventIntent intent)
    {
        if (intent == null) return;

        if (intent is BattleIntent battleIntent)
        {
            HandleBattleIntent(battleIntent);
        }
    }

    private void HandleBattleIntent(BattleIntent battleIntent)
    {
        if (_battleController == null || _playerHolder == null) return;

        BattleCharacterData playerData = _playerHolder.GetBattleData();
        BattleCharacterData enemyData = new BattleCharacterData(battleIntent.EnemyMaxHP, battleIntent.EnemyAttack);

        _battleController.InitializeBattle(playerData, enemyData);

        _battleController.OnBattleEnded += OnBattleFinished;

        _battleController.StartBattle();
    }

    private void OnBattleFinished(bool playerWon)
    {
        if (_battleController != null)
        {
            _battleController.OnBattleEnded -= OnBattleFinished;

            BattleCharacterData playerDataAfterBattle = _battleController.GetPlayerBattleDataAfterFight();
            if (playerDataAfterBattle != null && _playerHolder != null)
            {
                _playerHolder.UpdateFromBattleData(playerDataAfterBattle);
            }
        }
    }
}