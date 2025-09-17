using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public GameplaySettings gameplaySettings;
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Перезагрузка сцены по настроенной клавише
        KeyCode reloadKey = gameplaySettings != null ? gameplaySettings.reloadKey : KeyCode.R;
        if (Input.GetKeyDown(reloadKey))
        {
            ReloadScene();
        }
    }
    
    void ReloadScene()
    {
        // Получаем имя текущей сцены и перезагружаем её
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}