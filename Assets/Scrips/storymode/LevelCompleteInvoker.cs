using System;
using UnityEngine;

public static class LevelCompleteInvoker
{
    public static event Action OnLevelCompleted;

    public static void SignalComplete()
    {
        Debug.Log("⚡ Nivel completado → SignalComplete()");
        OnLevelCompleted?.Invoke();
    }
}






