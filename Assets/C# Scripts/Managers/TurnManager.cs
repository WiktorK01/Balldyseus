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
        Paused
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
        ballMovement = Balldyseus.GetComponent<BallMovement>();

        ResumeGame();
        UIManager.Instance.ShowGameplayUI();
        ChangeGameState("PlayerTurn");
    }

    // If player stops, update enemy list + map
    void Update()
    {
        if(ballMovement.BallExists()){
            BalldyseusStopped = ballMovement.HasStopped();
            BalldyseusIsMoving = ballMovement.IsMoving();
        }

        // During a Player's Turn
        if (currentState == GameState.PlayerTurn)
        {
            CheckForWin();
            if (BalldyseusStopped){
                UpdateEnemiesList();
                ChangeGameState("EnemyTurn");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !BalldyseusIsMoving){
            if (currentState != GameState.Paused){
                ChangeGameState("Paused");
            }
            else{
                ResumeGame();
            }
        }
    }
/*--------------------------------------------------------------------------------------------------*/
    public void ChangeGameState(string stateString){
        GameState newState = ReturnGameStateFromString(stateString);

        switch (stateString){
            case "Null":
                currentState = GameState.Null;
                break;

            case "PlayerTurn":
                if(currentState != GameState.Loss){
                    ballMovement.ResetForcePercentage();
                    UpdateEnemiesList();
                    CheckForWin();
                    cameraManager.SetCameraForPlayerTurn(Balldyseus.transform);
                    if(currentState != GameState.Win){
                        currentState = GameState.PlayerTurn;
                        TurnNumber++;
                        Balldyseus.GetComponent<BallMovement>().ResetMovement();
                    }
                }
                break;

            case "EnemyTurn":
                if(currentState != GameState.Loss){
                    cameraManager.SetCameraForEnemyTurn();
                    if(enemies.Count == 0){
                        CheckForWin();
                    }
                    else{
                        currentState = GameState.EnemyTurn;
                        StartCoroutine(EnemyTurnRoutine());
                    }
                }
                break;

            case "Win":
                HandleWin();
                break;

            case "Loss":
                currentState = GameState.Loss;
                HandleLoss();
                break;

            case "Paused":
                currentState = GameState.Paused;
                PauseGame();
                break;

            case "PreviousState":
                string stateName = previousState.ToString();
                ChangeGameState(stateName);
                break;
        }

        //this block here notifies the GameStateEventPublisher of any changes, which will be sent to any other subscribed objects
        if(previousState != newState){
            Debug.Log("Notifying of New State " + stateString);
            currentState = newState;
            GameStateEventPublisher.NotifyGameStateChange(newState);
        }

        if(currentState != GameState.Paused){
            previousState = currentState;
        }

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
            case "Paused": return GameState.Paused;
            default: return previousState;
        }
    }

/*--------------------------------------------------------------------------------------------------*/

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        ChangeGameState("PreviousState");
    }    

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
            currentState = GameState.Win;
            HandleWin();
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