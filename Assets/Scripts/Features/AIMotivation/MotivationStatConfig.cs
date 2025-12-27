using System;
using UnityEngine;

[Serializable]
public class MotivationStatConfig
{
    public string Name;
    public float MaxValue = 100f;
    public float StartValueMin = 60f;
    public float StartValueMax = 80f;
    public MapNodeType LowValueNodeType = MapNodeType.Rest;
    public MapNodeType HighValueNodeType = MapNodeType.Combat;

    public float GetRandomStartValue()
    {
        return UnityEngine.Random.Range(StartValueMin, StartValueMax);
    }
}
