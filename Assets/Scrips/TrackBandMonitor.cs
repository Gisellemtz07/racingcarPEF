using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class OutsideTimeByWalls : MonoBehaviour
{
    [Header("Paredes (PolygonCollider2D)")]
    public Collider2D pared1;
    public Collider2D pared2;

    [Header("UI (opcional)")]
    public TextMeshProUGUI vecesFueraTMP;   // "Fuera (veces): X"
    public TextMeshProUGUI tiempoFueraTMP;  // "Fuera (tiempo): 00.00s"

    [Header("Modo detección")]
    [Tooltip("Si tus paredes tienen IsTrigger=ON, deja esto en true.\nSi NO usas triggers, desmárcalo y se checa por punto (OverlapPoint).")]
    public bool useTriggers = true;

    Rigidbody2D rb;

    // Estado
    bool insideP1 = false, insideP2 = false;
    bool isOutside = false;
    int vecesFuera = 0;
    float tiempoFuera = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!pared1) Debug.LogWarning("[OutsideTimeByWalls] Asigna 'pared1'.");
        if (!pared2) Debug.LogWarning("[OutsideTimeByWalls] Asigna 'pared2'.");
    }

    void Update()
    {
        if (!useTriggers)
        {
            // Modo sin triggers: evalúa por punto (posición del rigidbody)
            bool nowInsideP1 = pared1 && pared1.OverlapPoint(rb.position);
            bool nowInsideP2 = pared2 && pared2.OverlapPoint(rb.position);
            UpdateOutsideState(nowInsideP1 || nowInsideP2);
        }

        if (isOutside) tiempoFuera += Time.deltaTime;

        // UI
        if (vecesFueraTMP)  vecesFueraTMP.text  = $"Fuera (veces): {vecesFuera}";
        if (tiempoFueraTMP) tiempoFueraTMP.text = $"Fuera (tiempo): {FormatSec(tiempoFuera)}";
    }

    void UpdateOutsideState(bool outsideNow)
    {
        if (!isOutside && outsideNow)
            vecesFuera++;           // contó una salida (entró a una pared)
        isOutside = outsideNow;
    }

    // ---------- Modo con triggers ----------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTriggers) return;
        if (other == pared1) { insideP1 = true;  UpdateOutsideState(true); }
        else if (other == pared2) { insideP2 = true;  UpdateOutsideState(true); }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!useTriggers) return;
        if (other == pared1) { insideP1 = false; UpdateOutsideState(insideP2); }
        else if (other == pared2) { insideP2 = false; UpdateOutsideState(insideP1); }
    }

    // ---------- Util ----------
    string FormatSec(float s)
    {
        if (s <= 0f) return "0.00s";
        int min = Mathf.FloorToInt(s / 60f);
        float sec = s - min * 60f;
        return (min > 0) ? $"{min}:{sec:00.00}" : $"{sec:0.00}s";
    }
}
