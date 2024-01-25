using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeginningCutscene3 : BaseCutscene
{
    [SerializeField]DialogueManager DialogueManager;
    [SerializeField]GameObject Balldyseus;

    private Coroutine cutsceneCoroutine;

    bool cutsceneFinished = false;

    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }

    protected override IEnumerator PlayCutsceneActions()
    {
        Debug.Log("Beginning Cutscene");

        DialogueManager.AddDialogue("Elpenor", "Awesome! You're just about ready to go deeper into the battlefield!");
        DialogueManager.AddDialogue("Balldyseus", "...");
        DialogueManager.AddDialogue("Elpenor", "The last thing to learn is your ability to switch modes while moving!");
        DialogueManager.AddDialogue("Elpenor", "After activating a movement in one mode, you can freely switch between Attack Mode and Shove Mode however many times you want!");
        
        DialogueManager.AddDialogue("Elpenor", "So as a final recap, Left-Click for Attack-Mode which can hurt enemies");
        DialogueManager.AddDialogue("Elpenor", "Right-Click for Shove Mode which can shove enemies and increase your speed");
        DialogueManager.AddDialogue("Elpenor", "And you can switch between the two at any time during a movement.");
        
        DialogueManager.AddDialogue("Elpenor", "Now try out everything you've learned against these soldiers!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.5f);
        
        EndCutsceneAndStartLevel();
    }

    void EndCutsceneAndStartLevel(){
        BallMovement BallMovement = Balldyseus.GetComponent<BallMovement>();
        cutsceneFinished = true;
        TurnManager.Instance.StartPlayerTurn();
    }

    IEnumerator MoveObjectSmoothly(GameObject objectToMove, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objectToMove.transform.position;
        float time = 0;

        while (time < duration)
        {
            objectToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        objectToMove.transform.position = targetPosition; // Ensure final position is set
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && TurnManager.Instance.currentState == TurnManager.GameState.Cutscene && cutsceneFinished == false){
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        if (cutsceneCoroutine != null)
        {
            StopCoroutine(cutsceneCoroutine);
            cutsceneCoroutine = null;
        }
        else
        {
            StopAllCoroutines(); 
        }
        DialogueManager.ClearDialogueQueue();
        DialogueManager.TurnOffDialogue();
        SetEntitiesToFinalState();
        EndCutsceneAndStartLevel();
    }

    private void SetEntitiesToFinalState()
    {
        Balldyseus.SetActive(true);
    }

}