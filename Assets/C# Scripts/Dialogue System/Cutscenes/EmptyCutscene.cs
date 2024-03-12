using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyCutscene : BaseCutscene
{

    private Coroutine cutsceneCoroutine;


    public override void StartCutscene()
    {
        cutsceneCoroutine = StartCoroutine(PlayCutsceneActions()); // Store the coroutine reference
    }

    protected override IEnumerator PlayCutsceneActions()
    {
        yield return new WaitForSeconds(.00000001f);
        EndCutsceneAndStartLevel();
    }

    void EndCutsceneAndStartLevel(){        
        TurnManager.Instance.ChangeGameState("PlayerTurn");
    }

}