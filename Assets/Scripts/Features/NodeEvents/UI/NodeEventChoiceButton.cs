using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NodeEventChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;

    private Button _button;
    private NodeEventChoice _choice;
    private NodeEventPopup _popup;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void Initialize(NodeEventChoice choice, NodeEventPopup popup)
    {
        _choice = choice;
        _popup = popup;

        if (_buttonText != null)
        {
            _buttonText.text = choice.ButtonText;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (_popup != null && _choice != null)
        {
            _popup.OnChoiceSelected(_choice);
        }
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnClick);
        }
    }
}
