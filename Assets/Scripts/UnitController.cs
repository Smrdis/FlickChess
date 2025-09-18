using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useGlobalSettings = true;
    
    [Header("Team Settings")]
    public bool isPlayerUnit = true; // true = свой юнит, false = чужой
    
    // Вспомогательные методы для получения настроек
    private PhysicsSettings GetPhysicsSettings()
    {
        return useGlobalSettings ? GameSettings.Physics : null;
    }
    
    private InputSettings GetInputSettings()
    {
        return useGlobalSettings ? GameSettings.Input : null;
    }
    
    [Header("Visual Settings")]
    public LineRenderer aimLine;
    
    [Header("Layer Settings")]
    public LayerMask touchableLayer = -1;
    public LayerMask groundLayer = 1; // Default layer
    
    private bool isAiming = false;
    private bool isOnGround = true;
    private Vector3 aimDirection;
    private Vector3 touchStartPosition;
    private Rigidbody rb;
    private Camera mainCamera;
    
    // Статическая переменная для отслеживания активного юнита
    private static UnitController activeUnit = null;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Применяем настройки физики из глобальных настроек
        var physics = GetPhysicsSettings();
        if (physics != null)
        {
            rb.mass = physics.unitMass;
            rb.drag = physics.unitDrag;
            rb.angularDrag = physics.unitAngularDrag;
        }
        
        // Устанавливаем начальные ограничения - только вращение по Y разрешено
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Находим камеру более надежным способом
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // Настройка линии прицеливания только для своих юнитов
        if (isPlayerUnit)
        {
            if (aimLine == null)
            {
                GameObject lineObj = new GameObject("AimLine");
                lineObj.transform.SetParent(transform);
                aimLine = lineObj.AddComponent<LineRenderer>();
                SetupAimLine();
            }
            
            // Добавляем компонент атаки для своих юнитов
            if (GetComponent<AttackOnCollision>() == null)
            {
                gameObject.AddComponent<AttackOnCollision>();
            }
        }
    }
    
    void Update()
    {
        HandleTouchInput();
        CheckGroundStatus();
        UpdatePhysics();
        
        if (isAiming)
        {
            UpdateAiming();
        }
    }
    
    void HandleTouchInput()
    {
        // Чужими юнитами нельзя управлять
        if (!isPlayerUnit) return;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorldPos = GetTouchWorldPosition(touch.position);
            
            if (touch.phase == TouchPhase.Began)
            {
                if (IsTouchOnUnit(touchWorldPos))
                {
                    StartAiming(touchWorldPos);
                }
            }
            else if (touch.phase == TouchPhase.Moved && isAiming)
            {
                UpdateAimDirection(touchWorldPos);
            }
            else if (touch.phase == TouchPhase.Ended && isAiming)
            {
                LaunchUnit();
            }
        }
        
        // Поддержка мыши для тестирования в редакторе
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GetTouchWorldPosition(Input.mousePosition);
            if (IsTouchOnUnit(mouseWorldPos))
            {
                StartAiming(mouseWorldPos);
            }
        }
        else if (Input.GetMouseButton(0) && isAiming)
        {
            Vector3 mouseWorldPos = GetTouchWorldPosition(Input.mousePosition);
            UpdateAimDirection(mouseWorldPos);
        }
        else if (Input.GetMouseButtonUp(0) && isAiming)
        {
            LaunchUnit();
        }
    }
    
    Vector3 GetTouchWorldPosition(Vector2 screenPosition)
    {
        if (mainCamera == null) return Vector3.zero;
        
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        
        // Если не попали в землю, возвращаем точку на плоскости Y=0
        float distance = mainCamera.transform.position.y / Mathf.Abs(mainCamera.transform.forward.y);
        return ray.GetPoint(distance);
    }
    
    bool IsTouchOnUnit(Vector3 worldPosition)
    {
        float distance = Vector3.Distance(transform.position, worldPosition);
        var input = GetInputSettings();
        float radius = input != null ? input.touchRadius : 2f;
        return distance <= radius;
    }
    
    void StartAiming(Vector3 worldPosition)
    {
        isAiming = true;
        touchStartPosition = worldPosition;
        
        // Устанавливаем активный юнит
        if (activeUnit != null && activeUnit != this)
        {
            activeUnit.StopAiming();
        }
        activeUnit = this;
        
        // Включаем линию прицеливания
        if (aimLine != null)
        {
            aimLine.enabled = true;
            Debug.Log("AimLine enabled!");
        }
        else
        {
            Debug.LogError("AimLine is null when starting aim!");
        }
        
        UpdateAimDirection(worldPosition);
    }
    
    void UpdateAimDirection(Vector3 worldPosition)
    {
        // Направление прицеливания - от точки касания к юниту (обратная сторона)
        aimDirection = (transform.position - worldPosition).normalized;
        aimDirection.y = 0; // Убираем вертикальную составляющую
        aimDirection = aimDirection.normalized;
        
        // Поворачиваем юнит в направлении прицеливания
        if (aimDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(aimDirection);
        }
    }
    
    void UpdateAiming()
    {
        if (aimLine != null)
        {
            // Вычисляем расстояние от юнита до текущей точки касания
            Vector3 currentTouchPos = GetTouchWorldPosition(Input.mousePosition);
            float distance = Vector3.Distance(transform.position, currentTouchPos);
            
            // Получаем настройки для максимальной длины прицела
            var input = GetInputSettings();
            float maxDistance = input != null ? input.maxAimDistance : 5f;
            
            // Ограничиваем длину прицела
            distance = Mathf.Clamp(distance, 0.5f, maxDistance);
            
            // Прицел от юнита в направлении прицеливания (обратная сторона)
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + aimDirection * distance;
            
            aimLine.SetPosition(0, startPos);
            aimLine.SetPosition(1, endPos);
        }
    }
    
    void LaunchUnit()
    {
        if (!isAiming) return;
        
        isAiming = false;
        
        // Убеждаемся, что Rigidbody активен и не заблокирован
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null!");
            return;
        }
        
        if (rb.isKinematic)
        {
            Debug.LogWarning("Rigidbody is kinematic, enabling physics!");
            rb.isKinematic = false;
        }
        
        // Скрываем линию прицеливания
        if (aimLine != null)
        {
            aimLine.enabled = false;
        }
        
        // Вычисляем силу на основе текущего расстояния от юнита до мыши/касания
        Vector3 currentTouchPos;
        if (Input.touchCount > 0)
        {
            currentTouchPos = GetTouchWorldPosition(Input.GetTouch(0).position);
        }
        else
        {
            currentTouchPos = GetTouchWorldPosition(Input.mousePosition);
        }
        float distance = Vector3.Distance(transform.position, currentTouchPos);
        var input = GetInputSettings();
        float maxDistance = input != null ? input.maxAimDistance : 10f;
        
        // Прямая зависимость: чем дальше оттягиваешь, тем сильнее летит
        float forceMultiplier = Mathf.Clamp(distance / maxDistance, 0.1f, 1f);
        
        // Применяем силу в направлении прицеливания (обратная сторона)
        float maxForce = input != null ? input.maxLaunchForce : 11f;
        float finalForce = maxForce * forceMultiplier;
        
        // Отладочная информация о настройках
        Debug.Log($"Input settings: {(input != null ? "loaded" : "null")}, maxForce: {maxForce}");
        
        // Используем ForceMode.Impulse для мгновенного применения силы
        rb.AddForce(aimDirection * finalForce, ForceMode.Impulse);
        
        // Отладочная информация
        Debug.Log($"Launch Force: {finalForce:F2} (Distance: {distance:F2}, Multiplier: {forceMultiplier:F2}, Direction: {aimDirection})");
        Debug.Log($"Rigidbody velocity before: {rb.velocity}");
        
        // Сбрасываем активный юнит
        if (activeUnit == this)
        {
            activeUnit = null;
        }
    }
    
    void StopAiming()
    {
        isAiming = false;
        if (aimLine != null)
        {
            aimLine.enabled = false;
        }
    }
    
    void CheckGroundStatus()
    {
        var physics = GetPhysicsSettings();
        float checkDistance = physics != null ? physics.groundCheckDistance : 1.5f;
        
        isOnGround = Physics.Raycast(transform.position, Vector3.down, checkDistance, groundLayer);
    }
    
    void UpdatePhysics()
    {
        // Убрали дополнительное сопротивление - пусть юнит летит свободно
    }
    
    void SetupAimLine()
    {
        if (aimLine == null) 
        {
            Debug.LogError("AimLine is null!");
            return;
        }
        
        // Используем стандартный шейдер Unity
        aimLine.material = new Material(Shader.Find("Standard"));
        aimLine.material.color = Color.red;
        
        aimLine.startWidth = 0.1f;
        aimLine.endWidth = 0.1f;
        aimLine.positionCount = 2;
        aimLine.enabled = false;
        
        Debug.Log("AimLine setup completed!");
    }
    
    // Методы для работы с командой
    public bool IsPlayerUnit()
    {
        return isPlayerUnit;
    }
    
    public bool IsEnemyUnit()
    {
        return !isPlayerUnit;
    }
    
    public void SetTeam(bool isPlayer)
    {
        isPlayerUnit = isPlayer;
    }
    
    void OnDrawGizmosSelected()
    {
        // Визуализация радиуса касания
        Gizmos.color = Color.yellow;
        var input = GetInputSettings();
        float radius = input != null ? input.touchRadius : 2f;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Визуализация проверки земли
        Gizmos.color = isOnGround ? Color.green : Color.red;
        var physics = GetPhysicsSettings();
        float checkDistance = physics != null ? physics.groundCheckDistance : 1.5f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
    }
}
