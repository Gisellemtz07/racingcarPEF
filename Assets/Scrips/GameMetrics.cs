using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class GameMetrics : MonoBehaviour
{
    // ====== UI (TMP) ======
    public TextMeshProUGUI velocidadTMP;       
    public TextMeshProUGUI vueltasTMP;         
    public TextMeshProUGUI ultimaVueltaTMP;    
    public TextMeshProUGUI mejorVueltaTMP;     
    public TextMeshProUGUI promedioVueltaTMP;  
    public TextMeshProUGUI fueraTMP;           // Tiempo fuera de pista
    public TextMeshProUGUI salidasTMP;         // Veces que se salió de la pista

    // ====== Vueltas / tiempos ======
    private Rigidbody2D rb;
    public int currentLaps { get; private set; } = 0;
    public int targetLaps = 3;

    private float lapStartTime = 0f;
    private float lastLap = 0f;
    private float bestLap = Mathf.Infinity;
    private float avgLap = 0f;
    private bool racePrimed = false;

    // ====== Métricas fuera de pista ======
    [Header("Fuera de pista")]
    public LayerMask offTrackMask;
    private bool fueraDePista = false;
    private float tiempoInicioFuera = 0f;
    private float tiempoTotalFuera = 0f;
    private int vecesFuera = 0;

    // ====== Propiedades públicas para otros scripts ======
// ====== Propiedades públicas para otros scripts ======
public int VueltasCompletadas => currentLaps;
public int VueltasObjetivo => targetLaps;
public float MejorVuelta => bestLap;
public float PromedioVuelta => avgLap;
public int GolpesTotales => 0; // compatibilidad con SessionRecorder
public float Fuera => Fuera;
public int VecesFuera => vecesFuera;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Autoconectar TMPs si faltan
        if (!velocidadTMP) velocidadTMP = FindTMP("velocidad");
        if (!vueltasTMP) vueltasTMP = FindTMP("vueltas");
        if (!ultimaVueltaTMP) ultimaVueltaTMP = FindTMP("ultimavuelta");
        if (!mejorVueltaTMP) mejorVueltaTMP = FindTMP("mejorvuelta");
        if (!promedioVueltaTMP) promedioVueltaTMP = FindTMP("promediovuelta");
        if (!fueraTMP) fueraTMP = FindTMP("fuerapista");
        if (!salidasTMP) salidasTMP = FindTMP("salidapista");

        UpdateUI();
    }

    void Update()
    {
        if (velocidadTMP)
            velocidadTMP.text = $"Vel: {rb.linearVelocity.magnitude:0.00}";
    }

    // ====== Inyección desde Story Mode ======
    public void SetTargetLaps(int laps)
    {
        targetLaps = Mathf.Max(1, laps);
        ResetSession(keepTarget: true);
        Debug.Log($"[GameMetrics] targetLaps fijado a {targetLaps} y sesión reiniciada");
    }

    public void ResetSession(bool keepTarget = false)
    {
        currentLaps = 0;
        lapStartTime = 0f;
        lastLap = 0f;
        bestLap = Mathf.Infinity;
        avgLap = 0f;
        racePrimed = false;
        tiempoInicioFuera = 0f;
        tiempoTotalFuera = 0f;
        vecesFuera = 0;
        if (!keepTarget) targetLaps = Mathf.Max(1, targetLaps);
        UpdateUI();
    }

    // ====== Vueltas ======
    public void RegisterLap()
    {
        float now = Time.time;

        if (!racePrimed)
        {
            racePrimed = true;
            lapStartTime = now;
            UpdateUI();
            Debug.Log("[GameMetrics] Carrera armada (vueltas = 0).");
            return;
        }

        lastLap = (lapStartTime > 0f) ? now - lapStartTime : 0f;
        lapStartTime = now;

        currentLaps++;
        if (lastLap > 0f && lastLap < bestLap) bestLap = lastLap;
        avgLap = (currentLaps == 1) ? lastLap : ((avgLap * (currentLaps - 1)) + lastLap) / currentLaps;

        UpdateUI();

        if (currentLaps >= targetLaps)
        {
            Debug.Log($"[GameMetrics] Vueltas objetivo alcanzadas ({currentLaps}/{targetLaps}).");
            LevelCompleteInvoker.SignalComplete();
        }
    }

    // ====== Fuera de pista ======
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & offTrackMask.value) != 0)
        {
            fueraDePista = true;
            tiempoInicioFuera = Time.time;
            vecesFuera++;
            Debug.Log($"[GameMetrics]  Salió de pista (vez #{vecesFuera})");
            UpdateUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & offTrackMask.value) != 0 && fueraDePista)
        {
            fueraDePista = false;
            float duracion = Time.time - tiempoInicioFuera;
            tiempoTotalFuera += duracion;
            Debug.Log($"[GameMetrics]  Regresó a pista (fuera {duracion:0.00}s, total {tiempoTotalFuera:0.00}s)");
            UpdateUI();
        }
    }

    // ====== Helpers ======
    TextMeshProUGUI FindTMP(string name)
    {
        var go = GameObject.Find(name);
        return go ? go.GetComponent<TextMeshProUGUI>() : null;
    }

    string FormatSec(float s)
    {
        if (s <= 0f || float.IsInfinity(s)) return "--";
        int min = Mathf.FloorToInt(s / 60f);
        float sec = s - min * 60f;
        return (min > 0) ? $"{min}:{sec:00.00}" : $"{sec:0.00}s";
    }

    void UpdateUI()
{
    bool esHistoria = false;
    if (GameModeManager.Instance != null)
        esHistoria = GameModeManager.Instance.CurrentMode == GameModeManager.GameMode.Story;

    // === Mostrar vueltas ===
    if (vueltasTMP)
    {
        vueltasTMP.gameObject.SetActive(true);

        if (esHistoria)
        {
            // Modo historia: con total
            vueltasTMP.text = $"Vueltas: {currentLaps}/{targetLaps}";
        }
        else
        {
            // Modo libre: solo las vueltas completadas
            vueltasTMP.text = $"Vueltas: {currentLaps}";
        }
    }

    if (ultimaVueltaTMP)
        ultimaVueltaTMP.text = $"Última: {FormatSec(lastLap)}";

    if (mejorVueltaTMP)
        mejorVueltaTMP.text = $"Mejor: {FormatSec(bestLap)}";

    if (promedioVueltaTMP)
        promedioVueltaTMP.text = $"Prom: {(avgLap > 0 ? FormatSec(avgLap) : "--")}";

    if (fueraTMP)
        fueraTMP.text = $"Fuera pista: {tiempoTotalFuera:0.0}s";

    if (salidasTMP)
        salidasTMP.text = $"Salidas: {vecesFuera}";
}


}


