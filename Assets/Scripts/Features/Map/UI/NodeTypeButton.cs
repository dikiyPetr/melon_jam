using UnityEngine;
using UnityEngine.UI;


public class NodeTypeButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;

    private MapNodeType _nodeType;
    private NodeTypeSelectionMenu _menu;


    public void Initialize(MapNodeType nodeType, Sprite icon, NodeTypeSelectionMenu menu)
    {
        _nodeType = nodeType;
        _menu = menu;

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
}