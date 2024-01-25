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
        Cutscene,
        PlayerTurn,
        EnemyTurn,
        Win,
        Loss
    }

    public GameState currentState;

    public GameObject Balldyseus;
    public Camera mainCamera;

    List<GameObject> enemies = new List<GameObject>();
    bool enemiesHaveMoved = false;

    public PathfindingManager pathfindingManager;
    [SerializeField] float secondsBetweenEnemyMoves = 0.5f;

    public GameObject playerTurnUI;
    public GameObject enemyTurnUI;

    public GameObject winUI;
    public GameObject lossUI;

    public GameObject OuterCanvasUI;

    public float TurnNumber = 0;

    [SerializeField] BaseCutscene beginningCutscene;
    [SerializeField] BaseCutscene endingCutscene;

/*--------------------------------------------------------------------------------------------------*/
    // initialize enemy list, update the walkable map, start the player's turn
    void Start()
    {
        TurnOffAllUI();
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
                StartEnemyTurn();
            }

        }
        
    }
/*--------------------------------------------------------------------------------------------------*/

    void CheckForBeginningCutscene()
    {
        if (beginningCutscene != null)
        {
            currentState = GameState.Cutscene;
            beginningCutscene.StartCutscene();
        }
        else
        {
            StartPlayerTurn();
        }
    }

    private void CheckForEndingCutscene(){
        if (endingCutscene != null)
        {
            currentState = GameState.Cutscene;
            endingCutscene.StartCutscene();
        }
        else
        {
            currentState = GameState.Win;
            StartCoroutine(HandleWin());
        }
    }

/*--------------------------------------------------------------------------------------------------*/
    //start player's turn
    public void StartPlayerTurn()
    {
        TurnOnAllUI();
        UpdateEnemiesList();
        CheckForWin();
        
        TurnNumber++;
        currentState = GameState.PlayerTurn;

        playerTurnUI.SetActive(true);
        enemyTurnUI.SetActive(false);

        Balldyseus.GetComponent<BallMovement>().ResetMovement();
        // center camera on Balldyseus
    }

    // begin Enemy Turn Routine
    public void StartEnemyTurn()
    {
        if(enemies.Count == 0){
            CheckForWin();
        }
        else{
            currentState = GameState.EnemyTurn;

            playerTurnUI.SetActive(false);
            enemyTurnUI.SetActive(true);

            StartCoroutine(EnemyTurnRoutine());
        }

    }

    //for every enemy, for moveMoney times, call move & wait secondsBetweenEnemyMoves
    IEnumerator EnemyTurnRoutine()
    {
        var enemiesToMove = new List<GameObject>(enemies);

        foreach (var enemyGameObject in enemiesToMove)
        {
            EnemyMovement enemyMovement = enemyGameObject.GetComponent<EnemyMovement>();
            EnemyProperties enemyProps = enemyGameObject.GetComponent<EnemyProperties>();

            // Apply fire damage first
            Fire.ApplyFireDamageIfOnFire(enemyProps);

            if (enemyProps.IsDefeated())
            {
                RemoveEnemy(enemyGameObject);
                continue;
            }

            int moves = enemyMovement.moveMoney;
            for (int i = 0; i < moves; i++)
            {
                enemyMovement.Move();
                yield return new WaitUntil(() => enemyMovement.HasMoved());
                yield return new WaitForSeconds(secondsBetweenEnemyMoves);
                UpdateEnemiesList();
                if(currentState == GameState.Loss){
                    break;
                }
            }
            
            enemyMovement.CompletedAllMovements();
            enemyMovement.ResetAllMovements();
        }

        // After all enemies have moved
        StartPlayerTurn();
    }


/*--------------------------------------------------------------------------------------------------*/

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

/*----------------------------------------------------------------------------------------------------*/
    
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

    //WIN/LOSS STUFF

    public void OnEnemyReachedObjective()
    {
        if (currentState != GameState.Loss)
        {
            currentState = GameState.Loss;
            StartCoroutine(HandleLoss());
        }
    }

    IEnumerator HandleLoss(){
        Balldyseus.SetActive(false);
        playerTurnUI.SetActive(false);
        enemyTurnUI.SetActive(false);

        yield return new WaitForSeconds(1);
        lossUI.SetActive(true);
    }

    private void CheckForWin(){
        if(enemies.Count == 0 && currentState != GameState.Win && currentState != GameState.Loss)
        {
            CheckForEndingCutscene();
            
        }
    }


    public IEnumerator HandleWin(){
        playerTurnUI.SetActive(false);
        enemyTurnUI.SetActive(false);

        yield return new WaitForSeconds(1);
        winUI.SetActive(true);
    }

    public float GetTurnNumber(){
        return TurnNumber;
    }

    void TurnOffAllUI(){
        winUI.SetActive(false);
        lossUI.SetActive(false);
        playerTurnUI.SetActive(false);
        enemyTurnUI.SetActive(false);
        OuterCanvasUI.SetActive(false);
    }

    public void TurnOnAllUI(){
        OuterCanvasUI.SetActive(true);
    }

    public void BeginHandlingWinCoroutine(){
        StartCoroutine(HandleWin());
    }
}