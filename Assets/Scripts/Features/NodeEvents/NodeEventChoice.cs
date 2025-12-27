using System;
using System.Collections.Generic;

[Serializable]
public class NodeEventChoice
{
    public string ButtonText;
    public List<MotivationChange> MotivationChanges = new List<MotivationChange>();
}

[Serializable]
public class MotivationChange
{
    public MotivationType MotivationType;
    public float ChangeValue;
}

public enum MotivationType
{
    Aggression,
    Wealth,
    Adventure
}
