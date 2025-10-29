using UnityEngine;
using System;

public static class LevelCompleteInvoker
{
    // 🔹 Evento global que cualquier script puede escuchar
    public static event Action OnLevelCompleted;

    // 🔹 Método que tus scripts (como GameMetrics) deben llamar
    public static void SignalComplete()
    {
        Debug.Log("[LevelCompleteInvoker] 🏁 Nivel completado, enviando señal...");
        OnLevelCompleted?.Invoke();
    }
}





