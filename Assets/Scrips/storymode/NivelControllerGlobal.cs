using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Linq; 

public class NivelControllerGlobal : MonoBehaviour
{
    private static NivelControllerGlobal instance;

    private GameObject panelFinNivel;
    private Button botonContinuar;
    private Button botonSalir;

    private bool mostrandoPanel = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[NivelControllerGlobal] Inicializado ✅");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LevelCompleteInvoker.OnLevelCompleted += OnNivelCompletado;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        LevelCompleteInvoker.OnLevelCompleted -= OnNivelCompletado;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // === BUSCAR UI ===
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[NivelControllerGlobal] Escena cargada: " + scene.name);
        TryWireUI();
    }
 // Intenta encontrar el panel y conectar botones

private void TryWireUI()
{
    // Intenta buscar el Panel incluso si está inactivo o dentro de otro Canvas
    panelFinNivel = Resources.FindObjectsOfTypeAll<Canvas>()
        .SelectMany(c => c.GetComponentsInChildren<Transform>(true))
        .FirstOrDefault(t => t.name == "PanelFinNivel")?.gameObject;

    if (panelFinNivel == null)
    {
        Debug.LogWarning("[NivelControllerGlobal] ❌ No encontré PanelFinNivel en esta escena (ni activo ni inactivo).");
        botonContinuar = null;
        botonSalir = null;
        return;
    }

    Debug.Log("[NivelControllerGlobal] ✅ Encontré PanelFinNivel (" + panelFinNivel.scene.name + ")");
    panelFinNivel.SetActive(false);
    mostrandoPanel = false;

    // Busca los botones aunque estén desactivados
    Transform continuarT = panelFinNivel.GetComponentsInChildren<Transform>(true)
        .FirstOrDefault(t => t.name == "BtnContinuar");
    Transform salirT = panelFinNivel.GetComponentsInChildren<Transform>(true)
        .FirstOrDefault(t => t.name == "BtnSalir");

    if (continuarT != null)
    {
        botonContinuar = continuarT.GetComponent<Button>();
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.onClick.AddListener(OnClickContinuar);
        Debug.Log("[NivelControllerGlobal] 🟢 BtnContinuar conectado correctamente.");
    }
    else
    {
        Debug.LogWarning("[NivelControllerGlobal] ⚠️ No encontré BtnContinuar dentro del PanelFinNivel.");
    }

    if (salirT != null)
    {
        botonSalir = salirT.GetComponent<Button>();
        botonSalir.onClick.RemoveAllListeners();
        botonSalir.onClick.AddListener(OnClickSalir);
        Debug.Log("[NivelControllerGlobal] 🟢 BtnSalir conectado correctamente.");
    }
    else
    {
        Debug.LogWarning("[NivelControllerGlobal] ⚠️ No encontré BtnSalir dentro del PanelFinNivel.");
    }
}

    // === CUANDO EL NIVEL TERMINA ===
    private void OnNivelCompletado()
    {
        Debug.Log("[NivelControllerGlobal] Recibí señal de nivel completado 🏁");

        // Asegúrate de que tenemos referenciado el panel,
        // por si no estaba listo en OnSceneLoaded.
        if (panelFinNivel == null)
        {
            Debug.Log("[NivelControllerGlobal] panelFinNivel era null, intentamos volver a enlazar UI...");
            TryWireUI();
        }

        // ¿ahora sí hay panel?
        if (panelFinNivel != null)
        {
            MostrarPanelFinNivel();
        }
        else
        {
            Debug.LogWarning("[NivelControllerGlobal] No hay PanelFinNivel, vamos directo al siguiente nivel 🚀");
            AvanzarAlSiguienteNivel();
        }
    }

    private void MostrarPanelFinNivel()
    {
        if (mostrandoPanel) return;
        mostrandoPanel = true;

        Debug.Log("[NivelControllerGlobal] Mostrando panel de fin de nivel ✅");

        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar UI
        panelFinNivel.SetActive(true);
    }

    // === BOTÓN CONTINUAR ===
    private void OnClickContinuar()
    {
        Debug.Log("[NivelControllerGlobal] Click en CONTINUAR");
        Time.timeScale = 1f;
        mostrandoPanel = false;

        if (panelFinNivel != null)
            panelFinNivel.SetActive(false);

        AvanzarAlSiguienteNivel();
    }

    // === BOTÓN SALIR ===
    private void OnClickSalir()
    {
        Debug.Log("[NivelControllerGlobal] Click en SALIR → MainMenu");
        Time.timeScale = 1f;
        mostrandoPanel = false;

        if (panelFinNivel != null)
            panelFinNivel.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    // === CAMBIO DE NIVEL ===
    private void AvanzarAlSiguienteNivel()
    {
        var manager = GameModeManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[NivelControllerGlobal] No hay GameModeManager 😭");
            return;
        }

        manager.AvanzarNivel();

        string siguiente = manager.GetNivelActual();
        if (!string.IsNullOrEmpty(siguiente))
        {
            Debug.Log("[NivelControllerGlobal] Cargando siguiente nivel: " + siguiente);
            SceneManager.LoadScene(siguiente);
        }
        else
        {
            Debug.Log("[NivelControllerGlobal] No hay más niveles. Fin de historia 🎉 → MainMenu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
