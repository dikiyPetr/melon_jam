using UnityEngine;
using UnityEngine.UI;

public class ItemIconView : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void Setup(PlayerItem item)
    {
        if (item == null || iconImage == null) return;

        if (item.Icon != null)
        {
            iconImage.sprite = item.Icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}
