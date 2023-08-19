using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] SOEnemyStats enemyStats;
    [Header("Base Stats")]
    public float might;
    public float agility;
    public float mind;
    public float fortitude;
    public float charisma;
    public int enemyLevel;
    [Header("Attributes")]
    public float meleeDamage;
    public float rangedDamage;
    public float physicalDefense;
    public float stamina;
    public float magicalDamage;
    public float magicalDefense;
    public float health;
    public float conditionResistance;
    public float socialSkill;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        GenerateStats(enemyStats);
    }

    void GenerateStats(SOEnemyStats enemyStats)
    {
        might = enemyStats.Might;
        agility = enemyStats.Agility;
        mind = enemyStats.Mind;
        fortitude = enemyStats.Fortitude;
        charisma = enemyStats.Charisma;
        enemyLevel = enemyStats.Level;

        meleeDamage = 6 + might;
        physicalDefense = 3 + might;
        rangedDamage = 6 + agility;
        stamina = 100 + (10 * agility);
        magicalDamage = 6 + mind;
        magicalDefense = 3 + mind;
        health = 20 + (fortitude * enemyLevel);
        conditionResistance = .01f * (3 + fortitude);
        socialSkill = 10 + charisma;
    }
}
