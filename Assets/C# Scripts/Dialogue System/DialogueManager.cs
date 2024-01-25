using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class DialogueManager : MonoBehaviour
{
    public Canvas dialogueCanvas; 

    public Text nameText;
    public Text dialogueText; 

    public GameObject dialoguePanel;
    public GameObject namePanel;
    public GameObject backgroundDimmerPanel;
    public GameObject ElpenorPortrait;
    public GameObject BalldyseusPortrait;
    public GameObject SoldierPortrait;
    
    private bool isDialogueAcknowledged = false;

    private Queue<DialogueData> dialogueQueue = new Queue<DialogueData>();

    void Awake(){
        TurnOffDialogue();
    }

    void Update(){
        StartDialogueWhenQueueIsntEmpty();
        AwaitInputToContinueDialogue();
        CheckForPortraits();
    }

    void StartDialogueWhenQueueIsntEmpty()
    {
        isDialogueAcknowledged = false;
        if(dialogueCanvas.enabled == false && dialogueQueue.Count > 0)
        {
            DisplayNextDialogue();
        }
    }

    void AwaitInputToContinueDialogue(){
        if(Input.GetMouseButtonDown(0) && dialogueQueue.Count > 0)
        {
            DisplayNextDialogue();
        }
        else if(Input.GetMouseButtonDown(0) && dialogueQueue.Count == 0 && dialogueCanvas.enabled)
        {
            TurnOffDialogue();
            isDialogueAcknowledged = true;
        }
    }

    void DisplayNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            TurnOffDialogue();
            return;
        }

        DialogueData dialogueData = dialogueQueue.Dequeue();
        TurnOnDialogue();
        nameText.text = dialogueData.speakerName;
        dialogueText.text = dialogueData.dialogueText;
        isDialogueAcknowledged = false;
    }

    public void AddDialogue(string speakerName, string dialogueText)
    {
        DialogueData data = new DialogueData(speakerName, dialogueText);
        dialogueQueue.Enqueue(data);
    }

    //this will be updated for better portrait functionality in the future
    public void CheckForPortraits(){
        if(dialogueCanvas.enabled == true && nameText.text == "Elpenor"){
            ElpenorPortrait.gameObject.SetActive(true);
        }
        else{
            ElpenorPortrait.gameObject.SetActive(false);
        }
        if(dialogueCanvas.enabled == true && nameText.text == "Balldyseus"){
            BalldyseusPortrait.gameObject.SetActive(true);
        }
        else{
            BalldyseusPortrait.gameObject.SetActive(false);
        }
        if(dialogueCanvas.enabled == true && nameText.text == "Enemy"){
            SoldierPortrait.gameObject.SetActive(true);
        }
        else{
            SoldierPortrait.gameObject.SetActive(false);
        }
    }

    void TurnOnDialogue(){
        dialogueCanvas.enabled = true;
    }

    public void TurnOffDialogue(){
        dialogueCanvas.enabled = false;
    }

    public int DialogueQueueCount(){
        return dialogueQueue.Count;
    }

    public bool IsDialogueAcknowledged(){
        return isDialogueAcknowledged;
    }
    
    public void ClearDialogueQueue()
    {
        dialogueQueue.Clear();
    }
}