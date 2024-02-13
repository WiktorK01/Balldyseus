using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using AStar.Algolgorithms;
using AStar;

public class EnemyMovement : MonoBehaviour
{

    public PathfindingManager pathfindingManager;
    public EnemyProperties enemyProperties;

    public int moveMoney = 2; 
    public float moveDuration = 0.1f;

    bool hasMoved = false;
    bool hasCompletedAllMovements = false;

    public GameObject pathfindingErrorPrefab;
    
    public enum Direction { Up, Down, Left, Right }
    
    public void Move()
    {
        Debug.Log("Beginning Enemy Movement");
        SetStartingPointWalkable();
        var path = FindPathSync(); // Updated call here
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Failed to find path");
            return;
        }

        MoveToNextSpace(path);
        EndMovement();
    }

    void SetStartingPointWalkable()
    {
        Debug.Log("Beginning Setting Starting Point as Walkable");
        int startX = Mathf.FloorToInt(transform.position.x);
        int startY = Mathf.FloorToInt(transform.position.y);
        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        walkableMap[startY, startX] = true;
        Debug.Log("Finished Setting Starting Point as Walkable");
    }

    public void ResetMovement()
    {
        hasMoved = false;
    }

    public void EndMovement()
    {
        FindObjectOfType<TurnManager>().UpdateEnemiesList();
        hasMoved = true;
    }

    public bool HasMoved()
    {
        return hasMoved;
    }

    public void ResetAllMovements()
    {
        hasCompletedAllMovements = false;
    }

    public void CompletedAllMovements()
    {
        hasCompletedAllMovements = true;
    }

    void MoveInDirection(Vector3 direction)
    {
        StartCoroutine(MoveOverSeconds(direction));
    }

    IEnumerator MoveOverSeconds(Vector3 direction)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition; 
    }

    public void Shove(Direction direction)
    {
        Vector3 directionVector = DirectionToVector(direction);
        Vector3 newPosition = transform.position + directionVector;
        int newX = Mathf.FloorToInt(newPosition.x);
        int newY = Mathf.FloorToInt(newPosition.y);

        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        if (walkableMap[newY, newX] || IsObstacleTriggerUnwalkableAt(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            enemyProperties.TakeDamage(1f);
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

    (int, int)[] FindPathSync()
    {
        Debug.Log("Finding a Path");
        GameObject objective = FindObjective();
        if (objective == null)
        {
            Debug.LogError("No objective found");
            return null;
        }
        Debug.Log("Closest Objective Found");

        int startX = Mathf.FloorToInt(transform.position.x);
        int startY = Mathf.FloorToInt(transform.position.y);
        int goalX = Mathf.FloorToInt(objective.transform.position.x);
        int goalY = Mathf.FloorToInt(objective.transform.position.y);
        
        Debug.Log("Getting Walkable Map");
        bool[,] walkableMap = pathfindingManager.GetWalkableMap();

        Debug.Log("Generating Path");
        // Call the synchronous pathfinding method here
        return AStar.AStarPathfinding.GeneratePathSync(startX, startY, goalX, goalY, walkableMap, true);
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

    void MoveToNextSpace((int, int)[] path)
    {
        Debug.Log("Moving to the Next Space");
        if (path == null || path.Length <= 1)
        {
            Debug.LogError("Path is null or empty");
            return;
        }

        (int, int) nextPos = path[1];
        int deltaX = nextPos.Item1 - Mathf.FloorToInt(transform.position.x);
        int deltaY = nextPos.Item2 - Mathf.FloorToInt(transform.position.y);

        if (deltaX > 0) MoveInDirection(DirectionToVector(Direction.Right));
        else if (deltaX < 0) MoveInDirection(DirectionToVector(Direction.Left));
        else if (deltaY > 0) MoveInDirection(DirectionToVector(Direction.Up));
        else if (deltaY < 0) MoveInDirection(DirectionToVector(Direction.Down));
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
}


