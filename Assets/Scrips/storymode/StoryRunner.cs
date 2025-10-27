using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryRunner : MonoBehaviour
{
    // ==== Singleton persistente ====
    public static StoryRunner Instance { get; private set; }

    [Header("Plan a ejecutar")]
    public StoryPlan plan;

    [Header("Opcional: escena a la que volver al terminar")]
    public string returnToScene = "MainMenu";

    private StoryRuntimeState state;
    private bool levelCompleted;

    void Awake()
    {
        // Singleton + persistencia entre escenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Asegura el runtime state (persistente)
        if (StoryRuntimeState.Instance == null)
        {
            var go = new GameObject("StoryRuntimeState");
            state = go.AddComponent<StoryRuntimeState>();
        }
        else
        {
            state = StoryRuntimeState.Instance;
        }

        if (plan == null || plan.steps.Count == 0)
        {
            Debug.LogError("[StoryRunner] No hay plan asignado o está vacío.");
            return;
        }

        state.currentPlan = plan;
        state.currentIndex = -1;

        StartCoroutine(RunPlan());
    }

    private IEnumerator RunPlan()
    {
        while (MoveNext())
        {
            var step = state.currentPlan.steps[state.currentIndex];
            state.SetStep(step);

            Debug.Log($"[StoryRunner] Cargando escena: {step.module.sceneName} con {state.currentStep.targetLaps} vueltas…");
            yield return SceneManager.LoadSceneAsync(step.module.sceneName, LoadSceneMode.Single);

            // Nos suscribimos DESPUÉS de cargar la escena, y como somos persistentes, no nos morimos al cambiar
            levelCompleted = false;
            LevelCompleteInvoker.OnLevelCompleted = HandleLevelCompleted;

            // Espera a que el nivel termine
            while (!levelCompleted)
                yield return null;
        }

        // Fin del plan
        if (!string.IsNullOrEmpty(returnToScene))
        {
            Debug.Log($"[StoryRunner] Plan terminado. Volviendo a {returnToScene}…");
            yield return SceneManager.LoadSceneAsync(returnToScene, LoadSceneMode.Single);
        }

        // Limpia el callback
        LevelCompleteInvoker.OnLevelCompleted = null;
    }

    private bool MoveNext()
    {
        if (state.currentPlan == null || state.currentPlan.steps.Count == 0) return false;
        state.currentIndex++;
        return state.currentIndex < state.currentPlan.steps.Count;
    }

    private void HandleLevelCompleted()
    {
        Debug.Log("[StoryRunner] Recibido: nivel completado ✅");
        levelCompleted = true;
        // seguridad: limpia el callback para evitar dobles llamadas
        LevelCompleteInvoker.OnLevelCompleted = null;
    }

    void OnDisable()
    {
        // Si por algo este objeto se desactiva, no dejes handlers colgando
        if (LevelCompleteInvoker.OnLevelCompleted != null)
            LevelCompleteInvoker.OnLevelCompleted = null;
    }
}
