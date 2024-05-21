using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public enum GameState
    {
        Null,
        PlayerTurn,
        EnemyTurn,
        Win,
        Loss,
    }

    [SerializeField] private GameObject balldyseus;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PathfindingManager pathfindingManager;

    public GameState currentState = GameState.Null;
    public float TurnNumber { get; private set; }

    private List<GameObject> enemies = new List<GameObject>();
    private GameState previousState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        balldyseus = GameObject.Find("Balldyseus");

        if (pathfindingManager == null) Debug.LogError("PathfindingManager not found.");
        if (balldyseus == null) Debug.LogError("Balldyseus not found.");
        
        ChangeGameState(GameState.PlayerTurn); //Player turn begins
    }

    public void ChangeGameState(GameState newState)
    {
        if (newState == currentState) return;

        previousState = currentState;
        currentState = newState;

        switch (newState)
        {
            case GameState.Null:
                break;

            case GameState.PlayerTurn:
                HandlePlayerTurnStart();
                break;

            case GameState.EnemyTurn:
                if (enemies.Count == 0)
                {
                    CheckForWin();
                }
                else
                {
                    StartCoroutine(EnemyTurnRoutine());
                }
                break;

            case GameState.Win:
                HandleWin();
                break;

            case GameState.Loss:
                HandleLoss();
                break;
        }

        GameStatePublisher.NotifyGameStateChange(newState);
    }

    private void HandlePlayerTurnStart()
    {
        if (currentState != GameState.Loss && currentState != GameState.Win)
        {
            UpdateEnemiesList();
            CheckForWin();
            TurnNumber++;
        }
    }

    public void UpdateEnemiesList()
    {
        enemies.Clear();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        pathfindingManager.UpdateWalkableMap(enemies);
    }

    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            pathfindingManager.UpdateWalkableMap(enemies);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Remove(enemy))
        {
            pathfindingManager.UpdateWalkableMap(enemies);
        }
        CheckForWin();
    }

    private IEnumerator EnemyTurnRoutine()
    {
        var enemiesToMove = new List<GameObject>(enemies);

        foreach (var enemyGameObject in enemiesToMove)
        {
            if (currentState == GameState.Loss) break;
            
            var enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            var enemyProps = enemyGameObject.GetComponent<EnemyProperties>();

            enemyProps.ThisEnemyTurnBegins();

            if (enemyProps.isOnFire)
            {
                Fire.ApplyFireDamageIfOnFire(enemyProps);
                yield return new WaitForSeconds(0.4f);
            }

            if (enemyProps.IsDefeated())
            {
                RemoveEnemy(enemyGameObject);
                continue;
            }

            UpdateEnemiesList();

            if (enemyGameObject != null)
            {
                enemyMovement.Move();
                yield return new WaitUntil(() => enemyMovement.HasMoved());

                UpdateEnemiesList();
                enemyProps.ThisEnemyTurnEnds();
            }
        }

        ResetEnemyMovements(enemiesToMove);
        ResolveCollisionsWithEnemies();
        ChangeGameState(GameState.PlayerTurn);
    }

    private void ResetEnemyMovements(List<GameObject> enemiesToMove)
    {
        foreach (var enemyGameObject in enemiesToMove)
        {
            if (enemyGameObject != null)
            {
                var enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
                enemyMovement.ResetMoveMoney();
                enemyMovement.ResetMovement();
            }
        }
    }

    private void ResolveCollisionsWithEnemies()
    {
        var balldyseusCollider = balldyseus.GetComponent<Collider2D>();
        var hitColliders = Physics2D.OverlapCircleAll(balldyseus.transform.position, balldyseusCollider.bounds.extents.x);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = hitCollider.transform.position - balldyseus.transform.position;
                balldyseus.transform.position -= directionToEnemy.normalized * 0.05f;
                break;
            }
        }
    }

    private void CheckForWin()
    {
        if (enemies.Count == 0 && currentState != GameState.Win && currentState != GameState.Loss)
        {
            Debug.Log("WIN DETECTED");
            ChangeGameState(GameState.Win);
        }
    }

    private void HandleWin()
    {
        currentState = GameState.Win;
    }

    private void HandleLoss()
    {
        currentState = GameState.Loss;
        balldyseus.SetActive(false);
    }

    public void OnEnemyReachedObjective()
    {
        if (currentState != GameState.Loss)
        {
            ChangeGameState(GameState.Loss);
        }
    }




//***************OBSERVERS********************
    private void OnEnable(){
        MovementStatePublisher.MovementStateChange += OnMovementStateChange;
    }

    private void OnDisable(){
        MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
    }

    private void OnMovementStateChange(BallMovement.MovementState newState)
    {
        if (newState == BallMovement.MovementState.HasCompletedMovement && currentState == GameState.PlayerTurn)
        {
            UpdateEnemiesList();
            CheckForWin();
            if (currentState != GameState.Win)
            {
                ChangeGameState(GameState.EnemyTurn);
            }
        }
    }
}