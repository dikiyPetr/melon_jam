using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Visual Data", fileName = "MapNodeVisualData")]
public class MapNodeVisualData : ScriptableObject
{
    public List<NodeTypeSprite> NodeTypeSprites = new List<NodeTypeSprite>();

    public Sprite DefaultNodeSprite;
    public Sprite VisitedNodeSprite;
    public Color AvailableColor = Color.white;
    public Color UnavailableColor = Color.gray;
    public Color VisitedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

    public Sprite GetSpriteForNodeType(MapNodeType nodeType)
    {
        var typeSprite = NodeTypeSprites.Find(x => x.NodeType == nodeType);
        return typeSprite != null ? typeSprite.Sprite : DefaultNodeSprite;
    }
}

[Serializable]
public class NodeTypeSprite
{
    public MapNodeType NodeType;
    public Sprite Sprite;
}
