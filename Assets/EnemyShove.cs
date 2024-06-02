using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AStar;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class EnemyShove : MonoBehaviour
{
    [SerializeField] GameObject myColliderObject;
    private PathfindingManager pathfindingManager;
    private Tilemap groundTilemap;

    public void Shove(EnemyMovement.Direction direction, Vector3 ballPosition)
    {
        Vector3 directionVector = DirectionToVector(direction);
        Vector3 newPosition = transform.position + directionVector;
        int newX = Mathf.FloorToInt(newPosition.x);
        int newY = Mathf.FloorToInt(newPosition.y);

        bool canShoveHere = false;

        if(pathfindingManager.CheckIfTileIsWalkable(newX, newY)) canShoveHere = true;

        if (canShoveHere)
        {
            EnemyShovePublisher.NotifyEnemyShove(gameObject, direction, true);
        }
        else
        {
            EnemyShovePublisher.NotifyEnemyShove(gameObject, direction, false);

            //handle damage
            GameObject enemyAtPosition = pathfindingManager.CheckIfEnemyAtPosition(newPosition);
            if (enemyAtPosition != null)
            {
                EnemyDamagePublisher.NotifyEnemyDamage(enemyAtPosition, Vector3.zero, EnemyProperties.DamageType.GotShovedInto);
            }

            EnemyDamagePublisher.NotifyEnemyDamage(gameObject, ballPosition, EnemyProperties.DamageType.BallBounce);
        }
    }

    private Vector3 DirectionToVector(EnemyMovement.Direction direction)
    {
        switch (direction)
        {
            case EnemyMovement.Direction.Up:
                return new Vector3(0, 1, 0);
            case EnemyMovement.Direction.Down:
                return new Vector3(0, -1, 0);
            case EnemyMovement.Direction.Left:
                return new Vector3(-1, 0, 0);
            case EnemyMovement.Direction.Right:
                return new Vector3(1, 0, 0);
            default:
                return Vector3.zero;
        }
    }


    void OnEnable(){
        groundTilemap = GameObject.FindWithTag("GroundTilemap")?.GetComponent<Tilemap>();
        if (groundTilemap == null)Debug.LogError("GroundTilemap not found or Tilemap component missing.");

        pathfindingManager = FindObjectOfType<PathfindingManager>();
        if (pathfindingManager == null)Debug.LogError("PathfindingManager not found.");
    
        BallCollisionPublisher.BallCollision += OnBallCollision;
    }

    void OnDisable(){
        BallCollisionPublisher.BallCollision -= OnBallCollision;
    }

    void OnBallCollision(Collision2D collision, Vector2 ballPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState){
        if(myColliderObject == collision.gameObject && bounceMode && remainingBounceCount > 0){
            Vector2 enemyPosition = transform.position;
            Vector2 direction = ballPosition - enemyPosition;

            if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
                if(direction.x < 0) 
                    Shove(EnemyMovement.Direction.Right, ballPosition);
                else 
                    Shove(EnemyMovement.Direction.Left, ballPosition);
                
            }
            else{
                if(direction.y < 0) 
                    Shove(EnemyMovement.Direction.Up, ballPosition);
                else 
                    Shove(EnemyMovement.Direction.Down, ballPosition);
            }
            
        }
    }
}
