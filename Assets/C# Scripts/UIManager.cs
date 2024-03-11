using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager2 : MonoBehaviour
{
    public static UIManager2 Instance { get; private set; }
    private UIFactory factory;

    //dictionary to hold all active UI elements
    private Dictionary<string, GameObject> activeUIElements = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        var uiDefinitions = Resources.Load<UIElementDefinitions>("UIElementDefinitions");
        factory = new UIFactory(uiDefinitions);
    }

    //UIManager.Instance.ShowUIElement("PauseMenuUI");

    public GameObject ShowUIElement(string elementType){
        if (activeUIElements.ContainsKey(elementType) && activeUIElements[elementType] != null)
        {
            activeUIElements[elementType].SetActive(true);
            return activeUIElements[elementType];
        }
        else
        {
            var uiElement = factory.CreateUIElement(elementType);
            activeUIElements[elementType] = uiElement;
            // Additional logic to manage UI elements (e.g., setting parent, positioning)
            return uiElement;
        }
    }

    public void HideUIElement(string elementType){
        if (activeUIElements.ContainsKey(elementType) && activeUIElements[elementType] != null)
        {
            activeUIElements[elementType].SetActive(false);
        }
    }

    public void DestroyUIElement(string elementType){
        if (activeUIElements.ContainsKey(elementType))
        {
            GameObject element = activeUIElements[elementType];
            if (element != null)
            {
                Destroy(element);
            }
            activeUIElements.Remove(elementType);
        }
    }

    public void HideAllUIElements(){
        foreach (var uiElement in activeUIElements.Values)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
    }

    public void DestroyAllUIElements(){
        foreach (var uiElement in new List<string>(activeUIElements.Keys))
        {
            DestroyUIElement(uiElement);
        }
    }
}
