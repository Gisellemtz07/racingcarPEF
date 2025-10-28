using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [Header("Escenas destino según modo")]
    [SerializeField] private string storyLoginScene = "HistoriaSetup"; // pantalla donde configuras el orden de niveles
    [SerializeField] private string freePlayLoginScene = "Login";      // pantalla de login normal antes de elegir pista

    /// <summary>
    /// Carga una escena por nombre si está configurada y en Build Settings.
    /// </summary>
    public void GoTo(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneNavigator] No se proporcionó nombre de escena.");
            return;
        }

        // Opcional: checar que la escena está en Build Settings
        if (!IsSceneInBuild(sceneName))
        {
            Debug.LogError("[SceneNavigator] La escena '" + sceneName +
                           "' NO está en Build Settings. Ábrela y agrégala con File > Build Settings > Add Open Scenes.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Según el modo de juego seleccionado, manda a la pantalla correcta.
    /// Story -> normalmente HistoriaSetup (terapeuta arma la secuencia)
    /// FreePlay -> normalmente Login (paciente elige nivel libre)
    /// </summary>
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

            default:
                Debug.LogWarning("[SceneNavigator] Modo no reconocido: " + mode);
                break;
        }
    }

    /// <summary>
    /// Helper para depurar: revisa si una escena está incluida en Scenes In Build.
    /// </summary>
    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            // saca el path completo tipo "Assets/Scenes/Login.unity"
            string fullPath = SceneUtility.GetScenePathByBuildIndex(i);

            // nos quedamos solo con el nombre "Login"
            string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);

            if (fileName == sceneName)
                return true;
        }

        return false;
    }

    // --- Opcionales (calidad de vida) ---
    public void GoHistoriaSetupDirect() => GoTo(storyLoginScene);
    public void GoFreePlayLoginDirect() => GoTo(freePlayLoginScene);
}
