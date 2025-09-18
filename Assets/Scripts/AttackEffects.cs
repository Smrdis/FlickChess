using UnityEngine;

public class AttackEffects : MonoBehaviour
{
    [Header("Attack Effects")]
    public GameObject hitEffectPrefab;
    public float effectDuration = 1f;
    public float effectScale = 1f;
    
    public void ShowHitEffect(Vector3 position, float damage)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * effectScale;
            
            // Уничтожаем эффект через время
            Destroy(effect, effectDuration);
        }
        
        // Показываем урон в консоли
        Debug.Log($"Hit effect at {position} for {damage:F1} damage");
    }
}
