using UnityEngine;

[CreateAssetMenu(fileName = "LevelModule", menuName = "NeuroTrack/StoryMode/Level Module")]
public class LevelModule : ScriptableObject
{
    [Tooltip("Nombre de la escena tal como aparece en Build Settings")]
    public string sceneName;

    [Tooltip("Nombre legible para UI")]
    public string displayName = "Nivel";

    [Min(1)]
    [Tooltip("Vueltas por defecto si no se sobreescriben en el StoryPlan")]
    public int defaultLaps = 3;

    [Tooltip("Tiempo objetivo opcional (segundos), 0 = sin objetivo")]
    public int targetTimeSeconds = 0;

    [Tooltip("Cualquier metadato extra (p.ej., dificultad)")]
    public string tagOrDifficulty = "Normal";
}

