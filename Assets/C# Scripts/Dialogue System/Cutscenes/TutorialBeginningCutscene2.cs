using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeginningCutscene2 : BaseCutscene
{
    [SerializeField]DialogueManager DialogueManager;
    [SerializeField]GameObject Balldyseus;

    private Coroutine cutsceneCoroutine;


    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }


    protected override IEnumerator PlayCutsceneActions()
    {
        Debug.Log("Beginning Cutscene");

        DialogueManager.AddDialogue("Elpenor", "Nice Job, Balldyseus! But the fight isn't over yet! Another one appeared! (PRESS SPACE TO SKIP CUTSCENES)");
        DialogueManager.AddDialogue("Elpenor", "He's awfully close to those flames, maybe you can shove him into it and light him on fire?");
        DialogueManager.AddDialogue("Elpenor", "If you click and hold with right-click instead of left-click, you can enter Shove-Mode! Hitting an enemy while in shove mode will push them!");
        DialogueManager.AddDialogue("Elpenor", "Shoving an enemy or a wall will also push you back!");
        DialogueManager.AddDialogue("Elpenor", "You have a limited number of shoves which resets every turn! You can view this on the top-left!");
        DialogueManager.AddDialogue("Elpenor", "Try it on him! And remember that you can click Reset if you mess up!");
        DialogueManager.AddDialogue("Narrator", "You will be unable to Left-Click for this section. Try to win using only Shove-Mode!");


        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.5f);
        
        EndCutsceneAndStartLevel();
    }

    void EndCutsceneAndStartLevel(){
        BallMovement BallMovement = Balldyseus.GetComponent<BallMovement>();
        BallMovement.GagAttack();
        
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
        if(Input.GetKeyDown(KeyCode.Space) && TurnManager.Instance.currentState == TurnManager.GameState.Cutscene){
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