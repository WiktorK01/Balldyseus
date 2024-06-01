using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class AchillesFeedback : MonoBehaviour
{
    [SerializeField] GameObject rotatedObject;

    [SerializeField] MMF_Player rotationResetter;
    [SerializeField] MMF_Player rotate90;
    [SerializeField] MMF_Player rotate180;
    [SerializeField] MMF_Player rotate270;
    [SerializeField] MMF_Player rotate360;

    List<MMF_Rotation> rotationResetterFeedbacks;

    void Awake(){
        rotationResetterFeedbacks = rotationResetter.GetFeedbacksOfType<MMF_Rotation>();
    }

    public void ReassignRotationResetter(){
        Vector3 currentRotation = rotatedObject.transform.rotation.eulerAngles;

        if(Mathf.Approximately(currentRotation.z, 90)){
            SetRotationResetterRotationFeedbacks(90);
        }
        else if(Mathf.Approximately(currentRotation.z, 180)){
            SetRotationResetterRotationFeedbacks(180);
        }
        else if(Mathf.Approximately(currentRotation.z, 270)){
            SetRotationResetterRotationFeedbacks(270);
        }
        else{
            SetRotationResetterRotationFeedbacks(0);
        }
    }

    public void RandomRotation(){
        int randomNumber = Random.Range(1,4);

        if(randomNumber == 1) Rotate90();
        else if(randomNumber == 2) Rotate180();
        else if(randomNumber == 3) Rotate270();
        //else Rotate360();
    }

    public void Rotate90(){
        rotate90.Initialization();
        rotate90.PlayFeedbacks();
    }
    public void Rotate180(){
        rotate180.Initialization();
        rotate180.PlayFeedbacks();
    }
    public void Rotate270(){
        rotate270.Initialization();
        rotate270.PlayFeedbacks();
    }
    public void Rotate360(){
        rotate360.Initialization();
        rotate360.PlayFeedbacks();
    }

    public void Initializer(){
        rotate90.Initialization();
    }

    void SetRotationResetterRotationFeedbacks(float newValue){
        for(int i = 0; i < rotationResetterFeedbacks.Count; i++){
            rotationResetterFeedbacks[i].DestinationAngles = new Vector3 (0f,0f, newValue);
        }
        Initializer();
    }

// -----------------OBSERVERS------------------
    bool firstTurn = true;

    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
        EndEnemyTurnPublisher.EndEnemyTurn += OnEndEnemyTurn;
    }
    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
        EndEnemyTurnPublisher.EndEnemyTurn -= OnEndEnemyTurn;
    }

    void OnGameStateChange(TurnManager.GameState newGameState){
        if(newGameState == TurnManager.GameState.PlayerTurn && firstTurn == false)RandomRotation();
        else firstTurn = false;
    }

    void OnEndEnemyTurn(GameObject enemyWhoseTurnEnded){
        /*if(gameObject == enemyWhoseTurnEnded){
            RandomRotation();
        }*/
    }

}
