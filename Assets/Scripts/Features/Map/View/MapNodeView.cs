using System;
using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class MapNodeView : MonoBehaviour
{
    public MapNodeData NodeData { get; private set; }

    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private GameObject _visitedIndicator;

    private CircleCollider2D _collider;
    private MapNodeVisualData _visualData;
    private bool _isHovered;

    public event Action<MapNodeData> OnNodeClicked;

    // todo
    public event Action<MapNodeData> OnNodeHoverEnter;
    public event Action<MapNodeData> OnNodeHoverExit;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    public void Initialize(MapNodeData nodeData, MapNodeVisualData visualData)
    {
        NodeData = nodeData;
        _visualData = visualData;

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (NodeData == null || _visualData == null) return;


        _iconRenderer.sprite = _visualData.GetSpriteForNodeType(NodeData.NodeType);


        _visitedIndicator.SetActive(NodeData.IsVisited);

        _collider.enabled = !NodeData.IsVisited;
    }

    public void HandleClick()
    {
        if (NodeData.IsAvailable && !NodeData.IsVisited)
        {
            OnNodeClicked?.Invoke(NodeData);
        }
    }

    public void HandleMouseEnter()
    {
        if (!NodeData.IsVisited)
        {
            _isHovered = true;
            OnNodeHoverEnter?.Invoke(NodeData);
        }
    }

    public void HandleMouseExit()
    {
        if (_isHovered)
        {
            _isHovered = false;
            OnNodeHoverExit?.Invoke(NodeData);
        }
    }
}