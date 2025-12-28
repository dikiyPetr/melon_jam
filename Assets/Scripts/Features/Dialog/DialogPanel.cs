using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPanel : MonoBehaviour
{
    [SerializeField] private GameObject _blockingPanel;
    [SerializeField] private GameObject _contentPanel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Transform _buttonsContainer;
    [SerializeField] private DialogButton _buttonPrefab;

    private List<DialogButton> _activeButtons = new List<DialogButton>();

    public void Show(string title, string description, List<DialogButtonData> buttons)
    {
        if (_titleText != null)
        {
            _titleText.text = title;
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = description;
        }

        ClearButtons();
        CreateButtons(buttons);

        if (_blockingPanel != null)
        {
            _blockingPanel.SetActive(true);
        }

        if (_contentPanel != null)
        {
            _contentPanel.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClearButtons();

        if (_blockingPanel != null)
        {
            _blockingPanel.SetActive(false);
        }

        if (_contentPanel != null)
        {
            _contentPanel.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private void CreateButtons(List<DialogButtonData> buttons)
    {
        if (_buttonPrefab == null || _buttonsContainer == null) return;

        foreach (var buttonData in buttons)
        {
            DialogButton button = Instantiate(_buttonPrefab, _buttonsContainer);
            button.Setup(buttonData.Text, buttonData.OnClick);
            _activeButtons.Add(button);
        }
    }

    private void ClearButtons()
    {
        foreach (var button in _activeButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        _activeButtons.Clear();
    }
}

public class DialogButtonData
{
    public string Text;
    public Action OnClick;

    public DialogButtonData(string text, Action onClick)
    {
        Text = text;
        OnClick = onClick;
    }
}
