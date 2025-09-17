using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Game/Settings/Gameplay Settings")]
public class GameplaySettings : ScriptableObject
{
    [Header("Scene Management")]
    public KeyCode reloadKey = KeyCode.R;
    
    [Header("Camera Settings")]
    public Vector3 cameraPosition = new Vector3(0, 20, -12);
    public Vector3 cameraRotation = new Vector3(60, 0, 0);
    
    [Header("Game Rules")]
    public int maxUnitsPerPlayer = 1;
    public bool allowMultipleAiming = false;
    
    [Header("Debug Settings")]
    public bool showDebugRays = true;
    public bool showTouchRadius = true;
}