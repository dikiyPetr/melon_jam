using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/VictoryIntent", fileName = "VictoryIntent")]
public class VictoryIntent : EventIntent
{
    public string Title = "Победа!";
    public string Description = "Вы одержали победу в битве!";
    public Sprite Icon;
}
