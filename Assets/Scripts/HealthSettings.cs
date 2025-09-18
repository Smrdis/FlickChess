using UnityEngine;

[CreateAssetMenu(fileName = "HealthSettings", menuName = "Game/Settings/Health Settings")]
public class HealthSettings : ScriptableObject
{
    [Header("Health Settings")]
    [Range(10f, 200f)]
    public float maxHealth = 100f;
    
    [Range(1f, 50f)]
    public float damageOnCollision = 10f;
    
    [Range(0.1f, 2f)]
    public float damageMultiplier = 1f;
    
    [Header("Health Bar Visual Settings")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    [Range(1f, 5f)]
    public float healthBarHeight = 2f;
    
    [Range(0.5f, 3f)]
    public float healthBarWidth = 2f;
    
    [Range(0.1f, 1f)]
    public float healthBarHeightPixels = 0.3f;
}