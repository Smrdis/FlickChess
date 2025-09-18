using UnityEngine;

public class MoveHealthBarToRoot : MonoBehaviour
{
    void Start()
    {
        // Находим health_bar_root среди дочерних объектов
        Transform healthBarRoot = transform.Find("health_bar_root");
        
        if (healthBarRoot != null)
        {
            // Перемещаем в корень сцены
            healthBarRoot.SetParent(null);
            
            // Добавляем скрипт для следования за юнитом
            healthBarRoot.gameObject.AddComponent<FollowUnit>();
            healthBarRoot.gameObject.GetComponent<FollowUnit>().target = transform;
            healthBarRoot.gameObject.GetComponent<FollowUnit>().offset = Vector3.up * 2f;
            
            // Добавляем billboard эффект
            healthBarRoot.gameObject.AddComponent<HealthBarBillboard>();
            
            Debug.Log($"Health bar moved to root for {gameObject.name}");
        }
    }
}

public class FollowUnit : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.up * 2f;
    
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
