using UnityEngine;
using TMPro;
using VContainer;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI goldText;
    [Inject] private PlayerHolder _playerHolder;

    private void Awake()
    {
        DI.Inject(this);
    }

    private void Start()
    {
        if (_playerHolder != null)
        {
            _playerHolder.OnStatsChanged += OnStatsChanged;
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (_playerHolder != null)
        {
            _playerHolder.OnStatsChanged -= OnStatsChanged;
        }
    }

    private void OnStatsChanged()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_playerHolder == null) return;

        if (healthText != null)
        {
            healthText.text = $"HP: {_playerHolder.CurrentHP}/{_playerHolder.MaxHP}";
        }

        if (attackText != null)
        {
            attackText.text = $"Damage: {_playerHolder.GetModifiedAttackDamage()}";
        }

        if (goldText != null)
        {
            goldText.text = $"Gold: {_playerHolder.Gold}";
        }
    }
}