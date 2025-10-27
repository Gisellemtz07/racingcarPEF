using TMPro;
using UnityEngine;

[RequireComponent(typeof(CarFollowMouseCursor2D))]
public class FatigueAdapter : MonoBehaviour
{
    public EMGCSVPlayer emgPlayer;
    public CarFollowMouseCursor2D car;

    public TextMeshProUGUI rmsTMP, mpfTMP, fatigueTMP;

    public float baselineSeconds = 5f;           // 3–5 para probar rápido
    [Range(0.1f, 0.6f)] public float fullFatigueDrop = 0.20f; // 20% = fatiga 100%
    [Range(0f, 0.9f)]   public float maxSpeedReduction = 0.80f; // baja 80% a fatiga 100%

    int _nAcc; float _mpfAcc; 
    float _mpfBaseline = float.NaN;

    // Getters útiles para HUD
    public float Fatigue01 { get; private set; } = 0f;
    public float MPFBaseline => _mpfBaseline;
    public float CurrentMPF  { get; private set; } = float.NaN;

    void Awake()
    {
        if (!car) car = GetComponent<CarFollowMouseCursor2D>();
    }

    void OnEnable()
    {
        ResetBaseline();
        if (emgPlayer != null) emgPlayer.OnSample += OnEMG;
    }

    void OnDisable()
    {
        if (emgPlayer != null) emgPlayer.OnSample -= OnEMG;
    }

    public void ResetBaseline()
    {
        _nAcc = 0; _mpfAcc = 0f;
        _mpfBaseline = float.NaN;
        Fatigue01 = 0f; CurrentMPF = float.NaN;
        if (car) car.SetSpeedMultiplier(1f);
    }

    void OnEMG(EMGCSVPlayer.Sample s)
    {
        // 1) Construye baseline hasta baselineSeconds
        if (float.IsNaN(_mpfBaseline) && !float.IsNaN(s.mpf))
        {
            if (s.t < baselineSeconds) { _mpfAcc += s.mpf; _nAcc++; }
            else if (_nAcc > 0) { _mpfBaseline = _mpfAcc / _nAcc; }
        }

        // 2) Calcula fatiga
        float fatigue01 = 0f;
        if (!float.IsNaN(_mpfBaseline) && _mpfBaseline > 0f && !float.IsNaN(s.mpf))
        {
            float dropFrac = (_mpfBaseline - s.mpf) / (_mpfBaseline * Mathf.Max(1e-6f, fullFatigueDrop));
            fatigue01 = Mathf.Clamp01(dropFrac);
        }

        // Guarda para HUD
        CurrentMPF = s.mpf;
        Fatigue01  = fatigue01;

        // 3) Aplica multiplicador al carro
        float k = 1f - maxSpeedReduction * fatigue01; // k de 1 a (1-maxReducción)
        if (car) car.SetSpeedMultiplier(Mathf.Clamp01(k));

        // 4) UI
        if (rmsTMP)     rmsTMP.text     = $"RMS: {s.rms:0.0} µV";
        if (mpfTMP)     mpfTMP.text     = float.IsNaN(s.mpf) ? "MPF: —" : $"MPF: {s.mpf:0} Hz";
        if (fatigueTMP) fatigueTMP.text = $"Fatiga: {Mathf.RoundToInt(fatigue01 * 100f)}%";
    }
}
