using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DefenseStats",menuName = "Scriptable/Defense",order = 1)]
public class DefenseStats : ScriptableObject
{
    public Sprite icon;
    public Sprite image;
    public new string name;

    public float cost;
    public DefenseStats upgrade;

    public float damage;
    public float timeBetweenAttacks;
    public float fightRange;

    public float projectileSpeed;
    public Sprite projectileImage;

}
