using UnityEngine;

public class MenuModeSelector : MonoBehaviour
{
    [SerializeField] private GameModeManager gameModeManager;
    [SerializeField] private SceneNavigator sceneNavigator;

    private void Awake()
    {
        EnsureReferences();
    }

    public void SelectStoryMode()
    {
        EnsureReferences();

        if (gameModeManager != null)
        {
            gameModeManager.SetMode(GameModeManager.GameMode.Story);
        }

        if (sceneNavigator != null)
        {
            sceneNavigator.GoToLoginForMode(GameModeManager.GameMode.Story);
        }
    }

    public void SelectFreePlayMode()
    {
        EnsureReferences();

        if (gameModeManager != null)
        {
            gameModeManager.SetMode(GameModeManager.GameMode.FreePlay);
        }

        if (sceneNavigator != null)
        {
            sceneNavigator.GoToLoginForMode(GameModeManager.GameMode.FreePlay);
        }
    }

    private void EnsureReferences()
    {
        // intenta usar el singleton primero
        if (gameModeManager == null)
        {
            gameModeManager = GameModeManager.Instance;
        }

        // reemplazo del FindObjectOfType por API nueva
        if (sceneNavigator == null)
        {
            // Unity moderno prefiere esto:
#if UNITY_2023_1_OR_NEWER
            sceneNavigator = Object.FindFirstObjectByType<SceneNavigator>();
#else
            // fallback por si algún día abres el proyecto en una versión más viejita de Unity
            sceneNavigator = FindObjectOfType<SceneNavigator>();
#endif
        }
    }
}
