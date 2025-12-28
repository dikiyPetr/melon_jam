using System;
using UnityEngine;

[Serializable]
public abstract class EventCondition : ScriptableObject
{
    public abstract bool Check(PlayerHolder playerHolder);
}
