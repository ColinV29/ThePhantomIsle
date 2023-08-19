using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyTemplate
{
    void GenerateEnemyStats(SOEnemyStats enemyStats);
    void TakeDamage(int incomingDamage);
    void Die();
    int SendDamage();
}
