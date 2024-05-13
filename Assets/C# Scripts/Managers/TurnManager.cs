using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    void Awake()
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
/*--------------------------------------------------------------------------------------------------*/
    public enum GameState
    {
        Null,
        PlayerTurn,
        EnemyTurn,
        Win,
        Loss,
    }

    public GameState currentState = GameState.Null;
    private GameState previousState;
    
    GameObject Balldyseus;
    BallMovement ballMovement;
    bool BalldyseusStopped;
    bool BalldyseusIsMoving;

    public Camera mainCamera;
    private CameraManager cameraManager;

    List<GameObject> enemies = new List<GameObject>();

    private PathfindingManager pathfindingManager;

    public float TurnNumber = 0;


/*--------------------------------------------------------------------------------------------------*/

    // Initialize enemy list, update the walkable map, start the player's turn
    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        Balldyseus = GameObject.Find("Balldyseus");

        if (pathfindingManager == null) Debug.LogError("PathfindingManager not found.");
        if (cameraManager == null) Debug.LogError("CameraManager not found.");
        if (Balldyseus == null) Debug.LogError("Balldyseus not found.");

        ballMovement = Balldyseus.GetComponent<BallMovement>();
        
        UIManager.Instance.ShowGameplayUI(); //THIS IS CURRENTLY ONLY HERE BECAUSE OF UNIMPLEMENTED BOUNCE COUNT UI. REMOVE THIS ONCE THATS IMPLEMENTED

        ChangeGameState("PlayerTurn"); //Player turn begins
    }

    void Update()
    {
        if(ballMovement != null){
            BalldyseusStopped = ballMovement.HasStopped();
            BalldyseusIsMoving = ballMovement.IsMoving();
        }

        // During a Player's Turn
        if (currentState == GameState.PlayerTurn)
        {
            if (BalldyseusStopped){
                UpdateEnemiesList();
                CheckForWin();
                if(currentState != GameState.Win){
                    ChangeGameState("EnemyTurn");
                }
            }
        }

    }
/*--------------------------------------------------------------------------------------------------*/
    public void ChangeGameState(string stateString){
        GameState newState = ReturnGameStateFromString(stateString);

        if (newState == currentState) return;

        previousState = currentState;
        
        currentState = newState; 

        switch (newState){
            case GameState.Null:
                // Handle Null state
                break;

            case GameState.PlayerTurn:
                if(currentState != GameState.Loss && currentState != GameState.Win){
                    ballMovement.ResetForcePercentage();
                    UpdateEnemiesList();
                    CheckForWin();
                    cameraManager.SetCameraForPlayerTurn(Balldyseus.transform);
                    Balldyseus.GetComponent<BallMovement>().ResetMovement();
                    TurnNumber++;
                }
                break;

            case GameState.EnemyTurn:
                if(currentState != GameState.Loss){
                    cameraManager.SetCameraForEnemyTurn();
                    if(enemies.Count == 0){
                        CheckForWin();
                    }
                    else{
                        StartCoroutine(EnemyTurnRoutine());
                    }
                }
                break;

            case GameState.Win:
                HandleWin();
                break;

            case GameState.Loss:
                HandleLoss();
                break;
        }

        Debug.Log("Notifying of New State: " + newState.ToString());
        GameStateEventPublisher.NotifyGameStateChange(newState);
    }

    //this returns the gamestate from a given string. this is used for the GameStateEventPublisher which needs to take in a 
    //GameState variable rather than a string
    private GameState ReturnGameStateFromString(string stateString) {
        switch (stateString){
            case "Null": return GameState.Null;
            case "PlayerTurn": return GameState.PlayerTurn;
            case "EnemyTurn": return GameState.EnemyTurn;
            case "Win": return GameState.Win;
            case "Loss": return GameState.Loss;
            default: return previousState;
        }
    }

/*--------------------------------------------------------------------------------------------------*/



/*--------------------------------------------------------------------------------------------------*/
    //MANAGING THE EXISTING ENEMIES
    public void UpdateEnemiesList()
    {
        enemies.Clear();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        pathfindingManager.UpdateWalkableMap(enemies);
    }

    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy)){
            enemies.Add(enemy);
            pathfindingManager.UpdateWalkableMap(enemies);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Remove(enemy)){
            pathfindingManager.UpdateWalkableMap(enemies);
        }
        CheckForWin();
    }

    // For every enemy, for moveMoney times, call move & wait secondsBetweenEnemyMoves
    IEnumerator EnemyTurnRoutine()
    {
        var enemiesToMove = new List<GameObject>(enemies);

        foreach (var enemyGameObject in enemiesToMove){
            if(currentState == GameState.Loss) break;
            EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();

            enemyProps.ThisEnemyTurnBegins();

            //StartCoroutine(DoThisBeforeEnemyMoves(enemyGameObject));
            //ideally we put all the stuff thats supposed to be checked prior to a movement in its own function here
            //for now we just put the fire here to easily wait for flame dmg animation
            if(enemyProps.isOnFire){
                Fire.ApplyFireDamageIfOnFire(enemyProps);
                yield return new WaitForSeconds(.4f);
            }

            if (enemyProps.IsDefeated())
            {
                RemoveEnemy(enemyGameObject);
                continue;
            }

            UpdateEnemiesList();
            
            if (enemyGameObject != null){
                enemyMovement.Move();
                yield return new WaitUntil(() => enemyMovement.HasMoved());

                UpdateEnemiesList();
                enemyProps.ThisEnemyTurnEnds();
            }
        }

        // After all enemies have moved
        foreach (var enemyGameObject in enemiesToMove){
            if(enemyGameObject != null){
                EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
                enemyMovement.ResetMoveMoney();
                enemyMovement.ResetMovement();
            }
        }

        ResolveCollisionsWithEnemies();
        ChangeGameState("PlayerTurn");
    }

    /*IEnumerator DoThisBeforeEnemyMoves(GameObject enemyGameObject){
        EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();
        if(enemyProps.isOnFire){
            Fire.ApplyFireDamageIfOnFire(enemyProps);
            yield return new WaitForSeconds(1f);
        }
    }*/

    //moves Balldyeus away from an enemy by a small amount if colliding when turn starts, to allow Balldyeus to attack the enemy if desired
    void ResolveCollisionsWithEnemies()
    {
        Collider2D balldyseusCollider = Balldyseus.GetComponent<Collider2D>();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(Balldyseus.transform.position, balldyseusCollider.bounds.extents.x);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = hitCollider.transform.position - Balldyseus.transform.position;
                Balldyseus.transform.position -= directionToEnemy.normalized * 0.05f; 
                break; 
            }
        }
    }

//--------------------------------------------------------------------------------------------------

    private void CheckForWin(){
        if(enemies.Count == 0 && currentState != GameState.Win && currentState != GameState.Loss)
        {
            Debug.Log("WIN DETECTED");
            ChangeGameState("Win");
        }
    }

    void HandleWin(){
        currentState = GameState.Win;
    }

    void HandleLoss(){
        Balldyseus.SetActive(false);
    }

    public void OnEnemyReachedObjective()
    {
        if (currentState != GameState.Loss)
        {
            ChangeGameState("Loss");
        }
    }

    public float GetTurnNumber(){
        return TurnNumber;
    }
}