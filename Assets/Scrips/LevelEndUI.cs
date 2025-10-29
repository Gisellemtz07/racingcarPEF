using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEndUI : MonoBehaviour
{
    public GameObject panelRoot;
    public TMP_Text resumenTMP;
    public Button btnSiguiente;
    public Button btnMenu;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void ShowEndScreen(string siguienteEscena, bool hayMas, string resumen)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resumenTMP != null)
            resumenTMP.text = resumen;

        if (btnSiguiente != null)
        {
            btnSiguiente.gameObject.SetActive(hayMas);
            btnSiguiente.onClick.RemoveAllListeners();
            btnSiguiente.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(siguienteEscena))
                    SceneManager.LoadScene(siguienteEscena);
            });
        }

        if (btnMenu != null)
        {
            btnMenu.onClick.RemoveAllListeners();
            btnMenu.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        }
    }
}
