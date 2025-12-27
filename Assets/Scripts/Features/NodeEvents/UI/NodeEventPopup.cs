using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeEventPopup : MonoBehaviour
{
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private TextMeshProUGUI _eventName;
    [SerializeField] private TextMeshProUGUI _eventText;
    [SerializeField] private List<NodeEventChoiceButton> _choiceButtons = new List<NodeEventChoiceButton>();

    public event Action<NodeEventChoice> OnChoiceMade;

    private NodeEvent _currentEvent;

    private void Awake()
    {
        Hide();
    }

    public void Show(NodeEvent nodeEvent)
    {
        if (nodeEvent == null) return;

        _currentEvent = nodeEvent;
        if (_eventName != null)
        {
            _eventName.text = nodeEvent.EventName;
        }

        if (_eventText != null)
        {
            _eventText.text = nodeEvent.EventText;
        }

        UpdateChoiceButtons();

        _popupPanel.SetActive(true);
    }

    public void Hide()
    {
        _currentEvent = null;
        _popupPanel.SetActive(false);
    }

    public void OnChoiceSelected(NodeEventChoice choice)
    {
        OnChoiceMade?.Invoke(choice);
        Hide();
    }

    private void UpdateChoiceButtons()
    {
        if (_currentEvent == null) return;

        int buttonIndex = 0;

        for (int i = 0; i < _currentEvent.Choices.Count && buttonIndex < _choiceButtons.Count; i++)
        {
            var choice = _currentEvent.Choices[i];
            _choiceButtons[buttonIndex].Initialize(choice, this);
            buttonIndex++;
        }

        for (int i = buttonIndex; i < _choiceButtons.Count; i++)
        {
            _choiceButtons[i].Hide();
        }
    }
}