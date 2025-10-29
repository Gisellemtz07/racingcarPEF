using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode { FreePlay, Story }
    public GameMode CurrentMode = GameMode.FreePlay;

    public List<string> nivelesHistoria = new List<string>();
    public Dictionary<string, int> vueltasPorNivel = new Dictionary<string, int>();
    public int nivelActual = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetMode(GameMode mode)
    {
        CurrentMode = mode;
        Debug.Log($"[GameModeManager] Modo cambiado a: {mode}");
    }

    public string GetNivelActual()
    {
        if (nivelActual < 0 || nivelActual >= nivelesHistoria.Count)
            return null;

        return nivelesHistoria[nivelActual];
    }

    public void AvanzarNivel()
    {
        nivelActual++;
        if (nivelActual >= nivelesHistoria.Count)
        {
            Debug.Log("[GameModeManager] Historia completa 🎉");
        }
        else
        {
            Debug.Log($"[GameModeManager] Avanzando al nivel {nivelActual + 1}");
        }
    }
}


