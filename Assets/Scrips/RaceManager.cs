using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static bool ControlsEnabled { get; private set; } = false;

    void Awake()
    {
        // Arranca SIEMPRE bloqueado
        ControlsEnabled = false;
    }

    public static void EnableControls()  => ControlsEnabled = true;
    public static void DisableControls() => ControlsEnabled = false;
}

