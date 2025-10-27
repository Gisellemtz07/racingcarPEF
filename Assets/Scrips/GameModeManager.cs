using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;
    public enum GameMode { Historia, Libre }
    public GameMode currentMode;

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
        else
        {
            Destroy(gameObject);
        }
    }

    public void ElegirModoHistoria()
    {
        currentMode = GameMode.Historia;
        SceneManager.LoadScene("HistoriaSetup");
    }

    public void ElegirModoLibre()
    {
        currentMode = GameMode.Libre;

        // 🔹 Aquí respetamos tu login del modo libre
        SceneManager.LoadScene("Login");
    }

    public string GetNivelActual()
    {
        if (nivelActual < nivelesHistoria.Count)
            return nivelesHistoria[nivelActual];
        else
            return null;
    }

    public void AvanzarNivel()
    {
        nivelActual++;
    }
}


