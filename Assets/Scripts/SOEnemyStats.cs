using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Create Enemy")]
public class SOEnemyStats : ScriptableObject
{
    public string EnemyName;
    public bool isBoss;
    public enum AttackType { ATTACK1, ATTACK2, ATTACK3 }
    public AttackType attackType;

    public float Might;
    public float Agility;
    public float Mind;
    public float Fortitude;
    public float Charisma;
    public int Level = 1;
    public float speed;
}
