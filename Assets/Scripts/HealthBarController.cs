using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("Health Bar References")]
    public Image fillImage;
    public Slider healthSlider;
    
    [Header("Health Bar Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    
    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    [Header("Team Colors")]
    public Color playerHealthyColor = Color.green;
    public Color playerDamagedColor = Color.yellow;
    public Color playerCriticalColor = Color.red;
    public Color enemyHealthyColor = Color.blue;
    public Color enemyDamagedColor = Color.cyan;
    public Color enemyCriticalColor = Color.magenta;
    
    private Transform unitTransform;
    private HealthSystem healthSystem;
    
    void Awake()
    {
        // Сохраняем ссылку на юнит перед перемещением
        if (transform.parent != null)
        {
            unitTransform = transform.parent;
        }
        
        // Перемещаем хелсбар в корень сцены, чтобы он не крутился с юнитом
        transform.SetParent(null);
    }
    
    void Start()
    {
        // Находим компоненты если они не назначены
        if (fillImage == null)
        {
            fillImage = GetComponentInChildren<Image>();
        }
        
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }
        
        // Находим HealthSystem у юнита
        if (unitTransform != null)
        {
            healthSystem = unitTransform.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Подписываемся на изменения здоровья
                healthSystem.OnHealthChanged += OnHealthChanged;
            }
        }
        
        // Инициализируем health bar
        UpdateHealthBar();
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= OnHealthChanged;
        }
    }
    
    void OnHealthChanged(float healthPercentage)
    {
        // Обновляем хелсбар при изменении здоровья
        UpdateHealthBar();
    }

    void Update()
    {
        // Таскаем хелсбар за юнитом
        if (unitTransform != null)
        {
            transform.position = unitTransform.position + Vector3.up * 2f;
        }
        else
        {
            // Если юнит уничтожен, уничтожаем хелсбар
            Destroy(gameObject);
        }
    }
    

    
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthBar();
    }
    
    public void SetMaxHealth(float maxHp)
    {
        maxHealth = maxHp;
        currentHealth = maxHealth;
        UpdateHealthBar();
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
        // Получаем актуальное здоровье из HealthSystem
        float currentHp = currentHealth;
        float maxHp = maxHealth;
        
        if (healthSystem != null)
        {
            currentHp = healthSystem.currentHealth;
            // Получаем максимальное здоровье из настроек
            maxHp = 100f; // Можно получить из GameSettings.Health.maxHealth
        }
        
        if (healthSlider != null)
        {
            healthSlider.value = currentHp / maxHp;
        }
        
        if (fillImage != null)
        {
            float healthPercentage = currentHp / maxHp;
            
            // Определяем команду юнита
            bool isPlayer = unitTransform != null && unitTransform.GetComponent<UnitController>() != null && 
                           unitTransform.GetComponent<UnitController>().IsPlayerUnit();
            
            // Выбираем цвета в зависимости от команды и здоровья
            Color healthyCol, damagedCol, criticalCol;
            if (isPlayer)
            {
                healthyCol = playerHealthyColor;
                damagedCol = playerDamagedColor;
                criticalCol = playerCriticalColor;
            }
            else
            {
                healthyCol = enemyHealthyColor;
                damagedCol = enemyDamagedColor;
                criticalCol = enemyCriticalColor;
            }
            
            // Меняем цвет в зависимости от здоровья
            if (healthPercentage > 0.6f)
            {
                fillImage.color = healthyCol;
            }
            else if (healthPercentage > 0.3f)
            {
                fillImage.color = damagedCol;
            }
            else
            {
                fillImage.color = criticalCol;
            }
        }
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
}