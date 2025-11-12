using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuModeSelector : MonoBehaviour
{
    public void SelectStoryMode()
    {
        // Garantizar que exista el GameModeManager
        if (GameModeManager.Instance == null)
        {
            var go = new GameObject("_GameModeManager");
            go.AddComponent<GameModeManager>();
        }

        // Cambiar modo a HISTORIA
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.Story);
        Debug.Log("[MenuModeSelector] Entrando a modo HISTORIA → HistoriaSetup");

        // Cargar directamente la escena de configuración de historia
        SceneManager.LoadScene("HistoriaSetup");
    }

    public void SelectFreePlayMode()
    {
        // Garantizar que exista el GameModeManager
        if (GameModeManager.Instance == null)
        {
            var go = new GameObject("_GameModeManager");
            go.AddComponent<GameModeManager>();
        }

        // Cambiar modo a LIBRE
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.FreePlay);
        Debug.Log("[MenuModeSelector] Entrando a modo LIBRE → SeleccionCarroPista");

        // Cargar escena del modo libre (selección de carro/pista)
        SceneManager.LoadScene("SeleccionCarroPista");
    }
}
