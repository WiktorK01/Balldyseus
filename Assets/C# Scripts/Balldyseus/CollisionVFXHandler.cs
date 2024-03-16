using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVFXHandler : MonoBehaviour
{
    BallCollision BallCollision;
    BallProperties BallProperties;
    public Animator BallBwompAnimator;

    void Start(){
        BallCollision = GetComponent<BallCollision>();
        BallProperties = GetComponent<BallProperties>();
    }

    void OnCollisionEnter2D(Collision2D collision) {

        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")) && BallCanShove()) 
        {
            BallBwompAnimator.Play("ballBwomp", -1, 0);
        }
    }

    bool BallCanShove(){
        if(BallProperties.ShoveMode && BallCollision.GetRemainingShoveCount() > 0f){
            return true;
        }
        else return false;
    }
}
