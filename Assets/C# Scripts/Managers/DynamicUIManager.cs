using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DynamicUIManager : MonoBehaviour
{
    //This Manager controls UI elements that change dynamically based on the current state of the game
    //This generally does not control instantiation of UI Canvases.

    UIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        //edits the winning text to display the total amount of turns needed to win
        if(TurnManager.Instance.currentState == TurnManager.GameState.Win){
            float currentTurnNumber = TurnManager.Instance.TurnNumber;   
            string winText = "It took you " + currentTurnNumber + " turns!";
            uiManager.SetTextValueInUIElement("WinUI", "TurnNumberText", winText);
        }
    }
}
