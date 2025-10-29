using System.IO;
using UnityEngine;

public static class SessionRecorder
{
    public static void GuardarResultadoNivel(
        string username,
        string sceneName,
        GameMetrics metrics)
    {
        if (metrics == null) return;

        string dataFolder = Path.Combine(Application.persistentDataPath, "sesiones");
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        string filePath = Path.Combine(dataFolder, username + ".log");

        string line = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
                      $"nivel={sceneName}," +
                      $"vueltas={metrics.VueltasCompletadas}/{metrics.VueltasObjetivo}," +
                      $"golpes={metrics.GolpesTotales}," +
                      $"mejorVuelta={metrics.MejorVuelta}," +
                      $"promedioVuelta={metrics.PromedioVuelta}";

        File.AppendAllLines(filePath, new string[] { line });

        Debug.Log($"[SessionRecorder] Guardado: {line}");
    }
}

