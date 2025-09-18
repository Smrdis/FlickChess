using UnityEngine;
using UnityEditor;

public class CreateHealthAsset
{
    [MenuItem("Tools/Create Health Settings")]
    public static void CreateHealthSettings()
    {
        HealthSettings settings = ScriptableObject.CreateInstance<HealthSettings>();
        AssetDatabase.CreateAsset(settings, "Assets/Settings/HealthSettings.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("HealthSettings created!");
    }
}