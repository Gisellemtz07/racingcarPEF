using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelController : MonoBehaviour
{
    public GameObject popupUI;
    public GameObject botonFinalizar;

    void Start()
    {
        if (GameModeManager.Instance.CurrentMode == GameModeManager.GameMode.Story)
        {
            botonFinalizar.SetActive(true);
        }
        else
        {
            botonFinalizar.SetActive(false);
        }

        popupUI.SetActive(false);
    }

    public void FinalizarNivel()
    {
        popupUI.SetActive(true);
    }

    public void AvanzarSiguienteNivel()
    {
        if (GameModeManager.Instance.CurrentMode == GameModeManager.GameMode.Story)
        {
            GameModeManager.Instance.AvanzarNivel();
            string siguiente = GameModeManager.Instance.GetNivelActual();

            if (siguiente != null)
                SceneManager.LoadScene(siguiente);
            else
                SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

