using UnityEngine;

public class Seguir : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform target;                 // Déjalo vacío: se buscará por tag "Player" al iniciar
    public string targetTag = "Player";

    [Header("Movimiento de cámara")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 5f;

    void Start()
    {
        TryFindTargetIfNull();
        // Teletransporta la cámara al inicio para evitar salto
        if (target != null) transform.position = target.position + offset;
    }

    void LateUpdate()  // mejor que Update para cámara
    {
        if (target == null)
        {
            TryFindTargetIfNull();
            return; // evita usar target.position si aún no existe
        }

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    void TryFindTargetIfNull()
    {
        if (target != null) return;

        var player = GameObject.FindWithTag(targetTag);
        if (player != null)
        {
            target = player.transform;
        }
    }
}

