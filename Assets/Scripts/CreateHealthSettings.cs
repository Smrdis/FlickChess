using UnityEngine;
using UnityEditor;

public class CreateHealthSettings
{
    [MenuItem("Tools/Create Health Settings")]
    public static void CreateHealthSettingsAsset()
    {
        HealthSettings settings = ScriptableObject.CreateInstance<HealthSettings>();
        AssetDatabase.CreateAsset(settings, "Assets/Settings/HealthSettings.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = settings;
        Debug.Log("HealthSettings asset created at Assets/Settings/HealthSettings.asset");
    }
}