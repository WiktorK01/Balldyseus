using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dossamer.Dialogue;
using Dossamer.Dialogue.Schema;

public class temptester : MonoBehaviour
{
    public Cutscene cutsceneToStart;
    public Cutscene cutsceneToEnd;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (DialogueManager.Instance != null && cutsceneToStart != null)
            {
                DialogueManager.Instance.StartNewDialogue(cutsceneToStart);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (DialogueManager.Instance != null && cutsceneToEnd != null)
            {
                DialogueManager.Instance.StartNewDialogue(cutsceneToEnd);
            }

        }


        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button is clicked
        {
            if (DialogueManager.Instance != null && 
                !DialogueManager.Instance.IsProgressionFrozen &&
                DialogueManager.Instance.GetIsDialogueActive())
            {
                DialogueManager.Instance.UpdateDialogue();
            }
        }
    }
}
