// EMGCSVPlayer.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class EMGCSVPlayer : MonoBehaviour
{
    public TextAsset csv; // time_s, emg_uV, rect_uV, rms250_uV, mdf_Hz, mpf_Hz
    public bool playOnStart = true;
    [Range(0.25f, 3f)] public float playbackSpeed = 1f;

    public struct Sample { public float t, emg, rect, rms, mdf, mpf; }
    public Sample Current { get; private set; }
    public event Action<Sample> OnSample;

    float _t;
    float[] _time, _emg, _rect, _rms, _mdf, _mpf;

    void Awake()
    {
        if (!csv) { Debug.LogError("[EMGCSVPlayer] Faltó asignar el CSV."); return; }
        Load(csv.text);
    }
    void Start() { if (playOnStart) _t = 0f; }

    void Update()
    {
        if (_time == null || _time.Length == 0) return;
        _t += Time.deltaTime * playbackSpeed;
        int i = Array.BinarySearch(_time, _t);
        if (i < 0) i = ~i;
        i = Mathf.Clamp(i, 0, _time.Length - 1);

        Current = new Sample {
            t=_time[i], emg=_emg[i], rect=_rect[i], rms=_rms[i], mdf=_mdf[i], mpf=_mpf[i]
        };
        OnSample?.Invoke(Current);
    }

    void Load(string text)
    {
        var rows = new List<string>(text.Split(new[] { '\n','\r' }, StringSplitOptions.RemoveEmptyEntries));
        if (rows.Count <= 1) { Debug.LogError("[EMGCSVPlayer] CSV vacío."); return; }
        rows.RemoveAt(0);

        int N = rows.Count;
        _time = new float[N]; _emg = new float[N]; _rect = new float[N];
        _rms  = new float[N]; _mdf = new float[N]; _mpf  = new float[N];

        var ci = CultureInfo.InvariantCulture;
        for (int i = 0; i < N; i++)
        {
            var c = rows[i].Split(',');
            if (c.Length < 6) continue;
            _time[i] = float.Parse(c[0], ci);
            _emg[i]  = float.Parse(c[1], ci);
            _rect[i] = float.Parse(c[2], ci);
            _rms[i]  = float.Parse(c[3], ci);
            _mdf[i]  = Try(c[4], ci);
            _mpf[i]  = Try(c[5], ci);
        }
        float Try(string s, IFormatProvider ci) => float.TryParse(s, NumberStyles.Float, ci, out var v) ? v : float.NaN;
    }
}

