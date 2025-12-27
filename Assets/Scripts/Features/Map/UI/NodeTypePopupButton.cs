using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NodeTypePopupButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;

    private MapNodeType _nodeType;
    private Button _button;
    private NodeTypeSelectionPopup _popup;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void Initialize(MapNodeType nodeType, Sprite icon, NodeTypeSelectionPopup popup)
    {
        _nodeType = nodeType;
        _popup = popup;

        if (_iconImage != null)
        {
            _iconImage.sprite = icon;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (_popup != null)
        {
            _popup.SelectNodeType(_nodeType);
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
