using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionPublisher : MonoBehaviour
{
    public delegate void OnBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState);
    public static event OnBallCollision BallCollision;

    public static void NotifyBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState)
    {
        BallCollision?.Invoke(collision, ballPosition, bounceMode, remainingBounceCount, currentSpeedState);
    }

    /*
    void OnEnable(){
        BallCollisionPublisher.BallCollision += OnBallCollision;
    }
    void OnDisable(){
        BallCollisionPublisher.BallCollision -= OnBallCollision;
    }

    void OnBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState){

    }
    */
}
