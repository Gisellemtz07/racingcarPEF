using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [SerializeField] private string storyLoginScene = "HistoriaSetup";
    [SerializeField] private string freePlayLoginScene = "Login";

    public void GoTo(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void GoToLoginForMode(GameModeManager.GameMode mode)
    {
        switch (mode)
        {
            case GameModeManager.GameMode.Story:
                GoTo(storyLoginScene);
                break;
            case GameModeManager.GameMode.FreePlay:
                GoTo(freePlayLoginScene);
                break;
        }
    }
}