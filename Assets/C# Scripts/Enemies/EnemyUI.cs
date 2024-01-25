using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public GameObject enemyUIPrefab;
    private GameObject uiInstance;

    private EnemyProperties EnemyProperties;
    private EnemyMovement EnemyMovement;
    private Text healthText;
    private Text moveMoneyText;
    private Image fireIndicatorImage;

    private Vector3 uiOffset = new Vector3(-0.15f, .4f, 0); 

    void Start()
    {
        uiInstance = Instantiate(enemyUIPrefab, transform.position + uiOffset, Quaternion.identity);

        healthText = uiInstance.transform.Find("HealthText").GetComponent<Text>();
        moveMoneyText = uiInstance.transform.Find("MoveMoneyText").GetComponent<Text>();

        EnemyProperties = GetComponent<EnemyProperties>();
        EnemyMovement = GetComponent<EnemyMovement>();

        if (EnemyProperties == null)
        {
            Debug.LogError("EnemyProperties component not found.");
        }
        if (EnemyMovement == null)
        {
            Debug.LogError("EnemyMovement component not found.");
        }

        fireIndicatorImage = uiInstance.transform.Find("FireIndicator").GetComponent<Image>();

        if (fireIndicatorImage == null)
        {
            Debug.LogError("FireIndicator Image component not found.");
        }
    }

    void Update()
    {
        if(uiInstance!=null){
            uiInstance.transform.position = transform.position + uiOffset;

            healthText.text = EnemyProperties.health.ToString();
            moveMoneyText.text = EnemyMovement.moveMoney.ToString();
            
            fireIndicatorImage.enabled = EnemyProperties.GetCurrentFireState();
        }

    }

    public void DestroyUI()
    {
        Debug.Log("Destroying ui");
        Destroy(uiInstance);
    }
}