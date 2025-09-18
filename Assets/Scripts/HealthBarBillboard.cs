using UnityEngine;

public class HealthBarBillboard : MonoBehaviour
{
    private Camera targetCamera;
    
    void Start()
    {
        targetCamera = Camera.main;
        if (targetCamera == null)
        {
            targetCamera = FindObjectOfType<Camera>();
        }
    }
    
    void LateUpdate()
    {
        if (targetCamera == null) return;
        
        // Простой billboard эффект - хелсбар всегда смотрит в камеру
        // но остается горизонтальным (не наклоняется вверх/вниз)
        Vector3 directionToCamera = targetCamera.transform.position - transform.position;
        directionToCamera.y = 0; // Убираем вертикальную составляющую
        
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}
