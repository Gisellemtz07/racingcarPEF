using System;

public static class LevelCompleteInvoker
{
    public static Action OnLevelCompleted;

    public static void SignalComplete()
    {
        OnLevelCompleted?.Invoke();
        OnLevelCompleted = null; // evita dobles llamadas tras cambio de escena
    }
}

