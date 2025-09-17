using UnityEngine;

public class UnitBounce : MonoBehaviour
{
    [Header("Settings")]
    public PhysicsSettings physicsSettings;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (physicsSettings == null) return;
        
        // Проверяем, достаточно ли скорости для рикошета
        if (rb.velocity.magnitude < physicsSettings.minBounceVelocity)
        {
            return;
        }
        
        // Получаем направление отскока
        Vector3 bounceDirection = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        
        // Применяем силу отскока
        rb.AddForce(bounceDirection * physicsSettings.bounceForce, ForceMode.Impulse);
        
        // Добавляем случайность для более интересного поведения
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0f, 1f),
            Random.Range(-1f, 1f)
        ) * physicsSettings.bounceForce * physicsSettings.bounceRandomness;
        
        rb.AddForce(randomForce, ForceMode.Impulse);
    }
}