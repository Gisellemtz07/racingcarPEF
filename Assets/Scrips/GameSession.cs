using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public string CurrentUser;     // nombre del usuario logueado
    public string SelectedCar;     // p. ej., "Rojo"
    public string SelectedTrack;   // etiqueta del trayecto
    public string SelectedScene;   // nombre real de la escena a cargar

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

