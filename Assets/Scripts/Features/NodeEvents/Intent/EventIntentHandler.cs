using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class EventIntentHandler : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [Inject] private PlayerHolder _playerHolder;
    [Inject] private DialogManager _dialogManager;
    [Inject] private NodeEventManager _nodeEventManager;

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
        else if (intent is RandomLootIntent randomLootIntent)
        {
            StartCoroutine(HandleRandomLootIntent(randomLootIntent));
        }
        else if (intent is VictoryIntent victoryIntent)
        {
            HandleVictoryIntent(victoryIntent);
        }
        else if (intent is DefeatIntent defeatIntent)
        {
            HandleDefeatIntent(defeatIntent);
        }
    }

    private void HandleBattleIntent(BattleIntent battleIntent)
    {
        if (_battleController == null || _playerHolder == null) return;

        BattleCharacterData playerData = _playerHolder.GetBattleData();
        BattleCharacterData enemyData = new BattleCharacterData(battleIntent.EnemyMaxHP, battleIntent.EnemyAttack,battleIntent.VictoryIntent,battleIntent.DefeatIntent);

        _battleController.InitializeBattle(playerData, enemyData, battleIntent.EnemySprite);


        _battleController.StartBattle();
    }

    private void HandleVictoryIntent(VictoryIntent victoryIntent)
    {
        if (_dialogManager == null) return;

        StartCoroutine(ShowVictoryDialogWithDelay(victoryIntent));
    }

    private System.Collections.IEnumerator ShowVictoryDialogWithDelay(VictoryIntent victoryIntent)
    {
        yield return new WaitForSeconds(0.1f);

        _nodeEventManager.ShowEvent(victoryIntent.Event);
    }

    private void HandleDefeatIntent(DefeatIntent defeatIntent)
    {
        if (_dialogManager == null) return;

        StartCoroutine(ShowDefeatDialogWithDelay(defeatIntent));
    }

    private System.Collections.IEnumerator ShowDefeatDialogWithDelay(DefeatIntent defeatIntent)
    {
        yield return new WaitForSeconds(0.1f);

        System.Collections.Generic.List<DialogButtonData> buttons =
            new System.Collections.Generic.List<DialogButtonData>
            {
                new DialogButtonData("Again", () =>
                {
                    _dialogManager.HideDialog();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                })
            };

        _dialogManager.ShowDialog(defeatIntent.Title, defeatIntent.Description, buttons, defeatIntent.Icon);
    }

    private System.Collections.IEnumerator HandleRandomLootIntent(RandomLootIntent randomLootIntent)
    {
        if (_playerHolder == null || _dialogManager == null) yield break;

        PlayerItem item = randomLootIntent.GetRandomItem();
        if (item == null) yield break;

        _playerHolder.AddItem(item);

        System.Collections.Generic.List<DialogButtonData> buttons =
            new System.Collections.Generic.List<DialogButtonData>
            {
                new DialogButtonData("OK", () => _dialogManager.HideDialog())
            };
        yield return new WaitForSeconds(0.1f);
        _dialogManager.ShowDialog(item.Name, item.Description, buttons, item.Icon);
    }
}