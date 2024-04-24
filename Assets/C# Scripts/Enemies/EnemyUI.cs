using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    private EnemyProperties enemyProperties;
    private EnemyMovement enemyMovement;
    [SerializeField] TextMeshProUGUI healthText; 
    [SerializeField] TextMeshProUGUI moveMoneyText; 
    [SerializeField] Image fireIndicatorImage;

    void Awake()
    {
        enemyProperties = GetComponent<EnemyProperties>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        healthText.text = enemyProperties.health.ToString();
        moveMoneyText.text = enemyMovement.moveMoneyDecrement.ToString();

        fireIndicatorImage.enabled = enemyProperties.GetCurrentFireState(); //turns on or off based on enemy's fire state
    }

}
