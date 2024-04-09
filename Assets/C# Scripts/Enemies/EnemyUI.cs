using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using MoreMountains.Feedbacks;

public class EnemyUI : MonoBehaviour
{
    public GameObject enemyUIPrefab;
    private GameObject uiInstance;

    private EnemyProperties enemyProperties;
    private EnemyMovement enemyMovement;
    private TextMeshProUGUI healthText; 
    private TextMeshProUGUI moveMoneyText; 
    private Image fireIndicatorImage;
    

    private MMF_Player FeedbackHealthTextBounce;


    //spawns UI, assigns components,
    void Start()
    {
        AssignComponents();
    }

    void Update()
    {
        if(uiInstance!=null){
            uiInstance.transform.position = transform.position;

            healthText.text = enemyProperties.health.ToString();
            moveMoneyText.text = enemyMovement.moveMoney.ToString();
            
            fireIndicatorImage.enabled = enemyProperties.GetCurrentFireState();
        }



    }

    public void DestroyUI()
    {
        Debug.Log("Destroying ui");
        Destroy(uiInstance);
    }


    private void AssignComponents(){
        //UI GETTERS
        uiInstance = Instantiate(enemyUIPrefab, transform.position, Quaternion.identity);
        healthText = uiInstance.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        moveMoneyText = uiInstance.transform.Find("MoveMoneyText").GetComponent<TextMeshProUGUI>(); 
        fireIndicatorImage = uiInstance.transform.Find("FireIndicator").GetComponent<Image>();

        //SEPARATE SCRIPT GETTERS
        enemyProperties = GetComponent<EnemyProperties>();
        enemyMovement = GetComponent<EnemyMovement>();

        //FEEDBACK
        FeedbackHealthTextBounce = healthText.gameObject.GetComponentInChildren<MMF_Player>();

        if (enemyProperties == null) Debug.LogError("EnemyProperties script component not found.");
        if (enemyMovement == null) Debug.LogError("EnemyMovement script component not found.");
        if (fireIndicatorImage == null) Debug.LogError("FireIndicator Image component not found.");
    }

    public void FeedbackEnemyHealthTextBounce(){
        FeedbackHealthTextBounce.PlayFeedbacks();
    }

}
