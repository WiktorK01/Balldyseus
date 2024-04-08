using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUI : MonoBehaviour
{
    public GameObject enemyUIPrefab;
    private GameObject uiInstance;

    private EnemyProperties EnemyProperties;
    private EnemyMovement EnemyMovement;
    private Text healthText;
    private Text moveMoneyText;
    private Image fireIndicatorImage;

    public Color damageTextColor = Color.magenta;

    [Header("Health Text Damage Animation")]
    [SerializeField] float healthTextDamageDuration = .2f;
    [SerializeField] float healthTextDamageScaleFactor = 3f;
    [SerializeField] Vector3 textTranslation = new Vector3(0, .5f, 0);

    //spawns UI, assigns components,
    void Start()
    {
        uiInstance = Instantiate(enemyUIPrefab, transform.position, Quaternion.identity);
        healthText = uiInstance.transform.Find("HealthText").GetComponent<Text>();
        moveMoneyText = uiInstance.transform.Find("MoveMoneyText").GetComponent<Text>();
        fireIndicatorImage = uiInstance.transform.Find("FireIndicator").GetComponent<Image>();
        EnemyProperties = GetComponent<EnemyProperties>();
        EnemyMovement = GetComponent<EnemyMovement>();

        if (EnemyProperties == null) Debug.LogError("EnemyProperties script component not found.");
        if (EnemyMovement == null) Debug.LogError("EnemyMovement script component not found.");
        if (fireIndicatorImage == null) Debug.LogError("FireIndicator Image component not found.");
    }

    void Update()
    {
        if(uiInstance!=null){
            uiInstance.transform.position = transform.position;

            healthText.text = EnemyProperties.health.ToString();
            moveMoneyText.text = EnemyMovement.moveMoney.ToString();
            
            fireIndicatorImage.enabled = EnemyProperties.GetCurrentFireState();
        }

        if (EnemyProperties.isThisEnemyTurn)
        {

        }
        else
        {

        }

    }

    public void DestroyUI()
    {
        Debug.Log("Destroying ui");
        Destroy(uiInstance);
    }


    /*************************ATTACK AND MOVE EFFECTS*******************************************/


    public void PerformAttackActionsUI(){
        StartCoroutine(AnimateHealthTextDamage());
    }

    public IEnumerator AnimateHealthTextDamage()
    {
        Vector3 originalPosition = healthText.transform.localPosition;
        Vector3 targetPosition = originalPosition + textTranslation;
        Color originalColor = healthText.color;
        Vector3 originalScale = healthText.transform.localScale;
        Vector3 targetScale = originalScale * healthTextDamageScaleFactor; 
        float halfDuration = healthTextDamageDuration / 2;

        // Enlarge, change color, and move
        float elapsed = 0;
        while (elapsed < halfDuration)
        {
            float t = Mathf.Clamp01(elapsed / halfDuration);
            healthText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            healthText.color = Color.Lerp(originalColor, damageTextColor, t);
            healthText.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original state
        elapsed = 0;
        while (elapsed < halfDuration)
        {
            float t = Mathf.Clamp01(elapsed / halfDuration);
            healthText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            healthText.color = Color.Lerp(damageTextColor, originalColor, t);
            healthText.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure everything is reset properly
        healthText.transform.localScale = originalScale;
        healthText.color = originalColor;
        healthText.transform.localPosition = originalPosition;
    }


}
