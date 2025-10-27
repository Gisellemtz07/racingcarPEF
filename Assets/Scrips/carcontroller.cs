using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class CarFollowMouseCursor2D : MonoBehaviour
{
    [Header("Movimiento directo al cursor")]
    public float followSpeed = 2f;         // velocidad base (será modulada por fatiga)
    public float stopDistance = 0.05f;     // distancia mínima para no vibrar
    public float maxDistFactor = 10f;      // tope del factor por distancia

    [Header("Rotación (opcional)")]
    public bool rotateToCursor = true;
    public float turnSpeed = 360f;         // grados/segundo

    private Rigidbody2D rb;
    [SerializeField] private Camera cam;

    // Escala multiplicativa aplicada por el FatigueAdapter (1 = sin cambio)
    private float speedMultiplier = 1f;

    void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    rb.gravityScale = 0f;
    rb.interpolation = RigidbodyInterpolation2D.Interpolate;

    if (!cam) cam = Camera.main; // respaldo por si olvidas arrastrarla
}


    // Llamado por FatigueAdapter: k en [0..1]
    public void SetSpeedMultiplier(float k)
    {
        // evita números raros
        speedMultiplier = Mathf.Clamp01(k);
    }

    void FixedUpdate()
    {
        if (!cam) cam = Camera.main; // por si la cámara se cargó tarde

        // ⛔ bloqueo: NO leer cursor ni moverse hasta "¡YA!"
        if (!RaceManager.ControlsEnabled)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        Vector2 mouseWorld = GetMouseWorld2D();
        Vector2 pos = rb.position;
        Vector2 toTarget = mouseWorld - pos;
        float dist = toTarget.magnitude;

        if (dist > stopDistance)
        {
            // velocidad base * multiplicador por fatiga * factor por distancia (acotado)
            float distFactor = Mathf.Min(dist, maxDistFactor);
            float speed = followSpeed * speedMultiplier * distFactor;

            Vector2 dir = toTarget.normalized;
            Vector2 vel = dir * speed;

            // mover SOLO con velocity para que los HUDs vean la modulación real
            rb.linearVelocity = vel;

            // rotar hacia el cursor
            if (rotateToCursor)
            {
                float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg - 90f;
                float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newAngle);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    Vector2 GetMouseWorld2D()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 screen = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        Vector2 screen = Input.mousePosition;
#endif
        // Nota: en 2D la cámara suele mirar a Z=-10; usamos la profundidad relativa de la cámara
        float z = -cam.transform.position.z;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, z));
        return new Vector2(world.x, world.y);
    }
}
