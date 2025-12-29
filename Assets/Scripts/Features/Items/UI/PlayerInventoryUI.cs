using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private ItemIconView itemIconPrefab;

    [Inject] private PlayerHolder _playerHolder;

    private List<ItemIconView> _activeItemIcons = new List<ItemIconView>();

    private void Awake()
    {
        DI.Inject(this);
    }

    private void Start()
    {
        if (_playerHolder != null)
        {
            _playerHolder.OnStatsChanged += OnInventoryChanged;
            UpdateInventoryDisplay();
        }
    }

    private void OnDestroy()
    {
        if (_playerHolder != null)
        {
            _playerHolder.OnStatsChanged -= OnInventoryChanged;
        }
    }

    private void OnInventoryChanged()
    {
        UpdateInventoryDisplay();
    }

    private void UpdateInventoryDisplay()
    {
        if (_playerHolder == null || itemsContainer == null || itemIconPrefab == null) return;

        ClearItemIcons();

        foreach (var item in _playerHolder.Items)
        {
            ItemIconView iconView = Instantiate(itemIconPrefab, itemsContainer);
            iconView.Setup(item);
            _activeItemIcons.Add(iconView);
        }
    }

    private void ClearItemIcons()
    {
        foreach (var iconView in _activeItemIcons)
        {
            if (iconView != null)
            {
                Destroy(iconView.gameObject);
            }
        }

        _activeItemIcons.Clear();
    }
}
