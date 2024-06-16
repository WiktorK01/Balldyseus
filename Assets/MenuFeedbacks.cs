using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MenuFeedbacks : MonoBehaviour
{
    bool isWinLossMenuOpen = false;

    enum MenuType{
        Pause,
        Win,
        Loss
    }
    [SerializeField] MenuType menuType;

    [SerializeField] MMF_Player lossEntrances;
    [SerializeField] MMF_Player winEntrances;
    [SerializeField] MMF_Player pauseEntrances;

    [SerializeField] MMF_Player exits;

    [SerializeField] MMF_Player resetButtonHover;
    [SerializeField] MMF_Player resetButtonHoverExit;
    [SerializeField] MMF_Player optionsButtonHover;
    [SerializeField] MMF_Player optionsButtonHoverExit;
    [SerializeField] MMF_Player mainMenuButtonHover;
    [SerializeField] MMF_Player mainMenuButtonHoverExit;
    [SerializeField] MMF_Player nextLevelButtonHover;
    [SerializeField] MMF_Player nextLevelButtonHoverExit;

    [SerializeField] MMF_Player winUiTurnCountUpdate;
    MMF_TMPText winUiTurnCountText;

    public void LossEntrances(){
        if(isWinLossMenuOpen) return;
        WinLossMenuOpenedPublisher.NotifyWinLossMenuOpen(true);
        lossEntrances.Initialization();
        lossEntrances.PlayFeedbacks();
    }
    public void WinEntrances(){
        if(isWinLossMenuOpen) return;
        WinLossMenuOpenedPublisher.NotifyWinLossMenuOpen(true);
        winEntrances.Initialization();
        winEntrances.PlayFeedbacks();
    }
    public void PauseEntrances(){
        if(isWinLossMenuOpen) return;
        pauseEntrances.Initialization();
        pauseEntrances.PlayFeedbacks();
    }
    public void Exits(){
        if(isWinLossMenuOpen) return;
        exits.Initialization();
        exits.PlayFeedbacks();
    }

    public void ResetButtonHover(){
        resetButtonHover.Initialization();
        resetButtonHover.PlayFeedbacks();
    }
    public void ResetButtonHoverExit(){
        resetButtonHover.StopFeedbacks();

        resetButtonHoverExit.Initialization();
        resetButtonHoverExit.PlayFeedbacks();
    }

    public void OptionsButtonHover(){
        optionsButtonHover.Initialization();
        optionsButtonHover.PlayFeedbacks();
    }
    public void OptionsButtonHoverExit(){
        optionsButtonHover.StopFeedbacks();

        optionsButtonHoverExit.Initialization();
        optionsButtonHoverExit.PlayFeedbacks();
    }

    public void MainMenuButtonHover(){
        mainMenuButtonHover.Initialization();
        mainMenuButtonHover.PlayFeedbacks();
    }
    public void MainMenuButtonHoverExit(){
        mainMenuButtonHover.StopFeedbacks();

        mainMenuButtonHoverExit.Initialization();
        mainMenuButtonHoverExit.PlayFeedbacks();
    }

    public void NextLevelButtonHover(){
        nextLevelButtonHover.Initialization();
        nextLevelButtonHover.PlayFeedbacks();
    }
    public void NextLevelButtonHoverExit(){
        nextLevelButtonHover.StopFeedbacks();

        nextLevelButtonHoverExit.Initialization();
        nextLevelButtonHoverExit.PlayFeedbacks();
    }

    int turnCount = -1;
    public void WinUiTurnCountUpdate(){
        if(turnCount == 1) winUiTurnCountText.NewText = "It Took You " + turnCount + " Turn to Win!";
        else winUiTurnCountText.NewText = "It Took You " + turnCount + " Turns to Win!";
        winUiTurnCountUpdate.Initialization();
        winUiTurnCountUpdate.PlayFeedbacks();
    }

    //******OBSERVERS********


    void OnEnable(){
        if(winUiTurnCountUpdate != null) winUiTurnCountText = winUiTurnCountUpdate.GetFeedbackOfType<MMF_TMPText>();
        PausePublisher.PauseChange += OnPauseChange;
        GameStatePublisher.GameStateChange += OnGameStateChange;
        WinLossMenuOpenedPublisher.WinLossMenuOpened += OnWinLossMenuOpened;
    }
    void OnDisable(){
        PausePublisher.PauseChange -= OnPauseChange;
        GameStatePublisher.GameStateChange -= OnGameStateChange;
        WinLossMenuOpenedPublisher.WinLossMenuOpened -= OnWinLossMenuOpened;
    }

    private void OnPauseChange(bool isGamePaused){
        if(menuType == MenuType.Pause){
            if (isGamePaused)
                PauseEntrances();
            else 
                Exits();
        }
    }

    private void OnGameStateChange(TurnManager.GameState newState){
        switch (newState){
            case TurnManager.GameState.Win:
                if(menuType == MenuType.Win) WinEntrances();
                break;
            
            case TurnManager.GameState.Loss:
                if(menuType == MenuType.Loss) LossEntrances();
                break;
            case TurnManager.GameState.PlayerTurn:
                if(menuType == MenuType.Win){
                    turnCount++;
                    WinUiTurnCountUpdate();
                }
                break;

        }
    }

    private void OnWinLossMenuOpened(bool newIsWinLossMenuOpen){
        if(newIsWinLossMenuOpen) isWinLossMenuOpen = true;
        else isWinLossMenuOpen = false;
    }
}
