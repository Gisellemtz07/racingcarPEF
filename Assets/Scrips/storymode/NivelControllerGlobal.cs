using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelControllerGlobal : MonoBehaviour
{
    void Awake() => DontDestroyOnLoad(gameObject);

    void OnEnable() => LevelCompleteInvoker.OnLevelCompleted += OnNivelTerminado;
    void OnDisable() => LevelCompleteInvoker.OnLevelCompleted -= OnNivelTerminado;

    private void OnNivelTerminado()
    {
        var manager = GameModeManager.Instance;
        if (manager == null) return;

        manager.nivelActual++;
        if (manager.nivelActual < manager.nivelesHistoria.Count)
        {
            string siguiente = manager.nivelesHistoria[manager.nivelActual];
            Debug.Log($"➡️ Cargando siguiente nivel: {siguiente}");
            SceneManager.LoadScene(siguiente);
        }
        else
        {
            Debug.Log("🎉 Todos los niveles completados. Regresando al menú principal.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}

