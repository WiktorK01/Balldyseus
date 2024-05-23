using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private UIFactory factory;

    //dictionary to hold all active UI elements
    private Dictionary<string, GameObject> activeUIElements = new Dictionary<string, GameObject>();
    //singleton
    void Awake()
    {
        var uiDefinitions = Resources.Load<UIElementDefinitions>("UIElementDefinitions");
        factory = new UIFactory(uiDefinitions);
        ShowGameplayUI();
    }

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
    
    ////////////////////////////////////////////////////////////////////////////////
    /// 

    public void SetTextValueInUIElement(string uiElementName, string textName, string newValue)
    {
        if(activeUIElements.TryGetValue(uiElementName, out GameObject uiElement))
        {
            Transform textTransform = uiElement.transform.Find(textName);
            if (textTransform != null)
            {
                TMP_Text exampleText = textTransform.GetComponent<TMP_Text>();
                if (exampleText != null)
                {
                    exampleText.text = newValue;
                }
                else
                {
                    Debug.LogWarning($"TMP_Text component not found on '{textName}' in UI element '{uiElementName}'.");
                }
            }
            else
            {
                Debug.LogWarning($"Child named '{textName}' not found in UI element '{uiElementName}'.");
            }
        }
    }

    public void SetElementActiveState(string uiElementName, string childElementName, bool isActive)
    {
        if (activeUIElements.TryGetValue(uiElementName, out GameObject uiElement))
        {
            if (string.IsNullOrEmpty(childElementName))
            {
                uiElement.SetActive(isActive);
            }
            else
            {
                Transform childTransform = uiElement.transform.Find(childElementName);
                if (childTransform != null)
                {
                    childTransform.gameObject.SetActive(isActive);
                }
                else
                {
                    Debug.LogWarning($"Child element '{childElementName}' not found in UI element '{uiElementName}'.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"UI element '{uiElementName}' not found.");
        }
    }


    ////////////////////////////////////////////////////////////////////////////////
    /// 

    void OnEnable()
    {
        //subscribes
        GameStatePublisher.GameStateChange += UpdateUI;
    }

    void OnDisable()
    {
        //unsubscribes to avoid memory leaks
        GameStatePublisher.GameStateChange -= UpdateUI;
    }

    private void UpdateUI(TurnManager.GameState newState)
    {
        HideUIElement("PauseMenuUI");
        

        switch (newState)
        {
            /*case TurnManager.GameState.PlayerTurn:
                ShowPlayerTurnUI();
                break;
            case TurnManager.GameState.EnemyTurn:
                ShowEnemyTurnUI();
                break; ideally these will be handled by their own feedback animations*/
            case TurnManager.GameState.Win:
                ShowWinUI();
                break;
            case TurnManager.GameState.Loss:
                ShowLossUI();
                break;
        }

        
    }

    //we call this in Awake so that the gameplay UI exists to perform their feedbacks after a turn happens
    public void ShowGameplayUI(){
        ShowUIElement("LaunchUI");
    }
    void ShowWinUI(){ 
        ShowUIElement("WinUI");
    }
    void ShowLossUI(){
        ShowUIElement("LossUI");
    }
}
