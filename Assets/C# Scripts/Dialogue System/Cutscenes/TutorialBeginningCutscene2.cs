using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeginningCutscene2 : BaseCutscene
{
    [SerializeField]DialogueManager DialogueManager;
    [SerializeField]GameObject Balldyseus;

    private Coroutine cutsceneCoroutine;
    [SerializeField]GameObject SpeechBubble;
    [SerializeField]GameObject S1;
    [SerializeField]GameObject S2;
    [SerializeField]GameObject S3;

    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }


    protected override IEnumerator PlayCutsceneActions()
    {
        Debug.Log("Beginning Cutscene");

        DialogueManager.AddDialogue("Elpenor", "Nice Job, Balldyseus! But the fight isn't over yet! Another one appeared!");
        DialogueManager.AddDialogue("Elpenor", "He's awfully close to those flames, maybe you can shove him into it and light him on fire?");
        DialogueManager.AddDialogue("Elpenor", "An enemy on-fire will take damage at the start of each turn!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        SpeechBubble.SetActive(true);
        S1.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "If you click and hold with right-click instead of left-click, you can enter Shove-Mode!");
        DialogueManager.AddDialogue("Elpenor", "Hitting an enemy while in shove mode will push them back! But it won't damage them unless you're pushing them into a wall!");
        DialogueManager.AddDialogue("Elpenor", "Shoving an enemy or a wall will also push you back! Increasing your velocity!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S1.SetActive(false);
        S2.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "You have a limited number of shoves which resets every turn! You can view this on the top-left!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S2.SetActive(false);
        S3.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "Try it on him! If you mess up, press Esc to pause & reset the level!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        SpeechBubble.SetActive(false);
        S3.SetActive(false);
        DialogueManager.AddDialogue("Narrator", "You will be unable to Left-Click for this section. Try to win using only Right-click's Shove-mode!");


        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.5f);
        
        EndCutsceneAndStartLevel();
    }

    void EndCutsceneAndStartLevel(){
        BallProperties BallProperties = Balldyseus.GetComponent<BallProperties>();
        BallProperties.GagAttack();
        
        TurnManager.Instance.ChangeGameState("PlayerTurn");
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