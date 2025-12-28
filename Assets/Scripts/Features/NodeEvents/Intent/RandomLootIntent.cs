using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NodeEvents/RandomLootIntent", fileName = "RandomLootIntent")]
public class RandomLootIntent : EventIntent
{
    public List<PlayerItem> PossibleItems = new List<PlayerItem>();

    public PlayerItem GetRandomItem()
    {
        if (PossibleItems.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, PossibleItems.Count);
        return PossibleItems[randomIndex];
    }
}