using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private bool useGlobalSettings = true;
    public float currentHealth;
    
    [Header("UI References")]
    public Canvas healthBarCanvas;
    public Slider healthBarSlider;
    public Image healthBarFill;
    
    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    [Header("Health Bar Visuals")]
    public float healthBarHeight = 2f; // Высота над юнитом
    
    private bool isDead = false;
    
    // Events
    public System.Action<float> OnHealthChanged;
    public System.Action OnDeath;
    
    void Start()
    {
        float maxHealth = useGlobalSettings && GameSettings.Health != null ? GameSettings.Health.maxHealth : 100f;
        currentHealth = maxHealth;
        SetupHealthBar();
        UpdateHealthBar();
    }
    
    void Update()
    {
        // Обновляем позицию хелсбара, чтобы он следовал за юнитом
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.position = transform.position + Vector3.up * healthBarHeight;
        }
    }
    
void SetupHealthBar()
    {
        // Создаем Canvas для health bar если его нет
        if (healthBarCanvas == null)
        {
            GameObject canvasObj = new GameObject($"HealthBarCanvas_{gameObject.name}");
            healthBarCanvas = canvasObj.AddComponent<Canvas>();
            healthBarCanvas.renderMode = RenderMode.WorldSpace;
            
            // Создаем как дочерний объект юнита
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * healthBarHeight;
            canvasObj.transform.localRotation = Quaternion.identity;
            
            // ПЕРЕМЕЩАЕМ В КОРЕНЬ СЦЕНЫ - теперь он независим от поворота юнита
            canvasObj.transform.SetParent(null);
            
            // Настраиваем размер Canvas
            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1f, 0.1f);
            canvasRect.localScale = Vector3.one;
            
            // Настраиваем CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 1;
            scaler.scaleFactor = 1f;
            
            // Добавляем GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Добавляем HealthBarBillboard компонент для поворота к камере
            canvasObj.AddComponent<HealthBarBillboard>();
            
            // Добавляем HealthBarController для управления хелсбаром
            canvasObj.AddComponent<HealthBarController>();
        }
        
        // Создаем Slider для health bar если его нет
        if (healthBarSlider == null)
        {
            GameObject sliderObj = new GameObject("HealthBarSlider");
            sliderObj.transform.SetParent(healthBarCanvas.transform);
            healthBarSlider = sliderObj.AddComponent<Slider>();
            
            // Настраиваем RectTransform
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(1f, 0.1f);
            sliderRect.localPosition = Vector3.zero;
            sliderRect.localRotation = Quaternion.identity;
            
            // Создаем фон
            GameObject background = new GameObject("Background");
            background.transform.SetParent(sliderObj.transform);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.color = Color.gray;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = Vector2.zero;
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            
            // Создаем область заполнения
            GameObject fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(sliderObj.transform);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.sizeDelta = new Vector2(-10, 0);
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            
            // Создаем изображение заполнения
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform);
            healthBarFill = fill.AddComponent<Image>();
            healthBarFill.color = healthyColor;
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            
            healthBarSlider.fillRect = fillRect;
            healthBarSlider.targetGraphic = healthBarFill;
            healthBarSlider.value = 1f;
            healthBarSlider.minValue = 0f;
            healthBarSlider.maxValue = 1f;
            
            // Настраиваем HealthBarController
            HealthBarController healthBarController = healthBarCanvas.GetComponent<HealthBarController>();
            if (healthBarController != null)
            {
                healthBarController.healthSlider = healthBarSlider;
                healthBarController.fillImage = healthBarFill;
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        float maxHealth = useGlobalSettings && GameSettings.Health != null ? GameSettings.Health.maxHealth : 100f;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (isDead) return;
        
        float maxHealth = useGlobalSettings && GameSettings.Health != null ? GameSettings.Health.maxHealth : 100f;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;
        
        float maxHealth = useGlobalSettings && GameSettings.Health != null ? GameSettings.Health.maxHealth : 100f;
        float healthPercentage = currentHealth / maxHealth;
        healthBarSlider.value = healthPercentage;
        
        // Меняем цвет в зависимости от здоровья
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
    
    void Die()
    {
        isDead = true;
        
        // Уничтожаем health bar при смерти юнита
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
            healthBarCanvas = null;
        }
        
        // Отключаем компоненты юнита
        var controller = GetComponent<MonoBehaviour>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Делаем юнит неактивным
        gameObject.SetActive(false);
        
        Debug.Log($"{gameObject.name} died!");
        OnDeath?.Invoke();
    }
    
    void OnDestroy()
    {
        // Уничтожаем health bar при уничтожении юнита
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        float maxHealth = useGlobalSettings && GameSettings.Health != null ? GameSettings.Health.maxHealth : 100f;
        return currentHealth / maxHealth;
    }
    
    void OnDrawGizmosSelected()
    {
        // Показываем позицию health bar в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up * healthBarHeight, new Vector3(1f, 0.1f, 0.1f));
    }
}