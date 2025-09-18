using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float healthBarHeight = 2f;
    
    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    private Canvas healthBarCanvas;
    private Slider healthBarSlider;
    private Image healthBarFill;
    
    void Start()
    {
        CreateHealthBar();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    void Update()
    {
        // Просто таскаем хелсбар за юнитом
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.position = transform.position + Vector3.up * healthBarHeight;
        }
    }
    
    void CreateHealthBar()
    {
        // Создаем Canvas
        GameObject canvasObj = new GameObject($"HealthBar_{gameObject.name}");
        healthBarCanvas = canvasObj.AddComponent<Canvas>();
        healthBarCanvas.renderMode = RenderMode.WorldSpace;
        
        // Создаем как дочерний объект
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.up * healthBarHeight;
        canvasObj.transform.localRotation = Quaternion.identity;
        
        // СРАЗУ ПЕРЕМЕЩАЕМ В КОРЕНЬ СЦЕНЫ
        canvasObj.transform.SetParent(null);
        
        // Настраиваем Canvas
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1f, 0.1f);
        canvasRect.localScale = Vector3.one;
        
        // CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1;
        scaler.scaleFactor = 1f;
        
        // GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Создаем Slider
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(canvasObj.transform);
        healthBarSlider = sliderObj.AddComponent<Slider>();
        
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(1f, 0.1f);
        sliderRect.localPosition = Vector3.zero;
        sliderRect.localRotation = Quaternion.identity;
        
        // Создаем Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = Color.gray;
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.sizeDelta = Vector2.zero;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        
        // Создаем FillArea
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(sliderObj.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.sizeDelta = new Vector2(-10, 0);
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        
        // Создаем Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        healthBarFill = fill.AddComponent<Image>();
        healthBarFill.color = healthyColor;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        
        // Настраиваем Slider
        healthBarSlider.fillRect = fillRect;
        healthBarSlider.targetGraphic = healthBarFill;
        healthBarSlider.value = 1f;
        healthBarSlider.minValue = 0f;
        healthBarSlider.maxValue = 1f;
        
        // Добавляем billboard эффект
        canvasObj.AddComponent<HealthBarBillboard>();
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;
        
        float healthPercentage = currentHealth / maxHealth;
        healthBarSlider.value = healthPercentage;
        
        if (healthBarFill != null)
        {
            if (healthPercentage > 0.6f)
            {
                healthBarFill.color = healthyColor;
            }
            else if (healthPercentage > 0.3f)
            {
                healthBarFill.color = damagedColor;
            }
            else
            {
                healthBarFill.color = criticalColor;
            }
        }
    }
    
    void OnDestroy()
    {
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
        }
    }
}
