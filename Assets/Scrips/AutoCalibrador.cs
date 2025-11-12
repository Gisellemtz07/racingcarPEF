using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AutoCalibrador : MonoBehaviour
{
    [Header("Referencias")]
    public Transform carro;                 // Car (Transform)
    public Transform zonaCalibracion;       // Objeto "ZonaCalibracion"
    public Image indicadorUI;               // Image (UI) en Canvas
    public GameObject guiaVisual;           // (opcional) marco/guía en escena

    [Header("UI de cuenta regresiva")]
    public CanvasGroup countdownPanel;      // CanvasGroup del panel
    public TMP_Text countdownText;          // Texto TMP dentro del panel

    [Header("Parámetros")]
    public float delayAntesDeContar = 0.3f; // pequeña pausa tras hacer snap
    public int cuentaRegresiva = 3;         // 3...2...1...
    public float tiempoParaVerde = 0.8f;    // segundos para pasar de rojo a verde

    private float tiempoColor = 0f;

    void Start()
    {
        // 🔹 Asegura refs
        if (carro == null)
        {
            var carObj = GameObject.FindWithTag("Player");
            if (carObj) carro = carObj.transform;
        }
        if (zonaCalibracion == null)
        {
            var z = GameObject.Find("ZonaCalibracion");
            if (z) zonaCalibracion = z.transform;
        }

        // 🔹 UI estado inicial
        if (indicadorUI != null)
            indicadorUI.color = Color.red;

        if (countdownPanel) countdownPanel.alpha = 0;
        if (guiaVisual) guiaVisual.SetActive(true);

        // 🔹 Bloquea controles de carrera mientras calibramos
        RaceManager.DisableControls();

        // ✅ Coloca el carro EXACTO en la zona (snap)
        if (carro != null && zonaCalibracion != null)
        {
            carro.position = zonaCalibracion.position;
            carro.rotation = zonaCalibracion.rotation;

            // desactiva input directo del carro por seguridad
            var mover = carro.GetComponent<CarFollowMouseCursor2D>();
            if (mover) mover.enabled = false;

            // Inicia la animación de color y cuenta regresiva
            StartCoroutine(AnimacionCalibracion());
        }
        else
        {
            Debug.LogWarning("[AutoCalibrador] Falta Car (tag Player) o 'ZonaCalibracion' en la escena.");
        }
    }

    IEnumerator AnimacionCalibracion()
    {
        // 🔹 Transición de rojo → verde en tiempoParaVerde segundos
        tiempoColor = 0f;
        while (tiempoColor < tiempoParaVerde)
        {
            tiempoColor += Time.deltaTime;
            if (indicadorUI != null)
                indicadorUI.color = Color.Lerp(Color.red, Color.green, tiempoColor / tiempoParaVerde);
            yield return null;
        }

        // 🔹 Color verde fijo
        if (indicadorUI != null)
            indicadorUI.color = Color.green;

        // 🔹 Oculta la guía visual, pero deja el círculo visible
        if (guiaVisual != null)
            guiaVisual.SetActive(false);

        // 🔹 Pausa y lanza cuenta regresiva
        yield return new WaitForSeconds(delayAntesDeContar);
        StartCoroutine(InicioTrasSnap());
    }

    IEnumerator InicioTrasSnap()
    {
        // Fade-in panel
        if (countdownPanel) StartCoroutine(Fade(countdownPanel, 0f, 1f, 0.35f));

        if (countdownText)
        {
            for (int i = cuentaRegresiva; i > 0; i--)
            {
                countdownText.text = $" Calibración completada\nIniciando en {i}...";
                yield return new WaitForSeconds(1f);
            }
        }

        // Fade-out
        if (countdownPanel) StartCoroutine(Fade(countdownPanel, 1f, 0f, 0.35f));
        yield return new WaitForSeconds(0.35f);

        // ✅ Activa movimiento e input
        var mover = carro ? carro.GetComponent<CarFollowMouseCursor2D>() : null;
        if (mover) mover.enabled = true;
        RaceManager.EnableControls();

        // 🔹 Desactiva el indicador después de iniciar
        if (indicadorUI != null)
            indicadorUI.gameObject.SetActive(false);

        // Listo: arrancamos 🚗
        gameObject.SetActive(false);
    }

    IEnumerator Fade(CanvasGroup cg, float a, float b, float t)
    {
        float e = 0f;
        while (e < t)
        {
            e += Time.deltaTime;
            cg.alpha = Mathf.Lerp(a, b, e / t);
            yield return null;
        }
        cg.alpha = b;
    }
}
