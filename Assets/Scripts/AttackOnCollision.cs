using UnityEngine;

public class AttackOnCollision : MonoBehaviour
{
    [Header("Attack Settings")]
    public float minDamagePercent = 0.2f; // 20% урона на минимальной скорости
    public float maxDamagePercent = 0.5f; // 50% урона на максимальной скорости
    public float minSpeed = 5f; // Минимальная скорость для урона
    public float maxSpeed = 20f; // Максимальная скорость для урона
    
    private Rigidbody rb;
    private UnitController unitController;
    private AttackEffects attackEffects;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        unitController = GetComponent<UnitController>();
        attackEffects = GetComponent<AttackEffects>();
        
        // Добавляем AttackEffects если его нет
        if (attackEffects == null)
        {
            attackEffects = gameObject.AddComponent<AttackEffects>();
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что это атака своего юнита по вражескому
        if (unitController == null || !unitController.IsPlayerUnit()) return;
        if (rb == null) return;
        
        UnitController targetUnit = collision.gameObject.GetComponent<UnitController>();
        if (targetUnit == null || !targetUnit.IsEnemyUnit()) return;
        
        // Получаем скорость атакующего юнита
        float currentSpeed = rb.velocity.magnitude;
        
        // Вычисляем урон на основе скорости
        float damagePercent = CalculateDamagePercent(currentSpeed);
        
        // Наносим урон
        HealthSystem targetHealth = targetUnit.GetComponent<HealthSystem>();
        if (targetHealth != null)
        {
            float maxHealth = 100f; // Можно получить из настроек
            float damage = maxHealth * damagePercent;
            targetHealth.TakeDamage(damage);
            
            // Показываем эффект атаки
            if (attackEffects != null)
            {
                attackEffects.ShowHitEffect(collision.contacts[0].point, damage);
            }
            
            Debug.Log($"{gameObject.name} атаковал {targetUnit.name} на {damage:F1} урона ({damagePercent:P0}) при скорости {currentSpeed:F1}");
        }
    }
    
    float CalculateDamagePercent(float speed)
    {
        // Ограничиваем скорость в пределах minSpeed - maxSpeed
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        
        // Вычисляем процент урона от 20% до 50%
        float normalizedSpeed = (speed - minSpeed) / (maxSpeed - minSpeed);
        return Mathf.Lerp(minDamagePercent, maxDamagePercent, normalizedSpeed);
    }
    
    void OnDrawGizmosSelected()
    {
        // Показываем диапазон скоростей в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Показываем минимальную и максимальную скорость
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * minSpeed * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * maxSpeed * 0.1f);
    }
}
