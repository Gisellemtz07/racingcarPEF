using UnityEngine;

public class CarSkinLoader : MonoBehaviour
{
    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.Log("[CarSkinLoader] No hay GameSession; dejo el sprite actual.");
            return;
        }

        // nombre tal cual viene del selector (p.ej. "Carro Amarillo")
        string rawName = GameSession.Instance.GetCarroActual();
        if (string.IsNullOrEmpty(rawName))
        {
            Debug.Log("[CarSkinLoader] No hay carro seleccionado; dejo el sprite actual.");
            return;
        }

        // versiones sanitizadas para buscar
        string noSpaces = rawName.Replace(" ", "");
        string pathNoSpaces = $"Carros/{noSpaces}";
        string pathRaw = $"Carros/{rawName}";

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("[CarSkinLoader] Este objeto no tiene SpriteRenderer.");
            return;
        }

        // 1) Intento como SPRITE (sin espacios)
        Sprite spr = Resources.Load<Sprite>(pathNoSpaces);

        // 2) Intento como SPRITE (con espacios, por si acaso)
        if (spr == null) spr = Resources.Load<Sprite>(pathRaw);

        // 3) Si no hay sprite directo, intento PREFAB y le tomo el sprite
        if (spr == null)
        {
            GameObject prefab = Resources.Load<GameObject>(pathNoSpaces);
            if (prefab == null) prefab = Resources.Load<GameObject>(pathRaw);

            if (prefab != null)
            {
                var prefabSR = prefab.GetComponentInChildren<SpriteRenderer>();
                if (prefabSR != null) spr = prefabSR.sprite;
            }
        }

        if (spr != null)
        {
            sr.sprite = spr;
            Debug.Log($"[CarSkinLoader] Sprite aplicado: '{rawName}' (ruta encontrada).");
            return;
        }

        // --- Depuración: listar qué hay en Resources/Carros ---
        var sprites = Resources.LoadAll<Sprite>("Carros");
        var prefabs = Resources.LoadAll<GameObject>("Carros");

        string sNames = sprites != null && sprites.Length > 0
            ? string.Join(", ", System.Array.ConvertAll(sprites, x => x.name))
            : "(ningún sprite)";

        string pNames = prefabs != null && prefabs.Length > 0
            ? string.Join(", ", System.Array.ConvertAll(prefabs, x => x.name))
            : "(ningún prefab)";

        Debug.LogWarning(
            $"[CarSkinLoader]  No se encontró sprite ni prefab para '{rawName}'.\n" +
            $"Busqué: '{pathNoSpaces}' y '{pathRaw}'.\n" +
            $"Sprites en Resources/Carros: {sNames}\n" +
            $"Prefabs en Resources/Carros: {pNames}"
        );
    }
}
