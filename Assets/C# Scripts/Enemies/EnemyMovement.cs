using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using AStar.Algolgorithms;
using AStar;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    PathfindingManager pathfindingManager;
    EnemyProperties enemyProperties;
    EnemyFeedback enemyFeedback;

    Tilemap groundTilemap;

    public int moveMoney = 2;
    public int moveMoneyDecrement; //this is a version of moveMoney that will decrement upon every movement.
    public float moveDuration = 0.1f;

    bool EnemyHasMoved = false;

    public enum Direction { Up, Down, Left, Right }

    public Color pathColor = Color.blue;

    
    void Awake()
    {
        GameObject groundTilemapObject = GameObject.FindWithTag("GroundTilemap");
        groundTilemap = groundTilemapObject.GetComponent<Tilemap>();

        enemyFeedback = GetComponent<EnemyFeedback>();
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        enemyProperties = GetComponent<EnemyProperties>();

        moveMoneyDecrement = moveMoney;
    }

    public void Move()
    {
        var path = FindPathSync();
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Failed to find path");
            return;
        }
        DecrementMoveMoney();
        MoveToNextSpace(path);
    }

    void DecrementMoveMoney(){
        moveMoneyDecrement--;
        enemyFeedback.MoveTextBounce();
    }

    //This shit is a mess atm but it works. just threw chatGPT at it as a temp fix until i feel like handling it 
    //can be greatly improved once i get a pathfinding algorithm that can detect movements via horizontal/vertical movements only.
    //or maybe without that even idk maybe i'm not thinking hard enough
    (int, int)[] FindPathSync()
    {
        GameObject objective = FindObjective();
        if (objective == null)
        {
            Debug.Log("No objective found");
            return null;
        }
        int startX = Mathf.FloorToInt(transform.position.x);
        int startY = Mathf.FloorToInt(transform.position.y);
        int goalX = Mathf.FloorToInt(objective.transform.position.x);
        int goalY = Mathf.FloorToInt(objective.transform.position.y);
        
        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        if (walkableMap == null)
        {
            Debug.LogError("Walkable map data is null.");
            return null;
        }
        return AStar.AStarPathfinding.GeneratePathSync(startX, startY, goalX, goalY, walkableMap, true);
    }

    void MoveToNextSpace((int, int)[] path)
    {
        Debug.Log("Moving to the Next Space");
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = groundTilemap.CellToWorld(new Vector3Int(path[i].Item1, path[i].Item2, 0)) + new Vector3(0.5f, 0.5f, 0);
            Vector3 end = groundTilemap.CellToWorld(new Vector3Int(path[i + 1].Item1, path[i + 1].Item2, 0)) + new Vector3(0.5f, 0.5f, 0);
            Debug.DrawLine(start, end, pathColor, 10.0f, false);
        }

        if (path.Length < 2)
        {
            Debug.LogError("Path is too short to move.");
            return;
        }

        (int, int) nextPos = path[1];
        int deltaX = nextPos.Item1 - Mathf.FloorToInt(transform.position.x);
        int deltaY = nextPos.Item2 - Mathf.FloorToInt(transform.position.y);

        // Check if horizontal movement is possible
        if (deltaX != 0)
        {
            Vector3 horizontalTarget = transform.position + new Vector3(deltaX, 0, 0);
            if (CanMove(Mathf.FloorToInt(horizontalTarget.x), Mathf.FloorToInt(transform.position.y)))
            {
                HandleMoveFeedback(DirectionToVector(deltaX > 0 ? Direction.Right : Direction.Left));
                EndMovement();
                return;
            }
        }

        // Check if vertical movement is needed if horizontal is blocked
        if (deltaY != 0)
        {
            Vector3 verticalTarget = transform.position + new Vector3(0, deltaY, 0);
            if (CanMove(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(verticalTarget.y)))
            {
                HandleMoveFeedback(DirectionToVector(deltaY > 0 ? Direction.Up : Direction.Down));
                EndMovement();
                return;
            }
        }
        Debug.LogError("Both horizontal and vertical moves are blocked or unnecessary.");
    }

    bool CanMove(int x, int y)
    {
        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        if (walkableMap == null || y >= walkableMap.GetLength(0) || x >= walkableMap.GetLength(1) || y < 0 || x < 0)
        {
            Debug.LogError("Invalid walkable map data or out of bounds access attempted.");
            return false;
        }
        return walkableMap[y, x] && !IsObstacleTriggerUnwalkableAt(new Vector3(x, y, 0));
    }

    private Vector3 DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return new Vector3(0, 1, 0);
            case Direction.Down: return new Vector3(0, -1, 0);
            case Direction.Left: return new Vector3(-1, 0, 0);
            case Direction.Right: return new Vector3(1, 0, 0);
            default: return Vector3.zero;
        }
    }

    void HandleMoveFeedback(Vector3 direction)
    {
        if (direction == Vector3.up)
        {
            enemyFeedback.MoveUp();
        }
        else if (direction == Vector3.down)
        {
            enemyFeedback.MoveDown();
        }
        else if (direction == Vector3.left)
        {
            enemyFeedback.MoveLeft();
        }
        else if (direction == Vector3.right)
        {
            enemyFeedback.MoveRight();
        }
    }


    private bool IsObstacleTriggerUnwalkableAt(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            ObstacleTriggerUnwalkable obstacle = hit.collider.GetComponent<ObstacleTriggerUnwalkable>();
            return obstacle != null;
        }
        return false;
    }

    public GameObject CheckForEnemyAtPosition(Vector3 position)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = 1 << enemyLayer;

        Collider2D collider = Physics2D.OverlapPoint(position, layerMask);

        if (collider != null && collider.gameObject.CompareTag("Enemy"))
        {
            return collider.gameObject;
        }

        return null;
    }

    /*void SetStartingPointWalkable()
    {
        Debug.Log("Setting Starting Point as Walkable");
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position);
        pathfindingManager.SetTileWalkability(cellPosition, true); 
    }*/


    public void Shove(Direction direction)
    {
        Vector3 directionVector = DirectionToVector(direction);
        Vector3 newPosition = transform.position + directionVector;
        int newX = Mathf.FloorToInt(newPosition.x);
        int newY = Mathf.FloorToInt(newPosition.y);

        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        
        //if it's walkable or there's an unwalkable triggger obstacle, allow the shove
        if (walkableMap[newY, newX] || IsObstacleTriggerUnwalkableAt(newPosition)){
            HandleShoveFeedback(direction);
        }

        //otherwise, if shoved into an enemy
        else
        {
            if (CheckForEnemyAtPosition(newPosition) != null){
                EnemyProperties otherEnemyProperties = CheckForEnemyAtPosition(newPosition).GetComponent<EnemyProperties>();
                otherEnemyProperties.TakeDamage(1f);
            }

            HandleSquashFeedback(direction);
            enemyProperties.TakeDamage(1f);
        }
    }

    void HandleSquashFeedback(Direction direction){
        if (direction == Direction.Up)
            enemyFeedback.SquashUp();

        else if (direction == Direction.Down)
            enemyFeedback.SquashDown();

        else if (direction == Direction.Left)
            enemyFeedback.SquashLeft();

        else if (direction == Direction.Right)
            enemyFeedback.SquashRight();
    }

    void HandleShoveFeedback(Direction direction) {
        if (direction == Direction.Up)
            enemyFeedback.ShoveUp();

        else if (direction == Direction.Down)
            enemyFeedback.ShoveDown();

        else if (direction == Direction.Left)
            enemyFeedback.ShoveLeft();

        else if (direction == Direction.Right)
            enemyFeedback.ShoveRight();
    }

    GameObject FindObjective()
    {
        GameObject[] objectives = GameObject.FindGameObjectsWithTag("Objective");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in objectives)
        {
            float distance = Vector3.Distance(obj.transform.position, currentPosition);
            if (distance < minDistance)
            {
                closest = obj;
                minDistance = distance;
            }
        }
        return closest;
    }

    public void ResetMovement()
    {
        EnemyHasMoved = false;
    }

    void EndMovement(){
        EnemyHasMoved = true;
    }

    public void ResetMoveMoney(){
        enemyFeedback.MoveTextBounce();
        moveMoneyDecrement = moveMoney;
    }

    public bool HasMoved()
    {
        return EnemyHasMoved;
    }
}