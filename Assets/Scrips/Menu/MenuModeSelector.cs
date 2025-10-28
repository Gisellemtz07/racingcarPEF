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
        if (gameModeManager == null)
        {
            gameModeManager = GameModeManager.Instance;
        }

        if (sceneNavigator == null)
        {
            sceneNavigator = FindObjectOfType<SceneNavigator>();
        }
    }
}
