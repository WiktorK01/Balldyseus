using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVFXHandler : MonoBehaviour
{
    BallCollision ballCollision;
    BallProperties ballProperties;
    BallFeedback ballFeedback;
    public Animator ballBwompAnimator;

    void Start(){
        ballFeedback = GetComponent<BallFeedback>();
        ballCollision = GetComponent<BallCollision>();
        ballProperties = GetComponent<BallProperties>();
    }

    void OnCollisionEnter2D(Collision2D collision) {

        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")) && BallCanShove()) 
        {
            ballFeedback.FloatingBounceNumber();
            ballBwompAnimator.Play("ballBwomp", -1, 0);
        }
    }

    bool BallCanShove(){
        if(ballProperties.ShoveMode && ballCollision.GetRemainingShoveCount() > 0f){
            return true;
        }
        else return false;
    }
}
