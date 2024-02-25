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

        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")) && BallProperties.ShoveMode && BallCollision.GetRemainingShoveCount() > 0f) 
        {
            BallBwompAnimator.Play("ballBwomp", -1, 0);
        }
    }
}
