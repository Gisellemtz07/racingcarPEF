using UnityEngine;

public class StoryRuntimeState : MonoBehaviour
{
    public static StoryRuntimeState Instance { get; private set; }

    [System.Serializable]
    public class StepRuntime
    {
        public string sceneName;
        public int targetLaps;
    }

    public StoryPlan currentPlan;
    public int currentIndex;
    public StepRuntime currentStep;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetStep(StoryPlan.Step step)
    {
        currentStep = new StepRuntime
        {
            sceneName = step.module.sceneName,
            targetLaps = step.overrideLaps ? step.lapsOverride : step.module.defaultLaps
        };
    }

    public bool HasNextStep()
    {
        return currentPlan != null && currentIndex + 1 < currentPlan.steps.Count;
    }
}

