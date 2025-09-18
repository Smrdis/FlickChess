using UnityEngine;
using UnityEditor;

public class HealthSettingsCreator
{
    [MenuItem("Tools/Create Health Settings Asset")]
    public static void CreateHealthSettings()
    {
        HealthSettings settings = ScriptableObject.CreateInstance<HealthSettings>();
        AssetDatabase.CreateAsset(settings, "Assets/Settings/HealthSettings.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = settings;
        Debug.Log("HealthSettings asset created!");
    }
}