using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NivelControllerGlobal : MonoBehaviour
{
    private static NivelControllerGlobal instance;
    private bool esperandoContinuar = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[NivelControllerGlobal] Inicializado correctamente ✅");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Suscribirse al evento de nivel completado
        LevelCompleteInvoker.OnLevelCompleted += OnNivelCompletado;
    }

    private void OnDestroy()
    {
        // Desuscribir para evitar errores si el objeto se destruye
        LevelCompleteInvoker.OnLevelCompleted -= OnNivelCompletado;
    }

    private void OnNivelCompletado()
    {
        if (esperandoContinuar) return;

        esperandoContinuar = true;
        Debug.Log("[NivelControllerGlobal] Nivel completado. Preparando siguiente...");

        StartCoroutine(EsperarYAvanzar());
    }

    private IEnumerator EsperarYAvanzar()
    {
        yield return new WaitForSeconds(2f); // pequeña pausa opcional

        var manager = GameModeManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[NivelControllerGlobal] GameModeManager no encontrado.");
            yield break;
        }

        // Avanza el índice de nivel
        manager.AvanzarNivel();

        // Si aún quedan niveles, carga el siguiente
        string siguienteNivel = manager.GetNivelActual();
        if (!string.IsNullOrEmpty(siguienteNivel))
        {
            Debug.Log($"[NivelControllerGlobal] Cargando siguiente nivel: {siguienteNivel}");
            esperandoContinuar = false;
            SceneManager.LoadScene(siguienteNivel);
        }
        else
        {
            Debug.Log("[NivelControllerGlobal] 🎉 Historia completada. Volviendo al menú.");
            esperandoContinuar = false;
            SceneManager.LoadScene("MainMenu");
        }
    }
}

