using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HistoriaSetupUI : MonoBehaviour
{
    [System.Serializable]
    public class PasoConfigUI
    {
        public TMP_Text pasoLabel;
        public TMP_Dropdown nivelDropdown;
        public TMP_InputField vueltasInput;
        public Image previewImage;
    }

    [Header("Slots de la sesión")]
    public PasoConfigUI[] pasos;

    [Header("Botones")]
    public Button empezarButton;
    public Button borrarButton;

    [Header("Catálogo de niveles disponibles")]
    public string[] nombresEscenasDisponibles;
    public Sprite[] previewsDeEscenas;

    private void Start()
    {
        // Llenar dropdowns con nombres de escenas
        for (int i = 0; i < pasos.Length; i++)
        {
            var dropdown = pasos[i].nivelDropdown;
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(nombresEscenasDisponibles));
            UpdatePreviewForStep(i);

            int indexCopia = i;
            dropdown.onValueChanged.AddListener(_ => UpdatePreviewForStep(indexCopia));
        }

        empezarButton.onClick.AddListener(OnEmpezarSesion);
        borrarButton.onClick.AddListener(OnBorrarConfig);
    }

    private void UpdatePreviewForStep(int pasoIndex)
    {
        if (pasoIndex < 0 || pasoIndex >= pasos.Length) return;

        int selectedLevelIdx = pasos[pasoIndex].nivelDropdown.value;

        if (pasos[pasoIndex].previewImage != null &&
            previewsDeEscenas != null &&
            selectedLevelIdx >= 0 &&
            selectedLevelIdx < previewsDeEscenas.Length)
        {
            pasos[pasoIndex].previewImage.sprite = previewsDeEscenas[selectedLevelIdx];
        }
    }

    private void OnBorrarConfig()
    {
        foreach (var p in pasos)
        {
            p.nivelDropdown.value = 0;
            p.vueltasInput.text = "";
        }

        GameModeManager.Instance.nivelActual = 0;
        GameModeManager.Instance.nivelesHistoria.Clear();
        GameModeManager.Instance.vueltasPorNivel.Clear();
    }

    private void OnEmpezarSesion()
    {
        var manager = GameModeManager.Instance;
        manager.nivelesHistoria.Clear();
        manager.vueltasPorNivel.Clear();
        manager.nivelActual = 0;
        manager.SetMode(GameModeManager.GameMode.Story);

        for (int i = 0; i < pasos.Length; i++)
        {
            int selectedIdx = pasos[i].nivelDropdown.value;
            string escena = nombresEscenasDisponibles[selectedIdx];

            int vueltas = 3;
            if (!string.IsNullOrEmpty(pasos[i].vueltasInput.text))
                int.TryParse(pasos[i].vueltasInput.text, out vueltas);

            manager.nivelesHistoria.Add(escena);
            manager.vueltasPorNivel[escena] = vueltas;
        }

        // 🔹 Asegura persistencia del manager
        DontDestroyOnLoad(manager.gameObject);

        // 🔹 Crear el controlador global de niveles si no existe
        EnsureNivelControllerListenerExists();

        // 🔹 Cargar la escena de Login
        Debug.Log("[HistoriaSetupUI] Redirigiendo al login antes de iniciar historia...");
        SceneManager.LoadScene("Login");
    }

    private void EnsureNivelControllerListenerExists()
{
    if (Object.FindFirstObjectByType<NivelControllerGlobal>() == null)
    {
        var go = new GameObject("NivelControllerGlobal");
        go.AddComponent<NivelControllerGlobal>();
        DontDestroyOnLoad(go);
    }
}

}
