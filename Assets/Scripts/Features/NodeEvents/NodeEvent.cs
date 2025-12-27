using System;
using System.Collections.Generic;

[Serializable]
public class NodeEvent
{
    public string EventName;
    public string EventText;
    public List<NodeEventChoice> Choices = new List<NodeEventChoice>();
    public bool IsOneTime = true;
}
