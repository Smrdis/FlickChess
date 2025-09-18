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
    
    [Range(1f, 500f)]
    public float maxLaunchForce = 11f;
    
    [Range(1f, 20f)]
    public float maxAimDistance = 10f;
    
    [Header("Visual Settings")]
    public Color aimLineColor = Color.red;
    public float aimLineWidth = 0.1f;
}