using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class FixPhysicsMaterials
{
    [MenuItem("Tools/Fix Physics Materials")]
    public static void FixMaterials()
    {
        // Загружаем материалы
        PhysicMaterial unitMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/PhysicsMaterials/UnitPhysics.physicMaterial");
        PhysicMaterial wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/PhysicsMaterials/WallPhysics.physicMaterial");
        PhysicMaterial arenaMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Materials/PhysicsMaterials/ArenaPhysics.physicMaterial");
        
        if (unitMaterial == null) Debug.LogError("UnitPhysics material not found!");
        if (wallMaterial == null) Debug.LogError("WallPhysics material not found!");
        if (arenaMaterial == null) Debug.LogError("ArenaPhysics material not found!");
        
        // Находим все объекты и назначаем материалы
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        
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
        
        // Сохраняем изменения
        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        AssetDatabase.SaveAssets();
        
        Debug.Log("🔰 Physics materials fixed and applied!");
    }
}