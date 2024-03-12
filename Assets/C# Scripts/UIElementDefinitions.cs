using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIElementDefinitions", menuName = "UI/UI Element Definitions")]
public class UIElementDefinitions : ScriptableObject
{
    public GameObject winUI;
    public GameObject lossUI;
    public GameObject pauseMenuUI;
    public GameObject highSpeedUI;
    public GameObject impulseCountUI;
    public GameObject launchUI;
    public GameObject enemyTurnUI;
    public GameObject playerTurnUI;
}
