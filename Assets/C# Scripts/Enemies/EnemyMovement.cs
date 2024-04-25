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
    [SerializeField] float secondsBetweenEnemyMoves = 0.6f; // this should be made equal to however long the move feedback is

    bool EnemyHasMoved = false;

    public enum Direction { Up, Down, Left, Right }

    
    void Awake()
    {
        GameObject groundTilemapObject = GameObject.FindWithTag("GroundTilemap");
        groundTilemap = groundTilemapObject.GetComponent<Tilemap>();

        enemyFeedback = GetComponent<EnemyFeedback>();
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        enemyProperties = GetComponent<EnemyProperties>();

        moveMoneyDecrement = moveMoney;
    }

//Move Related Code***************************************************************

    public void Move()
    {
        var path = FindPathSync();
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Failed to find path");
            return;
        }
        StartCoroutine(PerformMovements(path));
    }



    //Gets the starting point, the objective point
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
        (int, int)[] currentMap = AStar.AStarPathfinding.GeneratePathSync(startX, startY, goalX, goalY, walkableMap, true, false);
        return FillInDiagonals(currentMap);
    }

    (int, int)[] FillInDiagonals((int, int)[] path){
        if (path == null || path.Length == 0) return path;

        List<(int, int)> listPath = new List<(int, int)>(path); //convert the array to a list

        int previousX = listPath[0].Item1;
        int previousY = listPath[0].Item2;

        for(int i = 1; i < listPath.Count; i++){ 
            //go through every tuple
            //if both items in a tuple differ from the previous values, it must be a diagonal movement
            if(listPath[i].Item1 != previousX && listPath[i].Item2 != previousY){

                if(CanMove(listPath[i-1].Item1, listPath[i].Item2)) 
                    //if setting to the previous x is viable, insert a diagonal filler there
                    listPath.Insert(i, (listPath[i-1].Item1, listPath[i].Item2));
                else 
                    //else set it to the previous y
                    listPath.Insert(i, (listPath[i].Item1, listPath[i-1].Item2));
                i++;
            }
            previousX = listPath[i].Item1;
            previousY = listPath[i].Item2;
        }

        return listPath.ToArray();
    }

    //returns the closest Objective to the enemy
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

    IEnumerator PerformMovements((int, int)[] path)
    {
        if (path == null || path.Length == 0) {
        Debug.LogError("Path is null or empty in PerformMovements");
        yield break;
        }
        
        int previousX = path[0].Item1;
        int previousY = path[0].Item2;

        int minimum = Math.Min(moveMoney, path.Length);

        for(int i = 0; i < minimum; i++){
            if(TurnManager.Instance.currentState == TurnManager.GameState.Loss){
                break;
            }

            DecrementMoveMoney();

            if (path[i+1].Item1 > previousX)
                enemyFeedback.MoveRight();
            else if (path[i+1].Item1 < previousX)
                enemyFeedback.MoveLeft();
            else if (path[i+1].Item2 > previousY)
                enemyFeedback.MoveUp();
            else if (path[i+1].Item2 < previousY)
                enemyFeedback.MoveDown();

            previousX = path[i].Item1;
            previousY = path[i].Item2;
            yield return new WaitForSeconds(secondsBetweenEnemyMoves);
        }
        EnemyHasMoved = true;
    }


//Checking Spaces for x***************************************************************


    //checks if an enemy can move to a location
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

    //this checks for Obstacles that CANT WALK INTO but CAN SHOVE INTO
    bool IsObstacleTriggerUnwalkableAt(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            ObstacleTriggerUnwalkable obstacle = hit.collider.GetComponent<ObstacleTriggerUnwalkable>();
            return obstacle != null;
        }
        return false;
    }

    //checks if there is another enemy at a position (mainly for shoves)
    //returns it
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


//Shove related code***************************************************************


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


//Feedback related code***************************************************************


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

    /*void HandleMoveFeedback(Vector3 direction)
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
    }*/


//Misc***************************************************************

    public void ResetMovement(){
        EnemyHasMoved = false;
    }

    void DecrementMoveMoney(){
        moveMoneyDecrement--;
        enemyFeedback.MoveTextBounce();
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