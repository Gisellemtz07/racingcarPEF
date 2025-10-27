using UnityEngine;

public class Meta : MonoBehaviour
{
    public int Puntos = -1;

    // Anti-spam por si el collider se queda traslapado unos frames
    public float cooldown = 0.4f;
    float lastTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Anti rebote
        if (Time.time - lastTime < cooldown) return;
        lastTime = Time.time;

        // 1) Suma tu contador local (si lo usas)
        Puntos++;
        Debug.Log("Puntos: " + Puntos);

        // 2) Suma VUELTA en el HUD (busca GameMetrics en el Player o su padre)
        var metrics = other.GetComponentInParent<GameMetrics>();
        if (metrics != null)
        {
            metrics.RegisterLap();
        }
        else
        {
            Debug.LogWarning("[Meta] El Player no tiene GameMetrics (o no está en el mismo objeto/padre).");
        }
    }
}

