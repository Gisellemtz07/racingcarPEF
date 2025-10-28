using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    public GameMetrics metrics;

    void Start()
    {
        if (metrics == null)
            metrics = Object.FindFirstObjectByType<GameMetrics>();


        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var manager = GameModeManager.Instance;

        int laps = 3;
        if (manager != null && manager.vueltasPorNivel.ContainsKey(currentScene))
            laps = manager.vueltasPorNivel[currentScene];

        if (metrics != null)
        {
            metrics.SetTargetLaps(laps);
            Debug.Log($"[LevelBootstrap] Asignadas {laps} vueltas al nivel {currentScene}");
        }
        else
        {
            Debug.LogWarning("[LevelBootstrap] No se encontró GameMetrics.");
        }
    }
}


