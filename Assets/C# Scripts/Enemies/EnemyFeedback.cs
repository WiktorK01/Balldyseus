using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;

public class EnemyFeedback : MonoBehaviour
{
    bool isMoving = false;
    [SerializeField] GameObject myColliderObject;
    [SerializeField] TMP_Text moveMoneyText; 

    [SerializeField] MMF_Player healthTextBounce;
    [SerializeField] MMF_Player bigHealthTextBounce;
    [SerializeField] MMF_Player moveTextBounce;

    [SerializeField] MMF_Player shieldShake;
    [SerializeField] MMF_Player hoverTextGrowth;
    [SerializeField] MMF_Player hoverTextShrink;

    [SerializeField] MMF_Player moveDown;
    [SerializeField] MMF_Player moveUp;
    [SerializeField] MMF_Player moveLeft;
    [SerializeField] MMF_Player moveRight;

    [SerializeField] MMF_Player shoveDown;
    [SerializeField] MMF_Player shoveUp;
    [SerializeField] MMF_Player shoveLeft;
    [SerializeField] MMF_Player shoveRight;

    [SerializeField] MMF_Player squashDown;
    [SerializeField] MMF_Player squashUp;
    [SerializeField] MMF_Player squashLeft;
    [SerializeField] MMF_Player squashRight;

    [SerializeField] MMF_Player damageDown;
    [SerializeField] MMF_Player damageUp;
    [SerializeField] MMF_Player damageLeft;
    [SerializeField] MMF_Player damageRight;
    [SerializeField] MMF_Player damageFire;

    [SerializeField] MMF_Player deathFire;
    [SerializeField] MMF_Player deathFlush;

    [SerializeField] MMF_Player lowHealth;

    [SerializeField] MMF_Player onClick;

    [SerializeField] MMF_Player rotationResetter;

    public void HealthTextBounce(){
        healthTextBounce.Initialization();
        healthTextBounce.PlayFeedbacks();
    }

    public void BigHealthTextBounce(){
        bigHealthTextBounce.Initialization();
        bigHealthTextBounce.PlayFeedbacks();
    }

    public void MoveTextBounce(){
        moveTextBounce.Initialization();
        moveTextBounce.PlayFeedbacks();
    }

    public void ShieldShake(){
        shieldShake.Initialization();
        shieldShake.PlayFeedbacks();
    }
    public void ShieldShakeStop(){
        shieldShake.RestoreInitialValues();
        shieldShake.StopFeedbacks();
    }
    public void HoverTextGrowth(){
        if(!isMoving){
            hoverTextGrowth.Initialization();
            hoverTextGrowth.PlayFeedbacks();
        }
    }
    public void HoverTextShrink(){
        //hoverTextGrowth.RestoreInitialValues();
        hoverTextGrowth.StopFeedbacks();
        hoverTextShrink.Initialization();
        hoverTextShrink.PlayFeedbacks();
    }

    public void MoveDown(){
        moveDown.Initialization();
        moveDown.PlayFeedbacks();
    }

    public void MoveUp(){
        moveUp.Initialization();
        moveUp.PlayFeedbacks();
    }

    public void MoveLeft(){
        moveLeft.Initialization();
        moveLeft.PlayFeedbacks();
    }

    public void MoveRight(){
        moveRight.Initialization();
        moveRight.PlayFeedbacks();
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

    public void SquashDown(){
        squashDown.Initialization();
        squashDown.PlayFeedbacks();
    }

    public void SquashUp(){
        squashUp.Initialization();
        squashUp.PlayFeedbacks();
    }

    public void SquashLeft(){
        squashLeft.Initialization();
        squashLeft.PlayFeedbacks();
    }

    public void SquashRight(){
        squashRight.Initialization();
        squashRight.PlayFeedbacks();
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

    public void LowHealth(){
        lowHealth.Initialization();
        lowHealth.PlayFeedbacks();
    }
    public void StopLowHealth(){
        lowHealth.StopFeedbacks();
    }

    public void OnClick(){
        onClick.Initialization();
        onClick.PlayFeedbacks();
    }

    public void RotationResetter(){
        rotationResetter.Initialization();
        rotationResetter.PlayFeedbacks();
    }

//****************OBSERVERS***************

    void OnEnable(){
        MovementStatePublisher.MovementStateChange += OnMovementStateChange;
        EnemyHealthChangePublisher.EnemyHealthChange += OnEnemyHealthChange;
        BallCollisionPublisher.BallCollision += OnBallCollision;
        EnemyMoveMoneyPublisher.EnemyMoveMoneyChange += OnEnemyMoveMoneyChange;
    }

    void OnDisable(){
        MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
        EnemyHealthChangePublisher.EnemyHealthChange -= OnEnemyHealthChange;
        BallCollisionPublisher.BallCollision -= OnBallCollision;
        EnemyMoveMoneyPublisher.EnemyMoveMoneyChange -= OnEnemyMoveMoneyChange;
    }

    private void OnMovementStateChange(BallMovement.MovementState newState)
    {
        if(newState == BallMovement.MovementState.IsMoving) isMoving = true;
        else isMoving = false;
    }

    void OnEnemyHealthChange(GameObject enemyWhoLostDamage, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType){
        if(gameObject == enemyWhoLostDamage){
            if(amountLost == 1f) HealthTextBounce();
            else if(amountLost > 1f) BigHealthTextBounce();

            //Death Checks
            if(newHealthCount <= 0){
                if(damageType == EnemyProperties.DamageType.BallImpact 
                ||damageType == EnemyProperties.DamageType.BallImpactCritical
                ||damageType == EnemyProperties.DamageType.BallBounce) DeathFlush();
                
                else if(damageType == EnemyProperties.DamageType.FireDamage) DeathFire();
            }
            //Animations that are not Deaths
            else if(damageType == EnemyProperties.DamageType.FireDamage) DamageFire();


            if(newHealthCount == 1){
                LowHealth();
            }
            else{
                StopLowHealth(); //in case they can heal or something
            }
        }
    }
    void OnBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState){
        if(myColliderObject == collision.gameObject && !bounceMode){
            Vector2 enemyPosition = transform.position;

            Vector2 direction = ballPosition - enemyPosition;

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

    void OnEnemyMoveMoneyChange(GameObject enemyWhoLostMoveMoney, float newMoveMoneyCount){
        if(gameObject == enemyWhoLostMoveMoney){
            MoveTextBounce();
            moveMoneyText.text = newMoveMoneyCount.ToString();
        }
    }
}
