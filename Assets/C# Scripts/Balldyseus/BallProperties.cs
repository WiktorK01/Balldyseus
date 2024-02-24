using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    private bool ShoveMode = false;
    private bool HighSpeed;

    private bool ShoveGagged = false;
    private bool AttackGagged = false;

    [Header("HighSpeed vars")]
    [SerializeField] private float highSpeedThreshold = 10f;
    [SerializeField] private float delayToTurnOffHighSpeed = 0.7f;


    void SetShoveMode(){
        if(ShoveGagged == true)
            
        ShoveMode = true;
    }

    void SetAttackMode(){
        ShoveMode = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
