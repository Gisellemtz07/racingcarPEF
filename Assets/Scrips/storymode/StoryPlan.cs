using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryPlan", menuName = "NeuroTrack/StoryMode/Story Plan")]
public class StoryPlan : ScriptableObject
{
    [Serializable]
    public class Step
    {
        public LevelModule module;
        [Tooltip("Si está activo, usa este número de vueltas en vez del default del módulo")]
        public bool overrideLaps;
        [Min(1)] public int lapsOverride = 3;
    }

    [Tooltip("Nombre del plan (paciente/sesión)")]
    public string planName = "Plan Terapéutico";
    public List<Step> steps = new();
}

