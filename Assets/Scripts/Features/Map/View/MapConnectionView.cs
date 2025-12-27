using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MapConnectionView : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private MapNodeData _fromNode;
    private MapNodeData _toNode;
    private bool _isVisited;

    public Color ActiveColor = Color.white;
    public Color InactiveColor = Color.gray;
    public Color VisitedColor = new Color(0.3f, 0.8f, 1f, 1f);
    public Color AIPathColor = new Color(1f, 0.8f, 0.2f, 1f);

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(MapNodeData fromNode, MapNodeData toNode, Vector3 fromPosition, Vector3 toPosition)
    {
        _fromNode = fromNode;
        _toNode = toNode;

        fromPosition.z = 1f;
        toPosition.z = 1f;

        _lineRenderer.SetPosition(0, fromPosition);
        _lineRenderer.SetPosition(1, toPosition);
    }

    public void UpdateVisuals(string currentNodeId, bool isOnAIPath = false, System.Collections.Generic.HashSet<string> validNextNodes = null)
    {
        if (_fromNode == null || _toNode == null || _lineRenderer == null) return;

        Color color;
        if (_isVisited)
        {
            color = VisitedColor;
        }
        else if (isOnAIPath)
        {
            color = AIPathColor;
        }
        else
        {
            bool isActive = !string.IsNullOrEmpty(currentNodeId) &&
                            _fromNode.Id == currentNodeId &&
                            _toNode.IsAvailable &&
                            (validNextNodes == null || validNextNodes.Contains(_toNode.Id));
            color = isActive ? ActiveColor : InactiveColor;
        }

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    public void MarkAsVisited()
    {
        _isVisited = true;
    }

    public bool IsConnectionBetween(string fromNodeId, string toNodeId)
    {
        return (_fromNode.Id == fromNodeId && _toNode.Id == toNodeId) ||
               (_fromNode.Id == toNodeId && _toNode.Id == fromNodeId);
    }

    public bool IsConnectionLeadingToGoal(System.Collections.Generic.HashSet<string> nodesLeadingToGoal)
    {
        if (nodesLeadingToGoal == null) return false;

        return nodesLeadingToGoal.Contains(_fromNode.Id) && nodesLeadingToGoal.Contains(_toNode.Id);
    }

    public LineRenderer GetLineRenderer()
    {
        return _lineRenderer;
    }
}