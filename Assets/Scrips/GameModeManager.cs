using UnityEngine;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    // === Singleton ===
    public static GameModeManager Instance;

    // === Tipos de modo de juego ===
    public enum GameMode
    {
        Story = 0,      // Modo historia / terapeuta define el orden
        FreePlay = 1,   // Modo libre / elegir pista suelta

        // Alias legacy para no romper otros scripts viejos
        [System.Obsolete("Use Story instead of Historia")]
        Historia = Story,

        [System.Obsolete("Use FreePlay instead of Libre")]
        Libre = FreePlay
    }

    // === Modo actual seleccionado ===
    [SerializeField]
    private GameMode currentMode = GameMode.FreePlay;

    // Propiedad pública de solo lectura
    public GameMode CurrentMode => currentMode;

    // === CONFIGURACIÓN DEL MODO HISTORIA ===
    // Lista en orden de las escenas que se van a jugar en Historia
    // Ejemplo: ["CircuitoFacil", "CircuitoRectas", "CircuitoCurvas", "NivelFinal"]
    public List<string> nivelesHistoria = new List<string>();

    // Para cada escena, cuántas vueltas debe completar ese paciente
    // Ejemplo: vueltasPorNivel["CircuitoFacil"] = 3;
    public Dictionary<string, int> vueltasPorNivel = new Dictionary<string, int>();

    // Índice actual dentro de nivelesHistoria
    public int nivelActual = 0;

    // === Singleton setup ===
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // se queda vivo entre escenas
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // evitamos duplicados
        }
    }

    // === Cambiar el modo (la UI llama esto cuando eliges Historia o Libre) ===
    public void SetMode(GameMode mode)
    {
        currentMode = mode;
    }

    // === Helpers cómodos para otros scripts ===
    public bool IsStoryMode()
    {
        return currentMode == GameMode.Story;
    }

    public bool IsFreePlayMode()
    {
        return currentMode == GameMode.FreePlay;
    }

    // === HISTORIA: obtener nombre de la escena actual según el índice ===
    public string GetNivelActual()
    {
        if (nivelActual >= 0 && nivelActual < nivelesHistoria.Count)
        {
            return nivelesHistoria[nivelActual];
        }

        return null; // ya no hay más niveles
    }

    // === HISTORIA: avanzar al siguiente nivel en la lista ===
    public void AvanzarNivel()
    {
        nivelActual++;
    }

    // === HISTORIA: cuántas vueltas tiene que dar este nivel ===
    public int GetVueltasParaNivel(string nombreNivel)
    {
        if (vueltasPorNivel.ContainsKey(nombreNivel))
        {
            return vueltasPorNivel[nombreNivel];
        }

        // Valor default si no está configurado manualmente
        return 3;
    }

    // === HISTORIA: resetear para empezar desde el primer nivel ===
    public void ResetHistoria()
    {
        nivelActual = 0;
    }
}

