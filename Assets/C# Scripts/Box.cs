using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class Box : MonoBehaviour
{
    EnemyProperties enemyProperties;

    [SerializeField] GameObject myColliderObject;

    [SerializeField] MMF_Player shieldShake;

    [SerializeField] MMF_Player shoveDown;
    [SerializeField] MMF_Player shoveUp;
    [SerializeField] MMF_Player shoveLeft;
    [SerializeField] MMF_Player shoveRight;

    [SerializeField] MMF_Player damageDown;
    [SerializeField] MMF_Player damageUp;
    [SerializeField] MMF_Player damageLeft;
    [SerializeField] MMF_Player damageRight;
    [SerializeField] MMF_Player damageFire;

    [SerializeField] MMF_Player deathFire;
    [SerializeField] MMF_Player deathFlush;

    [SerializeField] MMF_Player onClick;

    [SerializeField] MMF_Player rotationResetter;


    public void ShieldShake(){
        shieldShake.Initialization();
        shieldShake.PlayFeedbacks();
    }
    public void ShieldShakeStop(){
        shieldShake.RestoreInitialValues();
        shieldShake.StopFeedbacks();
    }

    public void ShoveDown(){
        shoveDown.Initialization();
        shoveDown.PlayFeedbacks();
    }

    public void ShoveUp(){
        shoveUp.Initialization();
        shoveUp.PlayFeedbacks();
    }

    public void ShoveLeft(){
        shoveLeft.Initialization();
        shoveLeft.PlayFeedbacks();
    }

    public void ShoveRight(){
        shoveRight.Initialization();
        shoveRight.PlayFeedbacks();
    }
    public void DamageDown(){
        damageDown.Initialization();
        damageDown.PlayFeedbacks();
    }
    public void DamageUp(){
        damageUp.Initialization();
        damageUp.PlayFeedbacks();
    }
    public void DamageLeft(){
        damageLeft.Initialization();
        damageLeft.PlayFeedbacks();
    }
    public void DamageRight(){
        damageRight.Initialization();
        damageRight.PlayFeedbacks();
    }
    public void DamageFire(){
        damageFire.Initialization();
        damageFire.PlayFeedbacks();
    }

    public void DeathFire(){
        deathFire.Initialization();
        deathFire.PlayFeedbacks();
    }

    public void DeathFlush(){
        deathFlush.Initialization();
        deathFlush.PlayFeedbacks();
    }

    public void OnClick(){
        onClick.Initialization();
        onClick.PlayFeedbacks();
    }

    public void RotationResetter(){
        rotationResetter.Initialization();
        rotationResetter.PlayFeedbacks();
    }

    
    void OnEnable(){
        enemyProperties = GetComponent<EnemyProperties>();

        BallCollisionPublisher.BallCollision += OnBallCollision;
        EnemyShovePublisher.EnemyShove += OnEnemyShove;
        EnemyHealthChangePublisher.EnemyHealthChange += OnEnemyHealthChange;
    }

    void OnDisable(){
        BallCollisionPublisher.BallCollision -= OnBallCollision;
        EnemyHealthChangePublisher.EnemyHealthChange -= OnEnemyHealthChange;
        EnemyShovePublisher.EnemyShove -= OnEnemyShove;
    }


    void OnBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState){
        if(myColliderObject == collision.gameObject && !bounceMode){
            Vector2 enemyPosition = transform.position;
            Vector2 direction = ballPosition - enemyPosition;

            if(currentSpeedState == BallProperties.SpeedState.High)
                EnemyDamagePublisher.NotifyEnemyDamage(gameObject, ballPosition, EnemyProperties.DamageType.BallImpactCritical);
            else 
                EnemyDamagePublisher.NotifyEnemyDamage(gameObject, ballPosition, EnemyProperties.DamageType.BallImpact);

            if(currentSpeedState == BallProperties.SpeedState.Low) return;

            if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
                if(direction.x > 0) 
                    DamageRight();
                else 
                    DamageLeft();
                
            }
            else{
                if(direction.y > 0) 
                    DamageUp();
                else 
                    DamageDown();
            }
        }
    }

    void OnEnemyShove(GameObject enemyWhoGotShoved, EnemyMovement.Direction direction, bool canShoveHere){
        if(myColliderObject == enemyWhoGotShoved && canShoveHere){
            Debug.Log("I GOT SHOVED!");
            if(direction == EnemyMovement.Direction.Up) ShoveUp();
            else if(direction == EnemyMovement.Direction.Down) ShoveDown();
            else if(direction == EnemyMovement.Direction.Left) ShoveLeft();
            else if(direction == EnemyMovement.Direction.Right) ShoveRight();
        }
    }

    void OnEnemyHealthChange(GameObject enemyWhoGotHurt, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType){
        if(gameObject == enemyWhoGotHurt){
            if(enemyProperties.health <= 0 && damageType == EnemyProperties.DamageType.FireDamage)DeathFire();
            else if(enemyProperties.health <= 0)DeathFlush();
            else if(damageType == EnemyProperties.DamageType.FireDamage)DamageFire();
        }
    }

}
