using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagePublisher : MonoBehaviour
{

    public delegate void OnEnemyDamage(GameObject enemyWhoGotHurt, Vector3 ballPosition, EnemyProperties.DamageType damageType);
    public static event OnEnemyDamage EnemyDamage;

    public static void NotifyEnemyDamage(GameObject enemyWhoGotHurt, Vector3 ballPosition, EnemyProperties.DamageType damageType)
    {
        EnemyDamage?.Invoke(enemyWhoGotHurt, ballPosition, damageType);
    }

    /*
    void OnEnable(){
        EnemyDamagePublisher.EnemyDamage += OnEnemyDamage;
    }
    void OnDisable(){
        EnemyDamagePublisher.EnemyDamage -= OnEnemyDamage;
    }

    void OnEnemyDamage(GameObject enemyWhoGotHurt, float damageAmount, EnemyProperties.DamageType damageType){

    }
    */
}
