using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    [Header("Datos del usuario actual")]
    public string usuarioActual = "";
    public string rol = ""; // Ej: "terapeuta" o "paciente"

    [Header("Selección del jugador / sesión")]
    public string SelectedCar = "";
    public string SelectedScene = "";
    public string SelectedTrack = "";
    public int vueltasMeta = 3; // número de vueltas configuradas para el nivel actual


    // Alias por compatibilidad con scripts antiguos:
    public string CurrentUser => usuarioActual;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    // ===== Métodos para manipular los datos =====

    public void SetUsuarioActual(string nombreUsuario, string tipoRol = "")
    {
        usuarioActual = nombreUsuario;
        rol = tipoRol;
        Debug.Log($"👤 Usuario actual: {usuarioActual} (rol: {rol})");
    }

    public string GetUsuarioActual()
    {
        return usuarioActual;
    }

    public void SetCar(string carName)
    {
        SelectedCar = carName;
        Debug.Log($"🚗 Auto seleccionado: {carName}");
    }

    public void SetScene(string sceneName)
    {
        SelectedScene = sceneName;
        Debug.Log($"🎮 Escena seleccionada: {sceneName}");
    }

    public void SetTrack(string trackName)
    {
        SelectedTrack = trackName;
        Debug.Log($"🛣️ Circuito seleccionado: {trackName}");
    }
}
