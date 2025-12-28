using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogPanel _dialogPanel;

    [Inject] private NodeEventManager _nodeEventManager;

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
        if (nodeEvent == null || _dialogPanel == null) return;

        string title = nodeEvent.EventName;
        string description = nodeEvent.EventText;
        List<DialogButtonData> buttons = new List<DialogButtonData>();

        foreach (var choice in nodeEvent.Choices)
        {
            var buttonData = new DialogButtonData(
                choice.ButtonText,
                () => OnChoiceSelected(choice)
            );
            buttons.Add(buttonData);
        }

        _dialogPanel.Show(title, description, buttons);
    }

    public void ShowDialog(string title, string description, List<DialogButtonData> buttons)
    {
        if (_dialogPanel == null) return;

        _dialogPanel.Show(title, description, buttons);
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

        HideDialog();
    }
}
