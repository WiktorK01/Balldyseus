using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string speakerName;
    public string dialogueText;


    public DialogueData(string speakerName, string dialogueText)
    {
        this.speakerName = speakerName;
        this.dialogueText = dialogueText;
    }
}

