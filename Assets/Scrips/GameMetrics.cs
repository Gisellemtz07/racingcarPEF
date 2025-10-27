using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class GameMetrics : MonoBehaviour
{
    // ====== UI (TMP) ======
    public TextMeshProUGUI velocidadTMP;       // "velocidad"
    public TextMeshProUGUI vueltasTMP;         // "vueltas"
    public TextMeshProUGUI ultimaVueltaTMP;    // "ultimavuelta"
    public TextMeshProUGUI mejorVueltaTMP;     // "mejorvuelta"
    public TextMeshProUGUI promedioVueltaTMP;  // "promediovuelta"
    public TextMeshProUGUI golpesTMP;          // "golpes"

    // ====== Vueltas / tiempos ======
    private Rigidbody2D rb;
    public int currentLaps { get; private set; } = 0;  // vueltas completadas
    public int targetLaps = 3;                          // objetivo (lo sobreescribe LevelBootstrap)

    private float lapStartTime = 0f;        // cuándo empezó la vuelta en curso
    private float lastLap = 0f;
    private float bestLap = Mathf.Infinity;
    private float avgLap = 0f;

    // ⬇️ NUEVO: “armado” de carrera (primera pasada NO cuenta)
    private bool racePrimed = false;        // false = aún no se ha hecho el primer cruce “de armado”

    // ====== Colisiones ======
    [Header("Colisiones")]
    public LayerMask wallMask;              // capa 'Walls'
    public float collisionCooldown = 0.15f;
    public float minImpactSpeed = 0.6f;
    private float lastHitTime = -999f;
    private int golpesCount = 0;

    // ====== Feedback visual ======
    [Header("Feedback de choque")]
    public Color hitFlashColor = new Color(1f, 0.25f, 0.25f); // rojo suave
    public float hitFlashTime = 0.20f;       // segundos
    public float hitPunchScale = 1.15f;      // escala temporal del TMP

    Color golpesOriginalColor;
    Vector3 golpesOriginalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Autoconectar por nombre si faltan (opcional)
        if (!velocidadTMP)      velocidadTMP      = FindTMP("velocidad");
        if (!vueltasTMP)        vueltasTMP        = FindTMP("vueltas");
        if (!ultimaVueltaTMP)   ultimaVueltaTMP   = FindTMP("ultimavuelta");
        if (!mejorVueltaTMP)    mejorVueltaTMP    = FindTMP("mejorvuelta");
        if (!promedioVueltaTMP) promedioVueltaTMP = FindTMP("promediovuelta");
        if (!golpesTMP)         golpesTMP         = FindTMP("golpes");

        if (golpesTMP)
        {
            golpesOriginalColor = golpesTMP.color;
            golpesOriginalScale = golpesTMP.rectTransform.localScale;
        }

        // Asegura UI en estado inicial (vueltas 0 / target)
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
        ResetSession(keepTarget:true);
        Debug.Log($"[GameMetrics] targetLaps fijado a {targetLaps} y sesión reiniciada");
    }

    /// <summary>
    /// Reinicia contadores de la sesión. Útil al cargar escena.
    /// </summary>
    public void ResetSession(bool keepTarget = false)
    {
        currentLaps = 0;
        lapStartTime = 0f;
        lastLap = 0f;
        bestLap = Mathf.Infinity;
        avgLap = 0f;
        racePrimed = false;    // ⬅️ importante para NO contar la primera pasada
        if (!keepTarget) targetLaps = Mathf.Max(1, targetLaps);
        UpdateUI();
    }

    // ====== Cruce por la línea de meta ======
    // Llama a esto tu Meta.cs (ya lo hace) → NO cuenta la primera pasada
    public void RegisterLap()
    {
        float now = Time.time;

        // Primera vez que cruzas: SOLO arma la carrera (no suma vuelta)
        if (!racePrimed)
        {
            racePrimed = true;
            lapStartTime = now;          // empezamos a medir la PRIMERA vuelta
            UpdateUI();                  // seguirá mostrando 0/target
            Debug.Log("[GameMetrics] Primer cruce: carrera ARMADA (vueltas = 0).");
            return;
        }

        // De aquí en adelante, cada cruce SÍ cuenta una vuelta completa
        lastLap = (lapStartTime > 0f) ? now - lapStartTime : 0f;
        lapStartTime = now;

        currentLaps++;

        if (lastLap > 0f && lastLap < bestLap) bestLap = lastLap;

        if (currentLaps == 1) avgLap = lastLap; // primera vuelta completa define el promedio inicial
        else                  avgLap = ((avgLap * (currentLaps - 1)) + lastLap) / currentLaps;

        UpdateUI();

        // ¿Se alcanzó la meta?
        if (currentLaps >= targetLaps)
        {
            Debug.Log($"[GameMetrics] Vueltas objetivo alcanzadas ({currentLaps}/{targetLaps}). Finalizando nivel…");
            LevelCompleteInvoker.SignalComplete();
        }
    }

    // ====== Colisiones + feedback ======
    void OnCollisionEnter2D(Collision2D c)
    {
        // ¿Es pared?
        if (((1 << c.collider.gameObject.layer) & wallMask.value) == 0) return;

        // filtros
        if (Time.time - lastHitTime < collisionCooldown) return;
        if (c.relativeVelocity.magnitude < minImpactSpeed) return;

        lastHitTime = Time.time;
        golpesCount++;
        UpdateUI();

        if (golpesTMP) StartCoroutine(FlashTMP(golpesTMP, hitFlashColor, hitFlashTime, hitPunchScale));
    }

    IEnumerator FlashTMP(TextMeshProUGUI tmp, Color flashColor, float time, float punchScale)
    {
        Color startColor = tmp.color;
        Vector3 startScale = tmp.rectTransform.localScale;

        tmp.color = flashColor;
        tmp.rectTransform.localScale = startScale * punchScale;

        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / time);
            tmp.color = Color.Lerp(flashColor, golpesOriginalColor, k);
            tmp.rectTransform.localScale = Vector3.Lerp(startScale * punchScale, golpesOriginalScale, k);
            yield return null;
        }
        tmp.color = golpesOriginalColor;
        tmp.rectTransform.localScale = golpesOriginalScale;
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
        if (vueltasTMP)
        {
            if (targetLaps > 0) vueltasTMP.text = $"Vueltas: {currentLaps}/{targetLaps}";
            else                 vueltasTMP.text = $"Vueltas: {currentLaps}";
        }
        if (ultimaVueltaTMP)   ultimaVueltaTMP.text   = $"Última: {FormatSec(lastLap)}";
        if (mejorVueltaTMP)    mejorVueltaTMP.text    = $"Mejor: {FormatSec(bestLap)}";
        if (promedioVueltaTMP) promedioVueltaTMP.text = $"Prom: {(avgLap > 0 ? FormatSec(avgLap) : "--")}";
        if (golpesTMP)         golpesTMP.text         = $"Golpes: {golpesCount}";
    }
}

