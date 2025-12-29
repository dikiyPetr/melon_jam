using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogPanel _dialogPanel;

    [SerializeField]  private NodeEventManager _nodeEventManager;
    [Inject] private PlayerHolder _playerHolder;

    public event Action<NodeEventChoice> OnChoiceMade;

    private void Awake()
    {
        DI.Inject(this);

        if (_dialogPanel != null)
        {
            _dialogPanel.Hide();
        }
    }

    public void ShowNodeEvent(NodeEvent nodeEvent)
    {
        if (nodeEvent == null || _dialogPanel == null || _playerHolder == null) return;

        _dialogPanel.ShowNodeEvent(nodeEvent, _playerHolder, OnChoiceSelected);
    }

    public void ShowDialog(string title, string description, List<DialogButtonData> buttons, Sprite icon = null)
    {
        if (_dialogPanel == null) return;

        _dialogPanel.Show(title, description, buttons, icon);
    }

    public void HideDialog()
    {
        if (_dialogPanel != null)
        {
            _dialogPanel.Hide();
        }
    }

    private void OnChoiceSelected(NodeEventChoice choice)
    {
        if (_nodeEventManager != null)
        {
            _nodeEventManager.ApplyEventChoice(choice);
        }

        OnChoiceMade?.Invoke(choice);
        HideDialog();
    }
}
