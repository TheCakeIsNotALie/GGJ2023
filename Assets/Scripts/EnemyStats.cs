using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStat",menuName ="Scriptable/Enemies",order = 0)]
public class EnemyStats : ScriptableObject
{
    public GameState spawnTime = GameState.FullGrown;
    public new string name = "Debile";
    public float damage = 2f;
    public float speed = 1f;
    public Sprite image;
    public float pv = 20;
    public float timeBetweenAttacks = 2f;
    public float attackDistance = 2f;
}
