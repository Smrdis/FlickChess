using UnityEngine;

[CreateAssetMenu(fileName = "InputSettings", menuName = "Game/Settings/Input Settings")]
public class InputSettings : ScriptableObject
{
    [Header("Touch Settings")]
    [Range(0.5f, 5f)]
    public float touchRadius = 2f;
    
    [Header("Launch Settings")]
    [Range(1f, 50f)]
    public float launchForce = 10f;
    
    [Range(1f, 100f)]
    public float maxLaunchForce = 20f;
    
    [Range(1f, 10f)]
    public float maxAimDistance = 5f;
    
    [Header("Visual Settings")]
    public Color aimLineColor = Color.red;
    public float aimLineWidth = 0.1f;
}