using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFactory
{
    private UIElementDefinitions definitions;

    public UIFactory(UIElementDefinitions uiDefinitions)
    {
        definitions = uiDefinitions;
    }

    public GameObject CreateUIElement(string elementType)
    {
        switch (elementType)
        {

            case "WinUI":
                return Instantiate(definitions.winUI);
            case "LossUI":
                return Instantiate(definitions.lossUI);
            case "PauseMenuUI":
                return Instantiate(definitions.pauseMenuUI);
            case "HighSpeedUI":
                return Instantiate(definitions.highSpeedUI);
            case "ImpulseCountUI":
                return Instantiate(definitions.impulseCountUI);
            case "LaunchUI":
                return Instantiate(definitions.launchUI);
            case "EnemyTurnUI":
                return Instantiate(definitions.enemyTurnUI);
            case "PlayerTurnUI":
                return Instantiate(definitions.playerTurnUI);
            default:
                return null;
        }
    }

    private GameObject Instantiate(GameObject prefab)
    {
        return GameObject.Instantiate(prefab);
    }
}
