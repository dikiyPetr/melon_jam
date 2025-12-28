using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class DialogButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private Action _onClick;

    private void Awake()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }

    public void Setup(string text, Action onClick, bool isInteractable = true)
    {
        if (_buttonText != null)
        {
            _buttonText.text = text;
        }

        _onClick = onClick;

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClick);
            _button.interactable = isInteractable;
        }
    }

    private void OnButtonClick()
    {
        _onClick?.Invoke();
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
