using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PlayerTurnUI : MonoBehaviour
{
    [SerializeField] private Button _nextTurnButton;

    [Inject] private PlayerTurnController _playerTurnController;

    private void Awake()
    {
        DI.Inject(this);

        if (_nextTurnButton != null)
        {
            _nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
        }
    }

    private void Start()
    {
        if (_playerTurnController != null)
        {
            _playerTurnController.OnTurnStateChanged += UpdateButtonState;
        }

        UpdateButtonState();
    }

    private void OnNextTurnButtonClicked()
    {
        if (_playerTurnController != null)
        {
            _playerTurnController.MakeNextTurn();
        }
    }

    private void UpdateButtonState()
    {
        if (_nextTurnButton == null) return;

        bool canMove = _playerTurnController != null && _playerTurnController.CanMakeNextTurn();

        _nextTurnButton.interactable = canMove;
    }

    private void OnDestroy()
    {
        if (_playerTurnController != null)
        {
            _playerTurnController.OnTurnStateChanged -= UpdateButtonState;
        }

        if (_nextTurnButton != null)
        {
            _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClicked);
        }
    }
}
