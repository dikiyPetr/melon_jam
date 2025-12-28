using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NodeTypePopupButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private MapNodeType _nodeType;
    private Button _button;
    private NodeTypeSelectionPopup _popup;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void Initialize(MapNodeType nodeType, Sprite icon, string description, NodeTypeSelectionPopup popup)
    {
        _nodeType = nodeType;
        _popup = popup;

        if (_iconImage != null)
        {
            _iconImage.sprite = icon;
        }

        if (_titleText != null)
        {
            _titleText.text = nodeType.ToString();
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = description;
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
