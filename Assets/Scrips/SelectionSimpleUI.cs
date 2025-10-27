using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionSimpleUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text txtSaludo;
    public TMP_Text txtResumen;
    public Button btnComenzar;

    // Estado seleccionado
    string selectedCarId = null;     // ej. "Carro_0"
    string selectedScene = null;     // ej. "Nivel1"

    void Start()
    {
        if (GameSession.Instance == null)
        {
            var go = new GameObject("_GameSession");
            go.AddComponent<GameSession>();
        }

        if (txtSaludo) txtSaludo.text = $"Hola, {GameSession.Instance.CurrentUser} 👋";
        if (btnComenzar) btnComenzar.interactable = false;
        ActualizarResumen();
    }

    // ---- Llamados por botones ----
    public void SelectCar(string carId)
    {
        selectedCarId = carId;
        ActualizarResumen();
        ActualizarComenzar();
    }

    public void SelectLevel(string sceneName)
    {
        selectedScene = sceneName;
        ActualizarResumen();
        ActualizarComenzar();
    }

    public void OnClickComenzar()
    {
        if (string.IsNullOrEmpty(selectedCarId) || string.IsNullOrEmpty(selectedScene))
        {
            if (txtResumen) txtResumen.text = "Selecciona un carro y un nivel antes de comenzar.";
            return;
        }

        GameSession.Instance.SelectedCar = selectedCarId;   // "Carro_0"
        GameSession.Instance.SelectedScene = selectedScene;   // "Nivel1"
        GameSession.Instance.SelectedTrack = selectedScene;   // opcional

        PlayerPrefs.SetString("SelectedCarId", selectedCarId);
        PlayerPrefs.Save();

        SceneManager.LoadScene(selectedScene);
    }

    // ---- Helpers ----
    void ActualizarResumen()
    {
        string car = string.IsNullOrEmpty(selectedCarId) ? "(sin carro)" : selectedCarId;
        string lvl = string.IsNullOrEmpty(selectedScene) ? "(sin nivel)" : selectedScene;
        if (txtResumen) txtResumen.text = $"Carro: {car} | Nivel: {lvl}";
    }

    void ActualizarComenzar()
    {
        if (!btnComenzar) return;
        btnComenzar.interactable = !string.IsNullOrEmpty(selectedCarId) && !string.IsNullOrEmpty(selectedScene);
    }
}

