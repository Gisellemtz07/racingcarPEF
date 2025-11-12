using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TMP_Text txtUsuario;

    void Start()
    {
        if (GameSession.Instance != null)
            txtUsuario.text = "👤 Usuario: " + GameSession.Instance.usuarioActual;
    }
}

