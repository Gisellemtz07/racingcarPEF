using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelController : MonoBehaviour
{
    private bool nivelTerminado = false;

    void OnEnable()
    {
        LevelCompleteInvoker.OnLevelCompleted += OnNivelTerminado;
    }

    void OnDisable()
    {
        LevelCompleteInvoker.OnLevelCompleted -= OnNivelTerminado;
    }

    private void OnNivelTerminado()
    {
        if (nivelTerminado) return; // evitar doble llamada
        nivelTerminado = true;

        Debug.Log("🏁 Nivel completado. Revisando siguiente nivel...");

        var manager = GameModeManager.Instance;
        manager.AvanzarNivel();

        string siguiente = manager.GetNivelActual();

        if (!string.IsNullOrEmpty(siguiente))
        {
            Debug.Log($"➡️ Cargando siguiente nivel: {siguiente}");
            SceneManager.LoadScene(siguiente);
        }
        else
        {
            Debug.Log("🎉 Fin del modo historia. Regresando al menú principal.");
            SceneManager.LoadScene("MainMenu"); // cambia por tu escena del menú real
        }
    }
}

