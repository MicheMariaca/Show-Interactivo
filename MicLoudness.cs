using UnityEngine;

public class MicLoudness : MonoBehaviour
{
    [Header("Calibración (dBFS)")]
    public float minDb = -45f;   // por debajo de esto = silencio (0)
    public float maxDb = -10f;   // por encima de esto = máximo (1)

    [Header("Lectura (solo para ver)")]
    public float dbValue;
    [Range(0, 1)] public float loudness01;

    const int SAMPLES = 512;
    AudioClip clip;
    string device;
    float[] data = new float[SAMPLES];

    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No se encontró micrófono");
            enabled = false;
            return;
        }
        device = Microphone.devices[0];
        clip = Microphone.Start(device, true, 1, 44100);
    }

    void Update()
    {
        int pos = Microphone.GetPosition(device) - SAMPLES;
        if (pos < 0) return;

        clip.GetData(data, pos);

        float sum = 0f;
        for (int i = 0; i < SAMPLES; i++) sum += data[i] * data[i];
        float rms = Mathf.Sqrt(sum / SAMPLES);

        dbValue = rms > 0.0001f ? 20f * Mathf.Log10(rms) : -80f;
        loudness01 = Mathf.InverseLerp(minDb, maxDb, dbValue);
    }

    void OnDestroy()
    {
        if (device != null && Microphone.IsRecording(device))
            Microphone.End(device);
    }
}