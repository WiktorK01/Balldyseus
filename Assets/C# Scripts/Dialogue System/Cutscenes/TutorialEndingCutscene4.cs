using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEndingCutscene3 : BaseCutscene
{
    [SerializeField]DialogueManager DialogueManager;
    [SerializeField]GameObject Balldyseus;
    [SerializeField]GameObject StarUI;

    private Coroutine cutsceneCoroutine;

    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }

    protected override IEnumerator PlayCutsceneActions()
    {
        Debug.Log("Beginning Cutscene");
        yield return new WaitForSeconds(.5f);

        DialogueManager.AddDialogue("Elpenor", "Great job! I patched myself up, so we should go deeper into the city to help where we can!");
        DialogueManager.AddDialogue("Elpenor", "Ah! I almost forgot!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.5f);
        StarUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        StarUI.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        StarUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        StarUI.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        StarUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        DialogueManager.AddDialogue("Elpenor", "You may have noticed this symbol appear!");
        DialogueManager.AddDialogue("Elpenor", "This indicates that you are moving at a very high speed!");
        DialogueManager.AddDialogue("Elpenor", "If you Attack an Enemy while moving at High Speeds, you'll deal double-damage to them!");
        DialogueManager.AddDialogue("Elpenor", "Keep that in mind. Alright, let's move on now!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        
        EndCutsceneAndEndLevel();
    }

    void EndCutsceneAndStartLevel(){
        TurnManager.Instance.StartPlayerTurn();
    }
    void EndCutsceneAndEndLevel(){
        TurnManager.Instance.BeginHandlingWinCoroutine();
    }
}