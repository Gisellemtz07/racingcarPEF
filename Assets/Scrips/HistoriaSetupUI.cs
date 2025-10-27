using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HistoriaSetupUI : MonoBehaviour
{
    public TMP_InputField[] ordenInputs;
    public TMP_InputField[] vueltasInputs;

    public void IniciarHistoria()
    {
        GameModeManager.Instance.currentMode = GameModeManager.GameMode.Historia;
        GameModeManager.Instance.nivelesHistoria.Clear();
        GameModeManager.Instance.vueltasPorNivel.Clear();
        GameModeManager.Instance.nivelActual = 0;

        for (int i = 0; i < ordenInputs.Length; i++)
        {
            string nivel = ordenInputs[i].text;
            if (!string.IsNullOrEmpty(nivel))
            {
                GameModeManager.Instance.nivelesHistoria.Add(nivel);
                int vueltas = int.TryParse(vueltasInputs[i].text, out int v) ? v : 1;
                GameModeManager.Instance.vueltasPorNivel[nivel] = vueltas;
            }
        }

        SceneManager.LoadScene(GameModeManager.Instance.GetNivelActual());
    }
}

