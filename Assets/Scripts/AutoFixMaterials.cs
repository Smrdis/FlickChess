using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixMaterials
{
    static AutoFixMaterials()
    {
        EditorApplication.delayCall += FixMaterialsOnLoad;
    }
    
    static void FixMaterialsOnLoad()
    {
        // Загружаем материалы
        PhysicMaterial unitMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/UnitPhysics.physicsMaterial");
        PhysicMaterial wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/WallPhysics.physicsMaterial");
        PhysicMaterial arenaMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/ArenaPhysics.physicsMaterial");
        
        if (unitMaterial == null || wallMaterial == null || arenaMaterial == null)
        {
            Debug.LogWarning("Physics materials not found, skipping auto-fix");
            return;
        }
        
        // Находим все объекты и назначаем материалы
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Unit"))
            {
                CapsuleCollider collider = obj.GetComponent<CapsuleCollider>();
                if (collider != null && collider.sharedMaterial == null)
                {
                    collider.sharedMaterial = unitMaterial;
                    Debug.Log($"✅ Auto-applied unit material to {obj.name}");
                }
            }
            else if (obj.name == "Wall")
            {
                BoxCollider collider = obj.GetComponent<BoxCollider>();
                if (collider != null && collider.sharedMaterial == null)
                {
                    collider.sharedMaterial = wallMaterial;
                    Debug.Log("✅ Auto-applied wall material to Wall");
                }
            }
            else if (obj.name == "Arena")
            {
                BoxCollider collider = obj.GetComponent<BoxCollider>();
                if (collider != null && collider.sharedMaterial == null)
                {
                    collider.sharedMaterial = arenaMaterial;
                    Debug.Log("✅ Auto-applied arena material to Arena");
                }
            }
        }
    }
}