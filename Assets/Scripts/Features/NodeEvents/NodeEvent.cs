using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeEvent
{
    public string EventName;
    public string EventText;
    public Sprite Icon;
    public List<NodeEventChoice> Choices = new List<NodeEventChoice>();
    public bool IsOneTime = true;
}
