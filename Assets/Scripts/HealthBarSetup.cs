using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthBarSetup : MonoBehaviour
{
    [Header("Health Bar References")]
    public Canvas healthBarCanvas;
    public Slider healthBarSlider;
    public Image healthBarFill;
    public Image healthBarBackground;
    
    [Header("Health Bar Settings")]
    public float healthBarHeight = 2f;
    public Vector2 healthBarSize = new Vector2(1f, 0.1f);
    
    void Start()
    {
        SetupHealthBar();
    }
    
    void Update()
    {
        // Обновляем позицию хелсбара, чтобы он следовал за юнитом
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.position = transform.position + Vector3.up * healthBarHeight;
        }
    }
    
    public void SetupHealthBar()
    {
        // Создаем Canvas если его нет
        if (healthBarCanvas == null)
        {
            GameObject canvasObj = new GameObject($"HealthBarCanvas_{gameObject.name}");
            
            // Создаем как дочерний объект юнита
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * healthBarHeight;
            canvasObj.transform.localRotation = Quaternion.identity;
            
            // ПЕРЕМЕЩАЕМ В КОРЕНЬ СЦЕНЫ - теперь он независим от поворота юнита
            canvasObj.transform.SetParent(null);
            
            healthBarCanvas = canvasObj.AddComponent<Canvas>();
            healthBarCanvas.renderMode = RenderMode.WorldSpace;
            
            // Настраиваем Canvas для billboard эффекта
            healthBarCanvas.worldCamera = Camera.main;
            
            // Настраиваем CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 1;
            scaler.scaleFactor = 1f;
            
            // Добавляем GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Добавляем HealthBarBillboard компонент для поворота к камере
            canvasObj.AddComponent<HealthBarBillboard>();
            
            // Настраиваем RectTransform
            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = healthBarSize;
            canvasRect.localScale = Vector3.one;
        }
        
        // Создаем Slider если его нет
        if (healthBarSlider == null)
        {
            GameObject sliderObj = new GameObject("HealthBarSlider");
            sliderObj.transform.SetParent(healthBarCanvas.transform);
            sliderObj.transform.localPosition = Vector3.zero;
            sliderObj.transform.localRotation = Quaternion.identity;
            
            healthBarSlider = sliderObj.AddComponent<Slider>();
            
            // Настраиваем RectTransform
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.sizeDelta = healthBarSize;
            sliderRect.localPosition = Vector3.zero;
            sliderRect.localRotation = Quaternion.identity;
        }
        
        // Создаем Background если его нет
        if (healthBarBackground == null)
        {
            GameObject backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(healthBarSlider.transform);
            backgroundObj.transform.localPosition = Vector3.zero;
            backgroundObj.transform.localRotation = Quaternion.identity;
            
            healthBarBackground = backgroundObj.AddComponent<Image>();
            healthBarBackground.color = Color.gray;
            
            // Настраиваем RectTransform
            RectTransform backgroundRect = backgroundObj.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = Vector2.zero;
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
        }
        
        // Создаем FillArea если его нет
        GameObject fillAreaObj = healthBarSlider.transform.Find("FillArea")?.gameObject;
        if (fillAreaObj == null)
        {
            fillAreaObj = new GameObject("FillArea");
            fillAreaObj.transform.SetParent(healthBarSlider.transform);
            fillAreaObj.transform.localPosition = Vector3.zero;
            fillAreaObj.transform.localRotation = Quaternion.identity;
            
            RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.sizeDelta = new Vector2(-10, 0);
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
        }
        
        // Создаем Fill если его нет
        if (healthBarFill == null)
        {
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform);
            fillObj.transform.localPosition = Vector3.zero;
            fillObj.transform.localRotation = Quaternion.identity;
            
            healthBarFill = fillObj.AddComponent<Image>();
            healthBarFill.color = Color.green;
            
            // Настраиваем RectTransform
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
        }
        
        // Настраиваем Slider
        healthBarSlider.fillRect = healthBarFill.GetComponent<RectTransform>();
        healthBarSlider.targetGraphic = healthBarFill;
        healthBarSlider.value = 1f;
        healthBarSlider.minValue = 0f;
        healthBarSlider.maxValue = 1f;
        
        // Настраиваем HealthSystem если есть
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.healthBarCanvas = healthBarCanvas;
            healthSystem.healthBarSlider = healthBarSlider;
            healthSystem.healthBarFill = healthBarFill;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Показываем позицию health bar в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up * healthBarHeight, new Vector3(healthBarSize.x, healthBarSize.y, 0.1f));
    }
}