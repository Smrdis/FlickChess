using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private bool useGlobalSettings = true;
    
    private HealthSystem healthSystem;
    private Rigidbody rb;
    
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody>();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (healthSystem == null || healthSystem.IsDead()) return;
        
        // Вычисляем урон на основе скорости столкновения
        float collisionForce = collision.relativeVelocity.magnitude;
        
        if (useGlobalSettings && GameSettings.Health != null)
        {
            float baseDamage = GameSettings.Health.damageOnCollision;
            float damageMultiplier = GameSettings.Health.damageMultiplier;
            
            // Урон зависит от силы столкновения
            float damage = baseDamage * (collisionForce / 10f) * damageMultiplier;
            
            // Минимальный урон при любом столкновении
            damage = Mathf.Max(damage, baseDamage * 0.1f);
            
            healthSystem.TakeDamage(damage);
            
            Debug.Log($"{gameObject.name} took {damage:F1} damage from collision with {collision.gameObject.name}");
        }
    }
}