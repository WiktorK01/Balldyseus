using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCutscene : BaseCutscene
{
    [SerializeField]DialogueManager DialogueManager;
    [SerializeField]GameObject Balldyseus;
    [SerializeField]GameObject Elpenor;
    [SerializeField]GameObject Enemy;

    [SerializeField]GameObject SpeechBubble;
    [SerializeField]GameObject S1;
    [SerializeField]GameObject S2;
    [SerializeField]GameObject S3;
    [SerializeField]GameObject S4;

    private Coroutine cutsceneCoroutine;

    void Start(){
        Balldyseus.gameObject.SetActive(false);
    }
    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }

    protected override IEnumerator PlayCutsceneActions()
    {
        Debug.Log("Beginning Cutscene");
        DialogueManager.AddDialogue("Narrator", "The setting takes place during the fall of troy. The ancient city is being burnt, looted and destroyed (CLICK TO CONTINUE, PRESS SPACE TO SKIP CUTSCENES)");
        DialogueManager.AddDialogue("Narrator", "Balldyseus, the leader of those instigating this chaos, finishes his nap inside the Trojan Horse (Currently represented by green spaces) He steps outside.");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(1f);
        Balldyseus.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoveObjectSmoothly(Balldyseus, new Vector3(17, 8, 0), 1f));
        yield return new WaitForSeconds(.5f);
        Elpenor.SetActive(true);
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(MoveObjectSmoothly(Elpenor, new Vector3(19, 12, 0), 1f));
        yield return new WaitForSeconds(.5f);             
        yield return StartCoroutine(MoveObjectSmoothly(Elpenor, new Vector3(18, 8, 0), .5f));             
        yield return new WaitForSeconds(.3f);

        DialogueManager.AddDialogue("Elpenor", "Balldyseus! Glad to see you woke up! Your whole 'Trojan Horse' plan has been a huge success!");
        DialogueManager.AddDialogue("Balldyseus", "...");
        DialogueManager.AddDialogue("Elpenor", "I'm falling back due to a wound, I guess I shouldn't be surprised since it's my first battle! But everyone else is still out there, fighting the good fight!");
        DialogueManager.AddDialogue("Balldyseus", "...");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.5f);
        Enemy.SetActive(true);
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(MoveObjectSmoothly(Enemy, new Vector3(17, 12, 0), .4f));

        DialogueManager.AddDialogue("Enemy", "Get back over here you bastard!");
        DialogueManager.AddDialogue("Elpenor", "Dang! Looks like an enemy followed me! You can take care of him though, right?");
        DialogueManager.AddDialogue("Balldyseus", "...");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        SpeechBubble.SetActive(true);
        S1.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "Great! To attack, click and hold your character");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S1.SetActive(false);
        S2.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "Then drag your mouse to aim at the enemy!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S2.SetActive(false);
        S3.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "You can see how much force you'll be launching yourself via the indicator on the bottom-right");
        DialogueManager.AddDialogue("Elpenor", "Then you can click the Launch-Button to attack!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S3.SetActive(false);
        S4.SetActive(true);
        DialogueManager.AddDialogue("Elpenor", "The enemy's red number represents their health, while their blue number represents how many spaces they'll move on their turn!");
        DialogueManager.AddDialogue("Elpenor", "Every turn after you attack, the enemy will be moving to reach the green-tiles");
        DialogueManager.AddDialogue("Elpenor", "If they reach it, you'll lose!");
        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());
        yield return new WaitForSeconds(.00f);
        S4.SetActive(false);
        SpeechBubble.SetActive(false);
        DialogueManager.AddDialogue("Elpenor", "I'll be patching myself up inside the horse! Don't let him reach it or I'm a goner!");  

        yield return new WaitUntil(() => DialogueManager.IsDialogueAcknowledged());

        yield return new WaitForSeconds(.3f);
        yield return StartCoroutine(MoveObjectSmoothly(Elpenor, new Vector3(18, 6, 0), .6f));             
        yield return new WaitForSeconds(.3f);
        Elpenor.SetActive(false);
        yield return new WaitForSeconds(.4f);

        EndCutsceneAndStartLevel();
    }

    void EndCutsceneAndStartLevel(){
        BallProperties BallProperties = Balldyseus.GetComponent<BallProperties>();
        BallProperties.GagShove();
        
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
        Balldyseus.transform.position = new Vector3(17, 8, 0); 
        Enemy.transform.position = new Vector3(17, 12, 0);
        Balldyseus.SetActive(true);
        Enemy.SetActive(true);
        Elpenor.SetActive(false);
    }



}