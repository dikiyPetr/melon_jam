using UnityEngine;

[CreateAssetMenu(menuName = "AIMotivation/AI Motivation Config")]
public class AIMotivationConfig : ScriptableObject
{
    public float MotivationDifferenceThreshold = 2f;

    public MotivationStatConfig Aggression = new MotivationStatConfig
    {
        Name = "Жажда боя",
        MaxValue = 100f,
        StartValueMin = 60f,
        StartValueMax = 80f,
        LowValueNodeType = MapNodeType.Rest,
        HighValueNodeType = MapNodeType.Combat
    };

    public MotivationStatConfig Wealth = new MotivationStatConfig
    {
        Name = "Финансы",
        MaxValue = 100f,
        StartValueMin = 60f,
        StartValueMax = 80f,
        LowValueNodeType = MapNodeType.Money,
        HighValueNodeType = MapNodeType.Shop
    };

    public MotivationStatConfig Adventure = new MotivationStatConfig
    {
        Name = "Азартность",
        MaxValue = 100f,
        StartValueMin = 60f,
        StartValueMax = 80f,
        LowValueNodeType = MapNodeType.Event,
        HighValueNodeType = MapNodeType.Treasure
    };
}