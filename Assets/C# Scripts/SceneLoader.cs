using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;

public class SceneLoader : MonoBehaviour
{
    //EVERY scene will have a screen wipe start. as such, this object needs to be in every scene.
    [SerializeField] GameObject ScreenWipeUI;
    [SerializeField] MMF_Player screenWipeSceneStart;
    [SerializeField] MMF_Player screenWipeSceneEnd;

    enum SceneToBeLoaded {Next, Current, TitleScreen, SceneName};
    SceneToBeLoaded sceneToBeLoaded = SceneToBeLoaded.Current;

    string sceneName;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            LoadCurrentScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)){
            LoadTitleScreen();
        }
    }

    public void ScreenWipeSceneStart(){
        screenWipeSceneStart.Initialization();
        screenWipeSceneStart.PlayFeedbacks();
    }

    public void ScreenWipeSceneEnd(){
        screenWipeSceneEnd.Initialization();
        screenWipeSceneEnd.PlayFeedbacks();
    }

    void Awake(){
        ScreenWipeUI.SetActive(true);
    }
    void Start(){
        ScreenWipeSceneStart();
    }

    public void LoadCurrentScene(){
        sceneToBeLoaded = SceneToBeLoaded.Current;
        ScreenWipeSceneEnd();
    }

    public void LoadNextScene(){
        sceneToBeLoaded = SceneToBeLoaded.Next;
        ScreenWipeSceneEnd();
    }

    public void LoadTitleScreen(){
        sceneToBeLoaded = SceneToBeLoaded.TitleScreen;
        ScreenWipeSceneEnd();
    }

    public void LoadScene(string newSceneName){
        sceneToBeLoaded = SceneToBeLoaded.SceneName;
        sceneName = newSceneName;
        ScreenWipeSceneEnd();
    }

    public void LoadDesignatedScene(){
        switch(sceneToBeLoaded){
            case SceneToBeLoaded.Next:
               int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
                int nextSceneIndex = currentSceneIndex + 1;
                
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings){
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else{
                    Debug.Log("No more scenes to load");
                }                
                break;
            case SceneToBeLoaded.Current:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case SceneToBeLoaded.TitleScreen:
                SceneManager.LoadScene("TitleScreen");
                break;
            case SceneToBeLoaded.SceneName:
                SceneManager.LoadScene(sceneName);
                break;
        }
    }
}
