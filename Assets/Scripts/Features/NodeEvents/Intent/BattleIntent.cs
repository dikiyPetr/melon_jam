using System;
using UnityEngine;


[CreateAssetMenu(menuName = "NodeEvents/BattleIntent", fileName = "BattleIntent")]
public class BattleIntent : EventIntent
{
    public int EnemyMaxHP = 30;
    public int EnemyAttack = 3;
}
