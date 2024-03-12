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
        Cutscene,
        PlayerTurn,
        EnemyTurn,
        Win,
        Loss,
        Paused
    }

    public GameState currentState = GameState.Null;
    private GameState previousState;
    
    public GameObject Balldyseus;
    public Camera mainCamera;

    List<GameObject> enemies = new List<GameObject>();
    bool enemiesHaveMoved = false;

    public PathfindingManager pathfindingManager;

    bool endingCutsceneBegan = false;

    public float TurnNumber = 0;
    [SerializeField] float secondsBetweenEnemyMoves = 0.5f;

    [SerializeField] BaseCutscene beginningCutscene;
    [SerializeField] BaseCutscene endingCutscene;

/*--------------------------------------------------------------------------------------------------*/
    // initialize enemy list, update the walkable map, start the player's turn
    void Start()
    {
        ResumeGame();
        UIManager2.Instance.DestroyAllUIElements(); 
        CheckForBeginningCutscene();
    }

    // if player stops, update enemy list + map
    void Update()
    {
        //During a Player's Turn
        if (currentState == GameState.PlayerTurn)
        {
            CheckForWin();
            if (Balldyseus.GetComponent<BallMovement>().HasStopped()){
                UpdateEnemiesList();
                ChangeGameState("EnemyTurn");
            }

        }


        if (Input.GetKeyDown(KeyCode.Escape) && !Balldyseus.GetComponent<BallMovement>().IsMoving()){
            if (currentState != GameState.Paused){
                ChangeGameState("Paused");
            }
            else{
                ResumeGame();
            }
        }

        //Debug.Log(currentState);
    }
/*--------------------------------------------------------------------------------------------------*/
    public void ChangeGameState(string stateString){
        
        UIManager2.Instance.DestroyAllUIElements(); 

        switch (stateString){
            case "Null":
                currentState = GameState.Null;
                break;

            case "Cutscene":
                currentState = GameState.Cutscene;
                break;

            case "PlayerTurn":
                UIManager2.Instance.InstantiatePlayerTurnUI();
                UpdateEnemiesList();
                CheckForWin();
                currentState = GameState.PlayerTurn;
                TurnNumber++;
                Balldyseus.GetComponent<BallMovement>().ResetMovement();
                break;

            case "EnemyTurn":
                if(enemies.Count == 0){
                    CheckForWin();
                }
                else{
                    currentState = GameState.EnemyTurn;
                    UIManager2.Instance.InstantiateEnemyTurnUI();
                    StartCoroutine(EnemyTurnRoutine());
                }
                break;

            case "Win":
                StartCoroutine(HandleWin());
                break;

            case "Loss":
                StartCoroutine(HandleLoss());
                break;

            case "Paused":
                currentState = GameState.Paused;
                break;

            case "PreviousState":
                currentState = previousState;
                break;
        }

        if(currentState != GameState.Paused){
            previousState = currentState;
        }
    }


    void CheckForBeginningCutscene()
    {
        if (beginningCutscene != null)
        {
            ChangeGameState("Cutscene");
            beginningCutscene.StartCutscene();
        }
        else
        {
            ChangeGameState("PlayerTurn");
        }
    }

    private void CheckForEndingCutscene(){
        if (endingCutscene != null)
        {
            endingCutsceneBegan = true;
            ChangeGameState("Cutscene");
            endingCutscene.StartCutscene();
        }
        else
        {
            ChangeGameState("Win");
        }
    }

/*--------------------------------------------------------------------------------------------------*/

    void PauseGame()
    {
        UIManager2.Instance.ShowUIElement("PauseMenuUI");
        Time.timeScale = 0f;
        ChangeGameState("Paused");
    }

    public void ResumeGame()
    {
        UIManager2.Instance.DestroyUIElement("PauseMenuUI");
        Time.timeScale = 1f; 
        currentState = previousState; 
        ChangeGameState("PreviousState");
    }

/*--------------------------------------------------------------------------------------------------*/
    
    //for every enemy, for moveMoney times, call move & wait secondsBetweenEnemyMoves
    IEnumerator EnemyTurnRoutine()
    {
        var enemiesToMove = new List<GameObject>(enemies);

        foreach (var enemyGameObject in enemiesToMove)
        {
            UpdateEnemiesList();
            EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();

            DoThisBeforeEnemyMoves(enemyGameObject);

            if (enemyProps.IsDefeated())
            {
                RemoveEnemy(enemyGameObject);
                continue;
            }
            int moves = enemyMovement.moveMoney;
            UpdateEnemiesList();
            for (int i = 0; i < moves; i++)
            {
                UpdateEnemiesList();
                enemyMovement.Move();
                yield return new WaitUntil(() => enemyMovement.HasMoved());
                UpdateEnemiesList();
                yield return new WaitForSeconds(secondsBetweenEnemyMoves);
                UpdateEnemiesList();

                if(currentState == GameState.Loss){
                    break;
                }
            }
            enemyMovement.CompletedAllMovements();
            enemyMovement.ResetAllMovements();
            UpdateEnemiesList();
        }
        UpdateEnemiesList();

        // After all enemies have moved
        ChangeGameState("PlayerTurn");
    }

    void DoThisBeforeEnemyMoves(GameObject enemyGameObject){
        EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();
        Fire.ApplyFireDamageIfOnFire(enemyProps);
    }

/*--------------------------------------------------------------------------------------------------

    private bool CheckIfAllEnemiesHaveMoved()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.GetComponent<EnemyMovement>().HasMoved())
            {
                return false;
            }
        }
        return true;
    }

    private void ResetEnemyMovements()
    {
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<EnemyMovement>().ResetMovement();
        }
        enemiesHaveMoved = false;
    }

----------------------------------------------------------------------------------------------------*/
    
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
/*--------------------------------------------------------------------------------------------------*/

    //LOSS STUFF

    public void OnEnemyReachedObjective()
    {
        if (currentState != GameState.Loss)
        {
            ChangeGameState("Loss");
        }
    }

    IEnumerator HandleLoss(){
        Balldyseus.SetActive(false);

        yield return new WaitForSeconds(.5f);
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("LossUI"); 
        currentState = GameState.Loss;
    }

/*--------------------------------------------------------------------------------------------------*/

    //WIN STUFF
    private void CheckForWin(){
        if(enemies.Count == 0 && currentState != GameState.Win && currentState != GameState.Loss && !endingCutsceneBegan)
        {
            CheckForEndingCutscene();
            
        }
    }

    private IEnumerator HandleWin(){
        yield return new WaitForSeconds(0f);
        
        UIManager2.Instance.HideAllUIElements();
        UIManager2.Instance.ShowUIElement("WinUI");
        currentState = GameState.Win;
    }

/*--------------------------------------------------------------------------------------------------*/

    //public functions

    public float GetTurnNumber(){
        return TurnNumber;
    }

    public void BeginHandlingWinCoroutine()
{
        StartCoroutine(HandleWin());}
}