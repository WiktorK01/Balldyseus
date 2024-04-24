using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class BallProperties : MonoBehaviour
{
    BallFeedback ballFeedback;
    Rigidbody2D rb;

    static bool IsMouseOverLaunchButton = false;

    public bool ShoveMode = false;

    public bool ShoveGagged = false;
    public bool AttackGagged = false;

    [Header("HighSpeed Variables")]
    public bool HighSpeed;
    public float highSpeedThreshold = 10f;
    public float delayToTurnOffHighSpeed = 0.7f;
    Coroutine resetHighSpeedCoroutine = null;

    [Header("LowSpeed Variables")]
    public bool LowSpeed;
    public float lowSpeedThreshold = 5f;

    void Awake(){
        ballFeedback = GetComponent<BallFeedback>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if(ShoveGagged) SetAttackMode();
        if(AttackGagged) SetShoveMode();
        CheckForHighSpeed();
        CheckForLowSpeed();
    }

    public void SetShoveMode(){
        if(!IsMouseOverLaunchButton) ballFeedback.ChangeModes();
        if(ShoveGagged || IsMouseOverLaunchButton) 
            return;
        ShoveMode = true;
    }

    public void SetAttackMode(){
        if(!IsMouseOverLaunchButton) ballFeedback.ChangeModes();
        if(AttackGagged || IsMouseOverLaunchButton) 
            return;
        ShoveMode = false;
    }

    //High speed related code**********************************************

    private void CheckForHighSpeed(){
        if(SpeedGreaterThanThreshold())
        {
            if (resetHighSpeedCoroutine != null)
            {
                StopCoroutine(resetHighSpeedCoroutine);
                resetHighSpeedCoroutine = null;
            }

            HighSpeed = true;
        }

        //this will ensure that HighSpeed won't immediately disappear after going below the threshold, giving the player extra time to react 
        if(HighSpeed && !SpeedGreaterThanThreshold() && resetHighSpeedCoroutine == null)
        {
            resetHighSpeedCoroutine = StartCoroutine(ResetHighSpeedAfterDelay(delayToTurnOffHighSpeed));
        }
    }

    bool SpeedGreaterThanThreshold(){
        return rb.velocity.magnitude > highSpeedThreshold;
    }

    private IEnumerator ResetHighSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HighSpeed = false;
        resetHighSpeedCoroutine = null; // Reset the coroutine reference
    }

    void CheckForLowSpeed(){
        if(rb.velocity.magnitude < lowSpeedThreshold) LowSpeed = true;
        else LowSpeed = false;
    }

    //Gagging related code**********************************************
    public void GagShove(){
        ShoveGagged = true;
    }

    public void GagAttack(){
        AttackGagged = true;
    }

    public void UngagAll(){
        ShoveGagged = false;
        AttackGagged = false;
    }

    //check if mouse is over the launch button to ensure no mode switching during that
    public static void MouseEnterLaunchButton(){
        IsMouseOverLaunchButton = true;
    }

    public static void MouseExitLaunchButton(){
        IsMouseOverLaunchButton = false;
    }

}
