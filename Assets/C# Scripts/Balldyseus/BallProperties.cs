using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class BallProperties : MonoBehaviour
{
    Rigidbody2D rb;

    public bool bounceMode = false;

    static bool isMouseOverLaunchButton = false;
    
    public bool bounceGagged = false;
    public bool attackGagged = false;

    [Header("HighSpeed Variables")]
    [SerializeField] float highSpeedThreshold = 10f;
    [SerializeField] float delayToTurnOffHighSpeed = 0.7f;
    Coroutine resetHighSpeedCoroutine = null;

    [Header("LowSpeed Variables")]
    [SerializeField] float lowSpeedThreshold = 5f;

    

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if(bounceGagged) SetAttackMode();
        if(attackGagged) SetBounceMode();
        
        ManageSpeedState();
    }

    public void SetBounceMode(){
        if(!isMouseOverLaunchButton){
            bounceMode = true;
            BounceModePublisher.NotifyBounceModeChange(true);
        } 
        if(bounceGagged || isMouseOverLaunchButton) 
            return;
    }
    public void SetAttackMode(){
        if(!isMouseOverLaunchButton){
            bounceMode = false;
            BounceModePublisher.NotifyBounceModeChange(false);
        } 
        if(attackGagged || isMouseOverLaunchButton) 
            return;
    }

//Gagging related code**********************************************
    public void GagShove(){
        bounceGagged = true;
    }

    public void GagAttack(){
        attackGagged = true;
    }

    public void UngagAll(){
        bounceGagged = false;
        attackGagged = false;
    }

//Mouse Over Launch Button related code**********************************************


    //check if mouse is over the launch button to ensure no mode switching during that
    public static void MouseEnterLaunchButton(){
        isMouseOverLaunchButton = true;
    }

    public static void MouseExitLaunchButton(){
        isMouseOverLaunchButton = false;
    }

//speed related code**********************************************
    public enum SpeedState
    {
        Low,
        Normal,
        High,
    }
    public SpeedState currentSpeedState = SpeedState.Low;

    public void ChangeSpeedState(SpeedState newState)
    {
        if (newState == currentSpeedState) return;

        currentSpeedState = newState;
        SpeedStatePublisher.NotifySpeedStateChange(newState);
    }

    void ManageSpeedState()
    {
        if (HighVelocityCheck())
        {
            if (resetHighSpeedCoroutine != null)
            {
                StopCoroutine(resetHighSpeedCoroutine);
                resetHighSpeedCoroutine = null;
            }

            ChangeSpeedState(SpeedState.High);
        }
        else if (currentSpeedState == SpeedState.High)
        {
            if (resetHighSpeedCoroutine == null)
            {
                resetHighSpeedCoroutine = StartCoroutine(ResetHighSpeedAfterDelay(delayToTurnOffHighSpeed));
            }
        }
        else if (NormalVelocityCheck())
        {
            ChangeSpeedState(SpeedState.Normal);
        }
        else
        {
            ChangeSpeedState(SpeedState.Low);
        }
    }

    bool HighVelocityCheck(){
        return rb.velocity.magnitude > highSpeedThreshold;
    }

    bool LowVelocityCheck(){
        return rb.velocity.magnitude < lowSpeedThreshold;
    }

    bool NormalVelocityCheck(){
        return !HighVelocityCheck() && !LowVelocityCheck();
    }

    private IEnumerator ResetHighSpeedAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        if (!HighVelocityCheck())
        {
            ChangeSpeedState(SpeedState.Normal);
        }
        resetHighSpeedCoroutine = null; // Reset the coroutine reference
    }
}
