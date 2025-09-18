using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Billboard Settings")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    
    [Header("Camera Settings")]
    public Camera targetCamera;
    public bool useMainCamera = true;
    
    private void Start()
    {
        if (targetCamera == null && useMainCamera)
        {
            targetCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        if (targetCamera == null) return;
        
        // Billboard эффект - объект смотрит в камеру, но с учетом блокировок осей
        Vector3 directionToCamera = targetCamera.transform.position - transform.position;
        
        // Применяем блокировки осей
        if (lockX) directionToCamera.x = 0;
        if (lockY) directionToCamera.y = 0;
        if (lockZ) directionToCamera.z = 0;
        
        // Если направление не нулевое, поворачиваем объект
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
    
    private void OnValidate()
    {
        if (targetCamera == null && useMainCamera)
        {
            targetCamera = Camera.main;
        }
    }
}
