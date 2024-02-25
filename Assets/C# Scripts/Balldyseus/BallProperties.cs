using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    public bool ShoveMode = false;

    public bool ShoveGagged = false;
    public bool AttackGagged = false;

    [Header("HighSpeed vars")]
    public bool HighSpeed;
    public float highSpeedThreshold = 10f;
    public float delayToTurnOffHighSpeed = 0.7f;


    public void SetShoveMode(){
        if(ShoveGagged == true) ShoveMode = false;
        ShoveMode = true;
    }

    public void SetAttackMode(){
        if(AttackGagged == true) ShoveMode = true;
        ShoveMode = false;
    }

    public void EnableHighSpeed(){
        HighSpeed = true;
    }

    public void DisableHighSpeed(){
        HighSpeed = false;
    }

    public void GagShove(){
        ShoveGagged = true;
    }

    public void GagAttack(){
        AttackGagged = true;
    }

    public void UngagAll(){
        ShoveGagged = false;
        AttackGagged = false;
    }

}
