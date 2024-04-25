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
    
    public GameObject Balldyseus;
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

        ResumeGame();
        UIManager2.Instance.DestroyAllUIElements(); 
        ChangeGameState("PlayerTurn");
    }

    // If player stops, update enemy list + map
    void Update()
    {
        if(Balldyseus.GetComponent<BallMovement>().BallExists()){
            BalldyseusStopped = Balldyseus.GetComponent<BallMovement>().HasStopped();
            BalldyseusIsMoving = Balldyseus.GetComponent<BallMovement>().IsMoving();
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
        
        UIManager2.Instance.DestroyAllUIElements(); 

        switch (stateString){
            case "Null":
                currentState = GameState.Null;
                break;

            case "PlayerTurn":
                if(currentState != GameState.Loss){
                    UpdateEnemiesList();
                    CheckForWin();
                    cameraManager.SetCameraForPlayerTurn(Balldyseus.transform);
                    InstantiatePlayerTurnUI();
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
                        InstantiateEnemyTurnUI();
                        StartCoroutine(EnemyTurnRoutine());
                    }
                }
                break;

            case "Win":
                StartCoroutine(HandleWin());
                break;

            case "Loss":
                currentState = GameState.Loss;
                StartCoroutine(HandleLoss());
                break;

            case "Paused":
                currentState = GameState.Paused;
                UIManager2.Instance.ShowUIElement("PauseMenuUI");
                PauseGame();
                break;

            case "PreviousState":
                string stateName = previousState.ToString();
                ChangeGameState(stateName);
                break;
        }

        if(currentState != GameState.Paused){
            previousState = currentState;
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
    
    public void InstantiatePlayerTurnUI(){
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("PlayerTurnUI");
        UIManager2.Instance.ShowUIElement("LaunchUI");

        if(!Balldyseus.GetComponent<BallProperties>().ShoveGagged){
            UIManager2.Instance.ShowUIElement("ImpulseCountUI");
        }
    }

    public void InstantiateEnemyTurnUI(){
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("EnemyTurnUI");
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

        foreach (var enemyGameObject in enemiesToMove)
        {
            if(currentState == GameState.Loss) break;

            EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();

            enemyProps.ThisEnemyTurnBegins();

            UpdateEnemiesList();

            DoThisBeforeEnemyMoves(enemyGameObject);

            if (enemyProps.IsDefeated())
            {
                RemoveEnemy(enemyGameObject);
                continue;
            }

            UpdateEnemiesList();
            

            enemyMovement.Move();
            yield return new WaitUntil(() => enemyMovement.HasMoved());

            UpdateEnemiesList();
            enemyProps.ThisEnemyTurnEnds();
        }

        // After all enemies have moved
        foreach (var enemyGameObject in enemiesToMove){
            EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            enemyMovement.ResetMoveMoney();
            enemyMovement.ResetMovement();
        }

        ResolveCollisionsWithEnemies();
        ChangeGameState("PlayerTurn");
    }

    void DoThisBeforeEnemyMoves(GameObject enemyGameObject){
        EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();
        Fire.ApplyFireDamageIfOnFire(enemyProps);
    }

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
            StartCoroutine(HandleWin());
        }
    }

    IEnumerator HandleWin(){
        yield return new WaitForSeconds(0f);
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("WinUI");
        currentState = GameState.Win;
    }

    IEnumerator HandleLoss(){
        Balldyseus.SetActive(false);
        yield return new WaitForSeconds(.5f);
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("LossUI");
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

    public void BeginHandlingWinCoroutine()
    {
        StartCoroutine(HandleWin());
    }
}