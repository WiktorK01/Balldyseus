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
            case "OuterUI":
                return Instantiate(definitions.outerUI);
            case "WinUI":
                return Instantiate(definitions.winUI);
            case "LossUI":
                return Instantiate(definitions.lossUI);
            case "PauseMenuUI":
                return Instantiate(definitions.pauseMenuUI);
            default:
                return null;
        }
    }

    private GameObject Instantiate(GameObject prefab)
    {
        return GameObject.Instantiate(prefab);
    }
}
