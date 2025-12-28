using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeTypeSelectionPopup : MonoBehaviour
{
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private List<NodeTypePopupButton> _nodeTypeButtons = new List<NodeTypePopupButton>();

    public event Action<MapNodeType> OnNodeTypeSelected;

    private MapNodeData _currentNode;
    private MapNodeVisualData _visualData;

    private void Awake()
    {
        Hide();
    }

    public void Initialize(MapNodeVisualData visualData)
    {
        _visualData = visualData;
    }

    public void Show(MapNodeData node)
    {
        if (node == null || node.AvailableNodeTypes == null || node.AvailableNodeTypes.Count == 0)
        {
            Hide();
            return;
        }

        if (node.HasSelectedType)
        {
            Hide();
            return;
        }

        _currentNode = node;
        UpdateButtons();
        _popupPanel.SetActive(true);
    }

    public void Hide()
    {
        _currentNode = null;
        _popupPanel.SetActive(false);
    }

    public void SelectNodeType(MapNodeType nodeType)
    {
        if (_currentNode == null) return;

        _currentNode.SetNodeType(nodeType);
        OnNodeTypeSelected?.Invoke(nodeType);
        Hide();
    }

    private void UpdateButtons()
    {
        if (_currentNode == null || _visualData == null) return;

        var availableTypes = _currentNode.AvailableNodeTypes;
        int buttonIndex = 0;

        for (int i = 0; i < availableTypes.Count && buttonIndex < _nodeTypeButtons.Count; i++)
        {
            var nodeType = availableTypes[i];
            var sprite = _visualData.GetSpriteForNodeType(nodeType);
            var description = _visualData.GetDescriptionForNodeType(nodeType);
            _nodeTypeButtons[buttonIndex].Initialize(nodeType, sprite, description, this);
            buttonIndex++;
        }

        for (int i = buttonIndex; i < _nodeTypeButtons.Count; i++)
        {
            _nodeTypeButtons[i].Hide();
        }
    }
}
