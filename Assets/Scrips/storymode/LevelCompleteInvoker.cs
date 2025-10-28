using System;
using UnityEngine;

public class LevelCompleteInvoker : MonoBehaviour
{
    public static event Action OnLevelCompleted;
    public static void SignalComplete()
    {
        Debug.Log("LevelCompleteInvoker: ¡nivel completado!");
        OnLevelCompleted?.Invoke();
    }
}


