using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCutscene : MonoBehaviour
{
    protected bool actionsOccurring = false;

    public virtual void StartCutscene()
    {
        StartCoroutine(PlayCutsceneActions());
    }

    protected virtual IEnumerator PlayCutsceneActions()
    {
        yield return null;
    }

    
}