using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel;

    void Start()
    {
        popupPanel.SetActive(false);
    }

    public void MostrarPopup()
    {
        popupPanel.SetActive(true);
    }

    public void OcultarPopup()
    {
        popupPanel.SetActive(false);
    }
}

