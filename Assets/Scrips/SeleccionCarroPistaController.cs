using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SeleccionCarroPistaController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown dropdownPistas;
    public TMP_Dropdown dropdownCarros;
    public Button botonEmpezar;
    public Button botonVolver;

    [Header("Listas disponibles")]
    public string[] nombresPistas = { "Nivel 1", "Nivel 2", "Nivel 3", "Nivel 4" };
    public string[] nombresCarros = { "CarroRojo", "CarroAmarillo", "CarroAzul", "CarroPolicia" };
private readonly string[] nombresBonitos = { "Carro Rojo", "Carro Amarillo", "Carro Azul", "Carro Policia" };


    private void Start()
    {
        // Rellenar dropdowns
        dropdownPistas.ClearOptions();
        dropdownPistas.AddOptions(new System.Collections.Generic.List<string>(nombresPistas));

        dropdownCarros.ClearOptions();
        dropdownCarros.AddOptions(new System.Collections.Generic.List<string>(nombresCarros));

        // Listeners
        botonEmpezar.onClick.AddListener(OnClick_Empezar);
        botonVolver.onClick.AddListener(OnClick_Volver);
    }

    private void OnClick_Empezar()
    {
        // Asegurar el GameModeManager
        if (GameModeManager.Instance == null)
        {
            var go = new GameObject("_GameModeManager");
            go.AddComponent<GameModeManager>();
        }

        GameModeManager.Instance.SetMode(GameModeManager.GameMode.FreePlay);

        string pistaSeleccionada = nombresPistas[dropdownPistas.value];
        string carroSeleccionado = nombresCarros[dropdownCarros.value];

        // Guardar selección
        if (GameSession.Instance == null)
        {
            var go = new GameObject("_GameSession");
            go.AddComponent<GameSession>();
        }

        GameSession.Instance.SetUsuarioActual("ModoLibre");
        GameSession.Instance.SetCarroActual(carroSeleccionado);

        Debug.Log($"[ModoLibre]  {carroSeleccionado} | 🏁 {pistaSeleccionada}");
        SceneManager.LoadScene(pistaSeleccionada);
    }

    private void OnClick_Volver()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

