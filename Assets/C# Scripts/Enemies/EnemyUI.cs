using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    private EnemyProperties enemyProperties;
    private EnemyMovement enemyMovement;
    private TextMeshProUGUI healthText; 
    private TextMeshProUGUI moveMoneyText; 
    private Image fireIndicatorImage;

    void Start()
    {
        AssignComponents();
    }

    void Update()
    {
        healthText.text = enemyProperties.health.ToString();
        moveMoneyText.text = enemyMovement.moveMoney.ToString();
        fireIndicatorImage.enabled = enemyProperties.GetCurrentFireState();
    }

    private void AssignComponents()
    {
        healthText = transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        moveMoneyText = transform.Find("MoveMoneyText").GetComponent<TextMeshProUGUI>(); 
        fireIndicatorImage = transform.Find("FireIndicator").GetComponent<Image>();

        enemyProperties = GetComponent<EnemyProperties>();
        enemyMovement = GetComponent<EnemyMovement>();
    }
}
