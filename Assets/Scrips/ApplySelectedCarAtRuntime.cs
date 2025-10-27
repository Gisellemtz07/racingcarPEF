using UnityEngine;

public class ApplySelectedCarAtRuntime : MonoBehaviour
{
    [Header("Prefabs disponibles (arrástralos)")]
    public GameObject[] carPrefabs;     // Carro_0, Carro_1, ...

    [Header("Dónde aparecer")]
    public Transform spawnPoint;        // opcional si no tienes placeholder

    [Header("Opciones")]
    public string fallbackPlayerName = "Car";
    public bool renameNewAsCar = true;

    void Start()
    {
        string id = (GameSession.Instance != null && !string.IsNullOrEmpty(GameSession.Instance.SelectedCar))
            ? GameSession.Instance.SelectedCar
            : PlayerPrefs.GetString("SelectedCarId", "Carro_0");

        GameObject placeholder = GameObject.FindWithTag("Player");
        if (placeholder == null) placeholder = GameObject.Find(fallbackPlayerName);

        Vector3 pos = placeholder ? placeholder.transform.position :
                     (spawnPoint ? spawnPoint.position :
                     (Camera.main ? Camera.main.transform.position : Vector3.zero));
        Quaternion rot = placeholder ? placeholder.transform.rotation :
                        (spawnPoint ? spawnPoint.rotation : Quaternion.identity);

        GameObject prefab = null;
        foreach (var p in carPrefabs) if (p && p.name == id) { prefab = p; break; }
        if (prefab == null) { Debug.LogError($"No encontré prefab '{id}' en carPrefabs."); return; }

        if (placeholder) Destroy(placeholder);
        var newCar = Instantiate(prefab, pos, rot);
        newCar.tag = "Player";
        if (renameNewAsCar) newCar.name = "Car";
    }
}
