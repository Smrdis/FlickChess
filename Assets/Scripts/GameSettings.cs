using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("Global Settings")]
    public PhysicsSettings physicsSettings;
    public InputSettings inputSettings;
    public HealthSettings healthSettings;
    public GameplaySettings gameplaySettings;
    
    // Singleton для глобального доступа
    public static GameSettings Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern - force recompile
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Глобальные геттеры для удобства
    public static PhysicsSettings Physics => Instance?.physicsSettings;
    public static InputSettings Input => Instance?.inputSettings;
    public static HealthSettings Health => Instance?.healthSettings;
    public static GameplaySettings Gameplay => Instance?.gameplaySettings;
}