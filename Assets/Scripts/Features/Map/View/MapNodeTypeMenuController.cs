using System;
using UnityEngine;

public class MapNodeTypeMenuController : MonoBehaviour
{
    [SerializeField] private NodeTypeSelectionMenu _nodeTypeMenu;
    [SerializeField] private NodeTypeSelectionPopup _nodeTypePopup;

    private AIMotivationPathController _aiPathController;

    public event Action OnNodeTypeSelected;

    public void Initialize(AIMotivationPathController aiPathController, MapNodeVisualData visualData)
    {
        _aiPathController = aiPathController;

        if (_nodeTypeMenu != null)
        {
            _nodeTypeMenu.Initialize(visualData);
            _nodeTypeMenu.OnNodeTypeSelected += HandleNodeTypeSelected;
        }

        if (_nodeTypePopup != null)
        {
            _nodeTypePopup.Initialize(visualData);
            _nodeTypePopup.OnNodeTypeSelected += HandleNodeTypeSelected;
        }
    }

    public void SubscribeToNodeEvents(MapNodeView nodeView)
    {
        if (nodeView != null)
        {
            nodeView.OnNodeClicked += HandleNodeClicked;
            nodeView.OnNodeHoverEnter += HandleNodeHoverEnter;
            nodeView.OnNodeHoverExit += HandleNodeHoverExit;
        }
    }

    public void UnsubscribeFromNodeEvents(MapNodeView nodeView)
    {
        if (nodeView != null)
        {
            nodeView.OnNodeClicked -= HandleNodeClicked;
            nodeView.OnNodeHoverEnter -= HandleNodeHoverEnter;
            nodeView.OnNodeHoverExit -= HandleNodeHoverExit;
        }
    }

    private void HandleNodeClicked(MapNodeData nodeData)
    {
        if (_aiPathController != null && _nodeTypePopup != null)
        {
            var validNextNodes = _aiPathController.ValidNextNodes;
            if (validNextNodes != null && validNextNodes.Contains(nodeData.Id) && !nodeData.HasSelectedType)
            {
                _nodeTypePopup.Show(nodeData);
            }
        }
    }

    private void HandleNodeHoverEnter(MapNodeData nodeData)
    {
        if (_nodeTypeMenu != null && !nodeData.HasSelectedType && nodeData.AvailableNodeTypes != null && nodeData.AvailableNodeTypes.Count > 0)
        {
            _nodeTypeMenu.ShowPreview(nodeData);
        }
    }

    private void HandleNodeHoverExit(MapNodeData nodeData)
    {
        if (_nodeTypeMenu != null && !_nodeTypeMenu.IsSelectionMode)
        {
            _nodeTypeMenu.Hide();
        }
    }

    private void HandleNodeTypeSelected(MapNodeType nodeType)
    {
        OnNodeTypeSelected?.Invoke();
    }

    private void OnDestroy()
    {
        if (_nodeTypeMenu != null)
        {
            _nodeTypeMenu.OnNodeTypeSelected -= HandleNodeTypeSelected;
        }

        if (_nodeTypePopup != null)
        {
            _nodeTypePopup.OnNodeTypeSelected -= HandleNodeTypeSelected;
        }
    }
}
