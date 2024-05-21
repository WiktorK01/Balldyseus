using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;
    private GameObject centerObject; // Declare centerObject as a class member variable
    public Vector3 enemyTurnCameraPosition;

    public float playerOrthoSize = 5f;
    public float enemyOrthoSize = 10f;

    public float playerDamp = 4f;
    public float enemyDamp = 1f;

    public float zoomDuration = 0.25f;

    private bool isHoldingC = false;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        centerObject = new GameObject("EmptyObject");
        centerObject.transform.position = enemyTurnCameraPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)||Input.GetMouseButtonDown(2))
        {
            isHoldingC = true;
            SetCameraForEnemyTurn();
        }
        if (Input.GetKeyUp(KeyCode.C)||Input.GetMouseButtonUp(2))
        {
            isHoldingC = false;
            if (TurnManager.Instance.currentState == TurnManager.GameState.PlayerTurn)
            {
                SetCameraForPlayerTurn();
            }
        }
    }

    public void SetCameraForPlayerTurn()
    {
        Transform playerTransform = GetPlayerTransform();
        if (playerTransform != null)
        {
            cinemachineCamera = GetComponent<CinemachineVirtualCamera>(); // getting the component in this function ensures no oddities upon a scene restart
            StartCoroutine(TransitionOrthographicSize(playerOrthoSize));
            cinemachineCamera.Follow = playerTransform;
            SetDamping(playerDamp);
        }
    }

    public void SetCameraForEnemyTurn()
    {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        StartCoroutine(TransitionOrthographicSize(enemyOrthoSize));
        SetDamping(enemyDamp);
        cinemachineCamera.Follow = centerObject.transform;
    }

    void SetDamping(float damping)
    {
        CinemachineFramingTransposer transposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_XDamping = damping;
            transposer.m_YDamping = damping;
        }
    }

    IEnumerator TransitionOrthographicSize(float targetOrthoSize)
    {
        float startOrthoSize = cinemachineCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < zoomDuration)
        {
            float newOrthoSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, elapsedTime / zoomDuration);
            cinemachineCamera.m_Lens.OrthographicSize = newOrthoSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cinemachineCamera.m_Lens.OrthographicSize = targetOrthoSize;
    }

    private Transform GetPlayerTransform()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null){
            return playerObject.transform;
        }
        else return null;
    }

//************OBSERVERS***************

    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
    }

    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.PlayerTurn){
            SetCameraForPlayerTurn();
        }

        else if(newState == TurnManager.GameState.EnemyTurn){
            SetCameraForEnemyTurn();
        }
    }
}