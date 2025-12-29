using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/DefeatIntent", fileName = "DefeatIntent")]
public class DefeatIntent : EventIntent
{
    public string Title = "Поражение!";
    public string Description = "Вы потерпели поражение в битве...";
    public Sprite Icon;
}
