using System.Collections;
using UnityEngine;
using TMPro;

public class StartCountdownUI : MonoBehaviour
{
    [Header("Opcional: arrástrame tu TMP aquí")]
    public TextMeshProUGUI countdownTMP;

    [Header("Timing (seg)")]
    public float initialDelay = 0.5f;
    public float step = 1.0f;
    public float goHold = 0.8f;
    public string goText = "¡YA!";

    void Awake()
    {
        // Asegura bloqueo global al arrancar
        RaceManager.DisableControls();

        // Autowire: si no arrastraste el TMP, lo busco
        if (countdownTMP == null)
            countdownTMP = GetComponent<TextMeshProUGUI>();

        // Si aun así no hay TMP, lo digo en consola
        if (countdownTMP == null)
            Debug.LogError("[Countdown] No encontré TextMeshProUGUI en este objeto.");
    }

    void OnEnable()
    {
        // Por si el objeto estaba desactivado y lo activas justo al empezar
        StartCoroutine(CoCount());
    }

    IEnumerator CoCount()
    {
        if (countdownTMP == null) yield break;

        // Asegurar que el objeto y el texto están visibles
        gameObject.SetActive(true);
        countdownTMP.enabled = true;

        Debug.Log("[Countdown] Inicio en " + initialDelay + "s");
        yield return new WaitForSecondsRealtime(initialDelay);

        int[] nums = { 3, 2, 1 };
        foreach (int n in nums)
        {
            countdownTMP.text = n.ToString();
            Debug.Log("[Countdown] " + n);
            yield return new WaitForSecondsRealtime(step);
        }

        countdownTMP.text = goText;
        Debug.Log("[Countdown] GO");
        RaceManager.EnableControls();

        yield return new WaitForSecondsRealtime(goHold);

        // Oculto el texto al terminar
        countdownTMP.gameObject.SetActive(false);
    }
}
