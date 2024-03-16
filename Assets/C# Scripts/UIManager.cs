using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager2 : MonoBehaviour
{
    //UIManager2.Instance.ShowUIElement("PauseMenuUI");

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
            // Find the child GameObject by name.
            Transform textTransform = uiElement.transform.Find(textName);
            if (textTransform != null)
            {
                // Once the child is found, try to get the Text component.
                Text exampleText = textTransform.GetComponent<Text>();
                if (exampleText != null)
                {
                    // If the Text component is found, set its text value.
                    exampleText.text = newValue;
                }
                else
                {
                    // If the child does not have a Text component, log a warning.
                    Debug.LogWarning($"Text component not found on '{textName}' in UI element '{uiElementName}'.");
                }
            }
            else
            {
                // If no child with the given name is found, log a warning.
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
    public void InstantiatePlayerTurnUI(){
        HideAllUIElements();
        ShowUIElement("PlayerTurnUI");
        ShowUIElement("ImpulseCountUI");
        ShowUIElement("LaunchUI");
    }

    public void InstantiateEnemyTurnUI(){
        HideAllUIElements();
        ShowUIElement("EnemyTurnUI");
    }
}