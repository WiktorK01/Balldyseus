using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AStar;

public class EnemyMovement : MonoBehaviour
{

    private PathfindingManager pathfindingManager;
    private EnemyProperties enemyProperties;
    private EnemyFeedback enemyFeedback;
    private Tilemap groundTilemap;

    public int moveMoney = 2;
    public int moveMoneyDecrement;
    public float moveDuration = 0.1f;
    [SerializeField] private float secondsBetweenEnemyMoves = 0.6f;

    private bool enemyHasMoved = false;

    public enum Direction { Up, Down, Left, Right }

    private void OnEnable()
    {
        InitializeDependencies();

        GameStatePublisher.GameStateChange += OnGameStateChange;
    }

    private void InitializeDependencies()
    {
        groundTilemap = GameObject.FindWithTag("GroundTilemap")?.GetComponent<Tilemap>();
        if (groundTilemap == null)
        {
            Debug.LogError("GroundTilemap not found or Tilemap component missing.");
        }

        enemyFeedback = GetComponent<EnemyFeedback>();
        enemyProperties = GetComponent<EnemyProperties>();

        pathfindingManager = FindObjectOfType<PathfindingManager>();
        if (pathfindingManager == null)
        {
            Debug.LogError("PathfindingManager not found.");
        }

        moveMoneyDecrement = moveMoney;
    }

    // Move Related Code ***************************************************************

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

    private (int, int)[] FindPathSync()
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

        var path = AStar.AStarPathfinding.GeneratePathSync(startX, startY, goalX, goalY, walkableMap, true, false);
        return FillInDiagonals(path);
    }

    private (int, int)[] FillInDiagonals((int, int)[] path)
    {
        if (path == null || path.Length == 0) return path;

        var listPath = new List<(int, int)>(path);

        int previousX = listPath[0].Item1;
        int previousY = listPath[0].Item2;

        for (int i = 1; i < listPath.Count; i++)
        {
            if (listPath[i].Item1 != previousX && listPath[i].Item2 != previousY)
            {
                if (CanMove(listPath[i - 1].Item1, listPath[i].Item2))
                    listPath.Insert(i, (listPath[i - 1].Item1, listPath[i].Item2));
                else
                    listPath.Insert(i, (listPath[i].Item1, listPath[i - 1].Item2));
                i++;
            }
            previousX = listPath[i].Item1;
            previousY = listPath[i].Item2;
        }

        return listPath.ToArray();
    }

    private GameObject FindObjective()
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

private IEnumerator PerformMovements((int, int)[] path)
{
    if (path == null || path.Length < 2)
    {
        Debug.LogError("Path is null or too short in PerformMovements()");
        yield break;
    }

    for (int i = 0; i < moveMoney && i < path.Length - 1; i++)
    {
        if (TurnManager.Instance.currentState == TurnManager.GameState.Loss)
        {
            break;
        }

        int previousX = Mathf.FloorToInt(transform.position.x);
        int previousY = Mathf.FloorToInt(transform.position.y);
        int targetX = path[i + 1].Item1;
        int targetY = path[i + 1].Item2;
        Vector2 targetPosition = new Vector2(targetX, targetY);

        if (targetX > previousX)
            enemyFeedback.MoveRight();
        else if (targetX < previousX)
            enemyFeedback.MoveLeft();
        else if (targetY > previousY)
            enemyFeedback.MoveUp();
        else if (targetY < previousY)
            enemyFeedback.MoveDown();

        yield return new WaitForSeconds(secondsBetweenEnemyMoves);

        while ((Vector2)transform.position != targetPosition)
        {
            yield return null; 
        }

        transform.position = targetPosition;
    }

    enemyHasMoved = true;
}

    // Checking Spaces ***************************************************************

    private bool CanMove(int x, int y)
    {
        bool[,] walkableMap = pathfindingManager.GetWalkableMap();
        if (walkableMap == null || y >= walkableMap.GetLength(0) || x >= walkableMap.GetLength(1) || y < 0 || x < 0)
        {
            Debug.LogError("Invalid walkable map data or out of bounds access attempted.");
            return false;
        }
        return walkableMap[y, x] && !IsObstacleTriggerUnwalkableAt(new Vector3(x, y, 0));
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

    // Shove Related Code ***************************************************************

    public void Shove(Direction direction)
    {
        Vector3 directionVector = DirectionToVector(direction);
        Vector3 newPosition = transform.position + directionVector;
        int newX = Mathf.FloorToInt(newPosition.x);
        int newY = Mathf.FloorToInt(newPosition.y);

        bool[,] walkableMap = pathfindingManager.GetWalkableMap();

        if ((walkableMap[newY, newX] || IsObstacleTriggerUnwalkableAt(newPosition)) && CheckForEnemyAtPosition(newPosition) == null)
        {
            HandleShoveFeedback(direction);
        }
        else
        {
            GameObject enemyAtPosition = CheckForEnemyAtPosition(newPosition);
            if (enemyAtPosition != null)
            {
                EnemyProperties otherEnemyProperties = enemyAtPosition.GetComponent<EnemyProperties>();
                otherEnemyProperties.TakeDamage(1f, EnemyProperties.DamageType.BallBounce);
            }

            HandleSquashFeedback(direction);
            enemyProperties.TakeDamage(1f, EnemyProperties.DamageType.BallBounce);
        }
    }

    private Vector3 DirectionToVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector3(0, 1, 0),
            Direction.Down => new Vector3(0, -1, 0),
            Direction.Left => new Vector3(-1, 0, 0),
            Direction.Right => new Vector3(1, 0, 0),
            _ => Vector3.zero,
        };
    } //??? CHECK HOW THIS WORKS

    // Feedback Related Code ***************************************************************

    private void HandleShoveFeedback(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                enemyFeedback.ShoveUp();
                break;
            case Direction.Down:
                enemyFeedback.ShoveDown();
                break;
            case Direction.Left:
                enemyFeedback.ShoveLeft();
                break;
            case Direction.Right:
                enemyFeedback.ShoveRight();
                break;
        }
    }

    private void HandleSquashFeedback(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                enemyFeedback.SquashUp();
                break;
            case Direction.Down:
                enemyFeedback.SquashDown();
                break;
            case Direction.Left:
                enemyFeedback.SquashLeft();
                break;
            case Direction.Right:
                enemyFeedback.SquashRight();
                break;
        }
    }

// Miscellaneous ***************************************************************

    private void ResetMovement()
    {
        enemyHasMoved = false;
        enemyFeedback.MoveTextBounce();
        moveMoneyDecrement = moveMoney;
    }

    public bool HasMoved()
    {
        return enemyHasMoved;
    }

// OBSERVERS *******************

    //OnEnable is up top!

    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(TurnManager.GameState newGameState){
        if(newGameState == TurnManager.GameState.PlayerTurn){
            ResetMovement();
        }
    }

}