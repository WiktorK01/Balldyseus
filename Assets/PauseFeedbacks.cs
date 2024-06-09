using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PauseFeedbacks : MonoBehaviour
{
    [SerializeField] MMF_Player circleEntrances;
    [SerializeField] MMF_Player circleExits;

    public void CircleEntrances(){
        circleEntrances.Initialization();
        circleEntrances.PlayFeedbacks();
    }

    public void CircleExits(){
        circleExits.Initialization();
        circleExits.PlayFeedbacks();
    }


    //******OBSERVERS********

    void OnEnable(){
        PausePublisher.PauseChange += OnPauseChange;
    }
    void OnDisable(){
        PausePublisher.PauseChange -= OnPauseChange;
    }

    private void OnPauseChange(bool isGamePaused){
        if (isGamePaused){
            CircleEntrances();
        }
        else{
            CircleExits();
        }
    }
}
