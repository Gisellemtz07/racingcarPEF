using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NivelSelectorUI : MonoBehaviour
{
    [System.Serializable]
    public class NivelInfo
    {
        public string nombreNivel;
        public Sprite imagenNivel;
        public Button botonJugar;
        public Image imagenUI;
    }

    public NivelInfo[] niveles;

    void Start()
    {
        GameModeManager.Instance.currentMode = GameModeManager.GameMode.Libre;

        foreach (var nivel in niveles)
        {
            nivel.imagenUI.sprite = nivel.imagenNivel;
            nivel.botonJugar.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nivel.nombreNivel);
            });
        }
    }
}

