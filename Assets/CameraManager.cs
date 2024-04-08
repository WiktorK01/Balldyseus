using System.Collections;
using System.Collections.Generic;
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

    public float zoomDuration = .25f;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        centerObject = new GameObject("EmptyObject");
        centerObject.transform.position = enemyTurnCameraPosition;
    }

    public void SetCameraForPlayerTurn(Transform playerTransform)
    {
        StartCoroutine(TransitionOrthographicSize(playerOrthoSize));
        SetDamping(playerDamp);
        cinemachineCamera.Follow = playerTransform;
    }

    public void SetCameraForEnemyTurn()
    {
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
}