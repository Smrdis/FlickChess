using UnityEngine;
using UnityEditor;

public class CreateHealthBars : MonoBehaviour
{
    [MenuItem("Tools/Create Health Bars")]
    public static void CreateHealthBarsForUnits()
    {
        // Находим все юниты в сцене
        UnitController[] units = FindObjectsOfType<UnitController>();
        
        foreach (UnitController unit in units)
        {
            HealthSystem healthSystem = unit.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Принудительно создаем health bar
                healthSystem.SendMessage("SetupHealthBar", SendMessageOptions.DontRequireReceiver);
                Debug.Log($"Health bar created for {unit.name}");
            }
        }
        
        Debug.Log("Health bars created for all units!");
    }
}