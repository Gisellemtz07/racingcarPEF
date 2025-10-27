using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Tooltip("Tu script que controla las vueltas (p.ej., LapCounter)")]
    public MonoBehaviour lapCounterBehaviour;

    [Tooltip("Nombre del método público para fijar vueltas")]
    public string setLapsMethodName = "SetTargetLaps";

    void Start()
    {
        var state = StoryRuntimeState.Instance;
        if (state == null || state.currentStep == null)
        {
            Debug.LogWarning("StoryRuntimeState no presente; usando defaults del nivel.");
            return;
        }

        int laps = state.currentStep.targetLaps;

        var method = lapCounterBehaviour?.GetType().GetMethod(setLapsMethodName);
        if (method != null)
        {
            method.Invoke(lapCounterBehaviour, new object[] { laps });
            Debug.Log($"[LevelBootstrap] Vueltas objetivo fijadas a {laps}");
        }
        else
        {
            Debug.LogWarning($"No encontré el método {setLapsMethodName} en {lapCounterBehaviour?.GetType().Name}");
        }
    }
}

