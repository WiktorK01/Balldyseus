using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVFXHandler : MonoBehaviour
{
    bool ballCanShove;
    bool bounceMode;

    public Animator ballBwompAnimator;

    void OnEnable(){
        BounceCountPublisher.BounceCountChange += OnBounceCountChange;
        BounceModePublisher.BounceModeChange += OnBounceModeChange;
    }
    void OnDisable(){
        BounceCountPublisher.BounceCountChange -= OnBounceCountChange;
        BounceModePublisher.BounceModeChange -= OnBounceModeChange;
    }
    void OnBounceCountChange(float newBounceValue){
        if(newBounceValue == 0) ballCanShove = false;
        else ballCanShove = true;
    }
    void OnBounceModeChange(bool newBounceModeSetting){
        bounceMode = newBounceModeSetting;
    }


    void OnCollisionEnter2D(Collision2D collision) {

        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")) && ballCanShove && bounceMode) 
        {
            ballBwompAnimator.Play("ballBwomp", -1, 0);
        }
    }
}
