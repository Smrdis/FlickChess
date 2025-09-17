using UnityEngine;

public class ManualFixMaterials : MonoBehaviour
{
    [Header("Physics Materials")]
    public PhysicMaterial unitMaterial;
    public PhysicMaterial wallMaterial;
    public PhysicMaterial arenaMaterial;
    
    [ContextMenu("Fix Physics Materials")]
    void FixMaterials()
    {
        // Загружаем материалы если не назначены
        if (unitMaterial == null)
        {
            unitMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/UnitPhysics");
        }
        
        if (wallMaterial == null)
        {
            wallMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/WallPhysics");
        }
        
        if (arenaMaterial == null)
        {
            arenaMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/ArenaPhysics");
        }
        
        // Находим все объекты и назначаем материалы
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Unit"))
            {
                CapsuleCollider collider = obj.GetComponent<CapsuleCollider>();
                if (collider != null && unitMaterial != null)
                {
                    collider.sharedMaterial = unitMaterial;
                    Debug.Log($"✅ Applied unit material to {obj.name}");
                }
            }
            else if (obj.name == "Wall")
            {
                BoxCollider collider = obj.GetComponent<BoxCollider>();
                if (collider != null && wallMaterial != null)
                {
                    collider.sharedMaterial = wallMaterial;
                    Debug.Log("✅ Applied wall material to Wall");
                }
            }
            else if (obj.name == "Arena")
            {
                BoxCollider collider = obj.GetComponent<BoxCollider>();
                if (collider != null && arenaMaterial != null)
                {
                    collider.sharedMaterial = arenaMaterial;
                    Debug.Log("✅ Applied arena material to Arena");
                }
            }
        }
        
        Debug.Log("🔰 Physics materials fixed!");
    }
}