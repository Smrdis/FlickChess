using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("Settings")]
    public PhysicsSettings physicsSettings;
    public InputSettings inputSettings;
    
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
        
        // Применяем настройки физики
        if (physicsSettings != null)
        {
            rb.mass = physicsSettings.unitMass;
            rb.drag = physicsSettings.unitDrag;
            rb.angularDrag = physicsSettings.unitAngularDrag;
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
        
        // Настройка линии прицеливания
        if (aimLine == null)
        {
            GameObject lineObj = new GameObject("AimLine");
            lineObj.transform.SetParent(transform);
            aimLine = lineObj.AddComponent<LineRenderer>();
        }
        
        SetupAimLine();
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
    
    bool IsTouchOnUnit(Vector3 worldPosition)
    {
        float distance = Vector3.Distance(transform.position, worldPosition);
        float radius = inputSettings != null ? inputSettings.touchRadius : 2f;
        return distance <= radius;
    }
    
    void StartAiming(Vector3 touchPosition)
    {
        // Проверяем, не прицеливается ли уже другой юнит
        if (activeUnit != null && activeUnit != this)
        {
            return; // Другой юнит уже прицеливается
        }
        
        // Если этот юнит уже прицеливается, не начинаем заново
        if (isAiming)
        {
            return;
        }
        
        isAiming = true;
        activeUnit = this; // Устанавливаем себя как активного юнита
        touchStartPosition = touchPosition;
        aimLine.enabled = true;
        rb.isKinematic = true; // Останавливаем физику для прицеливания
    }
    
    void UpdateAiming()
    {
        // Обновляем прицеливание только для активного юнита
        if (activeUnit != this)
        {
            return;
        }
        
        if (aimLine != null)
        {
            aimLine.SetPosition(0, transform.position);
            // Показываем направление полета (aimDirection уже инвертирован)
            float aimDistance = inputSettings != null ? inputSettings.maxAimDistance : 5f;
            aimLine.SetPosition(1, transform.position + aimDirection * aimDistance);
        }
    }
    
    void UpdateAimDirection(Vector3 currentTouchPosition)
    {
        // Обновляем направление только для активного юнита
        if (activeUnit != this)
        {
            return;
        }
        
        Vector3 direction = (currentTouchPosition - transform.position).normalized;
        direction.y = 0; // Ограничиваем движение только по горизонтали
        
        if (direction.magnitude > 0.1f)
        {
            // Инвертируем направление - юнит летит в противоположную сторону
            aimDirection = -direction;
        }
    }
    
    void LaunchUnit()
    {
        // Проверяем, что это активный юнит
        if (activeUnit != this)
        {
            return; // Не активный юнит не может запускаться
        }
        
        isAiming = false;
        activeUnit = null; // Освобождаем активного юнита
        aimLine.enabled = false;
        rb.isKinematic = false; // Включаем физику обратно
        
        // Устанавливаем ограничения вращения в зависимости от того, на земле ли мы
        if (isOnGround)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None; // Свободное вращение в воздухе
        }
        
        // Вычисляем силу запуска на основе расстояния прицеливания
        float maxAimDist = inputSettings != null ? inputSettings.maxAimDistance : 5f;
        float launchF = inputSettings != null ? inputSettings.launchForce : 10f;
        float maxLaunchF = inputSettings != null ? inputSettings.maxLaunchForce : 20f;
        
        float aimDistance = Vector3.Distance(touchStartPosition, transform.position + aimDirection * maxAimDist);
        float forceMultiplier = Mathf.Clamp01(aimDistance / maxAimDist);
        float finalForce = launchF + (maxLaunchF - launchF) * forceMultiplier;
        
        // Применяем силу в направлении прицеливания
        rb.AddForce(aimDirection * finalForce, ForceMode.Impulse);
    }
    
    Vector3 GetTouchWorldPosition(Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            // Попробуем найти камеру заново
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            
            if (mainCamera == null)
            {
                Debug.LogError("Main camera is null! Please assign a camera to the scene.");
                return Vector3.zero;
            }
        }
        
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        
        return Vector3.zero;
    }
    
    void SetupAimLine()
    {
        if (aimLine != null)
        {
            aimLine.material = new Material(Shader.Find("Sprites/Default"));
            aimLine.material.color = inputSettings != null ? inputSettings.aimLineColor : Color.red;
            float lineWidth = inputSettings != null ? inputSettings.aimLineWidth : 0.1f;
            aimLine.startWidth = lineWidth;
            aimLine.endWidth = lineWidth;
            aimLine.positionCount = 2;
            aimLine.enabled = false;
        }
    }
    
    void CheckGroundStatus()
    {
        // Проверяем, находится ли юнит на земле
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;
        
        float checkDistance = physicsSettings != null ? physicsSettings.groundCheckDistance : 1.5f;
        bool wasOnGround = isOnGround;
        isOnGround = Physics.Raycast(rayStart, rayDirection, checkDistance, groundLayer);
        
        // Дополнительная проверка - если юнит слишком высоко, считаем что упал
        if (transform.position.y > 2f)
        {
            isOnGround = false;
        }
        
        // Отладочная информация
        Debug.DrawRay(rayStart, rayDirection * checkDistance, isOnGround ? Color.green : Color.red);
        
        // Если упали с платформы, разрешаем свободное вращение
        if (wasOnGround && !isOnGround)
        {
            if (physicsSettings == null || physicsSettings.allowRotationInAir)
            {
                rb.constraints = RigidbodyConstraints.None;
                Debug.Log("Unit fell off platform - free rotation enabled");
            }
        }
        // Если вернулись на платформу, замораживаем вращение по X и Z
        else if (!wasOnGround && isOnGround)
        {
            if (physicsSettings == null || physicsSettings.freezeRotationOnGround)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                Debug.Log("Unit back on platform - X and Z rotation frozen");
            }
        }
    }
    
    void UpdatePhysics()
    {
        if (isOnGround)
        {
            // На земле - скольжение с сопротивлением
            float slideDrag = physicsSettings != null ? physicsSettings.slidingDrag : 1f;
            rb.drag = slideDrag;
            rb.angularDrag = slideDrag;
            
            // Ограничиваем вертикальную скорость, чтобы не подпрыгивал
            Vector3 velocity = rb.velocity;
            if (velocity.y < 0)
            {
                velocity.y = 0;
                rb.velocity = velocity;
            }
        }
        else
        {
            // В воздухе - свободное падение
            float fallDrag = physicsSettings != null ? physicsSettings.fallingDrag : 0.1f;
            rb.drag = fallDrag;
            rb.angularDrag = fallDrag;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Визуализация радиуса касания в редакторе
        Gizmos.color = Color.yellow;
        float radius = inputSettings != null ? inputSettings.touchRadius : 2f;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Визуализация проверки земли
        Gizmos.color = isOnGround ? Color.green : Color.red;
        float checkDistance = physicsSettings != null ? physicsSettings.groundCheckDistance : 1.5f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
    }
}