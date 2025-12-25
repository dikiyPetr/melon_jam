using System;
using UnityEngine;

public enum GameMode
{
    Playing,
    Paused,
}

[Serializable]
public class GameModeEntry
{
    public GameMode Mode;
    public GameObject[] gameObjects;
}