using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyFeedback : MonoBehaviour
{
    [SerializeField] MMF_Player healthTextBounce;
    [SerializeField] MMF_Player bigHealthTextBounce;
    [SerializeField] MMF_Player moveTextBounce;

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

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            ShoveLeft();
        }
    }

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
}
