using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeTypeSelectionMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _menuPanel;
    [SerializeField] private Vector2 _cursorOffset = new Vector2(10f, 0f);
    [SerializeField] private List<NodeTypeButton> _nodeTypeButtons = new List<NodeTypeButton>();

    public event Action<MapNodeType> OnNodeTypeSelected;

    private MapNodeData _currentNode;
    private bool _isVisible;
    private bool _isSelectionMode;
    private Camera _camera;
    private MapNodeVisualData _visualData;

    private void Awake()
    {
        _camera = Camera.main;
        Hide();
    }

    public void Initialize(MapNodeVisualData visualData)
    {
        _visualData = visualData;
    }

    private void Update()
    {
        if (_isVisible)
        {
            UpdatePosition();
        }
    }

    public void ShowPreview(MapNodeData node)
    {
        if (node == null || node.AvailableNodeTypes == null || node.AvailableNodeTypes.Count == 0)
        {
            Hide();
            return;
        }

        _currentNode = node;
        _isVisible = true;
        _isSelectionMode = false;
        gameObject.SetActive(true);
        UpdateButtons();
        UpdatePosition();
    }

    public void Hide()
    {
        _isVisible = false;
        _isSelectionMode = false;
        _currentNode = null;
        gameObject.SetActive(false);
    }

    public void SelectNodeType(MapNodeType nodeType)
    {
        if (_currentNode == null || !_isSelectionMode) return;

        _currentNode.SetNodeType(nodeType);
        OnNodeTypeSelected?.Invoke(nodeType);
        Hide();
    }

    public List<MapNodeType> GetAvailableTypes()
    {
        return _currentNode?.AvailableNodeTypes ?? new List<MapNodeType>();
    }

    public bool IsSelectionMode => _isSelectionMode;

    private void UpdateButtons()
    {
        if (_currentNode == null || _visualData == null) return;

        var availableTypes = _currentNode.AvailableNodeTypes;
        int buttonIndex = 0;

        for (int i = 0; i < availableTypes.Count && buttonIndex < _nodeTypeButtons.Count; i++)
        {
            var nodeType = availableTypes[i];
            var sprite = _visualData.GetSpriteForNodeType(nodeType);
            _nodeTypeButtons[buttonIndex].Initialize(nodeType, sprite, this);
            buttonIndex++;
        }

        for (int i = buttonIndex; i < _nodeTypeButtons.Count; i++)
        {
            _nodeTypeButtons[i].Hide();
        }
    }

    private void UpdatePosition()
    {
        if (_camera == null || Mouse.current == null || _menuPanel == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 menuPosition = mousePosition + _cursorOffset;

        _menuPanel.position = menuPosition;
    }
}