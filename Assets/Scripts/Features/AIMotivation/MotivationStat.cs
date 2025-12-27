using System;
using UnityEngine;

[Serializable]
public class MotivationStat
{
    public string Name;
    public float CurrentValue;
    public float MaxValue;
    public MapNodeType LowValueNodeType;
    public MapNodeType HighValueNodeType;

    public MotivationStat(string name, float initialValue, float maxValue)
    {
        Name = name;
        CurrentValue = initialValue;
        MaxValue = maxValue;
        LowValueNodeType = MapNodeType.Rest;
        HighValueNodeType = MapNodeType.Combat;
    }

    public float Percentage => MaxValue > 0 ? Mathf.Clamp01(CurrentValue / MaxValue) : 0f;

    public MapNodeType GetDesiredNodeType()
    {
        return Percentage >= 0.5f ? HighValueNodeType : LowValueNodeType;
    }

    public void AddValue(float amount)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0f, MaxValue);
    }

    public void SetValue(float value)
    {
        CurrentValue = Mathf.Clamp(value, 0f, MaxValue);
    }
}
