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

    [SerializeField] private PathfindingManager pathfindingManager;

    public GameState currentState = GameState.Null;
    public float TurnNumber { get; private set; }

    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> enemiesToRemove = new List<GameObject>();

    private GameState previousState;

    bool currentEnemyTurnActive = false;

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

        if (pathfindingManager == null) Debug.LogError("PathfindingManager not found.");
        
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
                currentState = GameState.Win;
                break;

            case GameState.Loss:
                currentState = GameState.Loss;
                break;
        }

        GameStatePublisher.NotifyGameStateChange(newState);
    }

    private void HandlePlayerTurnStart()
    {
        if (currentState != GameState.Loss && currentState != GameState.Win)
        {
            UpdateEnemiesList();
            UpdateEnemyLocations();
            CheckForWin();
            TurnNumber++;
        }
    }

    public void UpdateEnemiesList()
    {
        // Remove any inactive or destroyed enemies
        enemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);

        GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in currentEnemies)
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
    }

    public void UpdateEnemyLocations(){
        pathfindingManager.UpdateWalkableMap(enemies);
    }

    private void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Remove(enemy))
        {
            UpdateEnemyLocations();
        }
        CheckForWin();
    }

    private IEnumerator EnemyTurnRoutine()
    {
        //Remove Enemies if needed
        foreach(var enemyToRemove in enemiesToRemove){
            RemoveEnemy(enemyToRemove);
            Destroy(enemyToRemove);
        }
        enemiesToRemove.Clear();

        var enemiesToMove = new List<GameObject>(enemies);

        foreach (var enemyGameObject in enemiesToMove)
        {
            if (currentState == GameState.Loss) break;
            if (enemyGameObject == null) continue;
            
            currentEnemyTurnActive = true;
            EnemyTurnPublisher.NotifyEnemyTurnChange(enemyGameObject);
            yield return new WaitUntil(() => !currentEnemyTurnActive);
        }

        //Remove Enemies if needed
        foreach(var enemyToRemove in enemiesToRemove){
            RemoveEnemy(enemyToRemove);
            Destroy(enemyToRemove);
        }

        CheckForWin();

        if(currentState != GameState.Win || currentState != GameState.Loss){
            ChangeGameState(GameState.PlayerTurn);
        }
    }

    public void FlagEnemyForRemovalAtEndOfRound(GameObject enemyGameObject){
        enemiesToRemove.Add(enemyGameObject);
    }

    private void CheckForWin()
    {
        if (enemies.Count == 0 && currentState != GameState.Win && currentState != GameState.Loss)
        {
            Debug.Log("WIN DETECTED");
            ChangeGameState(GameState.Win);
        }
    }


//***************OBSERVERS********************
    private void OnEnable(){
        MovementStatePublisher.MovementStateChange += OnMovementStateChange;
        EndEnemyTurnPublisher.EndEnemyTurn += OnEndEnemyTurn;
        EnemyHealthChangePublisher.EnemyHealthChange += OnEnemyHealthChange;
        EnemyEnteredObjectivePublisher.EnemyEnteredObjective += OnEnemyEnteredObjective;
    }

    private void OnDisable(){
        MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
        EndEnemyTurnPublisher.EndEnemyTurn -= OnEndEnemyTurn;
        EnemyHealthChangePublisher.EnemyHealthChange -= OnEnemyHealthChange;
        EnemyEnteredObjectivePublisher.EnemyEnteredObjective -= OnEnemyEnteredObjective;
    }

    private void OnMovementStateChange(BallMovement.MovementState newMovementState)
    {
        if (newMovementState == BallMovement.MovementState.HasCompletedMovement && currentState == GameState.PlayerTurn)
        {
            UpdateEnemiesList();
            UpdateEnemyLocations();

            CheckForWin();
            if (currentState != GameState.Win)
            {
                ChangeGameState(GameState.EnemyTurn);
            }
        }
    }

    void OnEndEnemyTurn(GameObject enemyWhoseTurnEnded){
        currentEnemyTurnActive = false;
    }

    void OnEnemyHealthChange(GameObject enemyWhoLostDamage, float newHealthCount, float amountLost, EnemyProperties.DamageType damageType){
        if(newHealthCount <= 0f) FlagEnemyForRemovalAtEndOfRound(enemyWhoLostDamage);
    }

    private void OnEnemyEnteredObjective(GameObject enemy){
        if (currentState != GameState.Loss)
        {
            ChangeGameState(GameState.Loss);
        }
    }
}