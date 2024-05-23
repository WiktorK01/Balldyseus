using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthChangePublisher : MonoBehaviour
{
    public delegate void OnEnemyHealthChange(GameObject enemyWhoLostDamage, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType);
    public static event OnEnemyHealthChange EnemyHealthChange;

    public static void NotifyEnemyHealthChange(GameObject enemyWhoLostDamage, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType)
    {
        EnemyHealthChange?.Invoke(enemyWhoLostDamage, newHealthCount, amountLost, damageType);
    }

    /*
    void OnEnable(){
        EnemyHealthChangePublisher.EnemyHealthChange += OnEnemyHealthChange;
    }
    void OnDisable(){
        EnemyHealthChangePublisher.EnemyHealthChange -= OnEnemyHealthChange;
    }

    void OnEnemyHealthChange(GameObject enemyWhoLostDamage, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType){

    }
    */
}
