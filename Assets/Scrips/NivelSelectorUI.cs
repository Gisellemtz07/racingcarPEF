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
        // Establecer modo Libre al entrar aquí
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.FreePlay);

        foreach (var nivel in niveles)
        {
            // Mostrar la imagen del nivel en su botón
            nivel.imagenUI.sprite = nivel.imagenNivel;

            // Guardar referencia local del nivel dentro del bucle
            string nivelCopia = nivel.nombreNivel;

            // Configurar qué pasa al presionar el botón
            nivel.botonJugar.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nivelCopia);
            });
        }
    }
}


