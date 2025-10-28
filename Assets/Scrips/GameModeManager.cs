using UnityEngine;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode
    {
        Story,
        FreePlay
    }

    [SerializeField]
    private GameMode currentMode;
    public GameMode CurrentMode => currentMode;

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
        currentMode = mode;
    }

    public string GetNivelActual()
    {
        if (nivelActual < nivelesHistoria.Count)
        {
            return nivelesHistoria[nivelActual];
        }

        return null;
    }

    public void AvanzarNivel()
    {
        nivelActual++;
    }
}
