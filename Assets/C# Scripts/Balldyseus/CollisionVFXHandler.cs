using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVFXHandler : MonoBehaviour
{
    [SerializeField] private BallMovement BallMovement;
    [SerializeField] private BallCollision BallCollision;
    public Animator BallBwompAnimator;

    void OnCollisionEnter2D(Collision2D collision) {
        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")) && BallMovement.IsShoveMode() && BallCollision.GetRemainingShoveCount() > 0f) 
        {
            BallBwompAnimator.Play("ballBwomp", -1, 0);
        }
    }
}
