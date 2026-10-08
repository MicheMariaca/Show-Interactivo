using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LaunchOnNoise : MonoBehaviour
{
    public MicLoudness mic;
    public float listenTime = 4f;                       // segundos de "carga"
    public float maxForce = 60f;                        // fuerza con el ruido máximo
    public Vector2 direction = new Vector2(0.3f, 1f);   // hacia dónde sale volando
    public float spinSpeed = 360f;
    public float shakeAmount = 0.1f;

    Rigidbody2D rb;
    Vector3 startPos;
    float timer, peak;
    bool launched;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        startPos = transform.position;
    }

    void Update()
    {
        if (launched)
        {
            transform.Rotate(0, 0, spinSpeed * Mathf.Max(0.2f, peak) * Time.deltaTime);
            return;
        }

        timer += Time.deltaTime;
        peak = Mathf.Max(peak, mic.loudness01);

        // temblor según el ruido actual
        transform.position = startPos + (Vector3)(Random.insideUnitCircle * shakeAmount * mic.loudness01);

        if (timer >= listenTime) Launch();
    }

    void Launch()
    {
        launched = true;
        transform.position = startPos;
        float force = maxForce * peak * peak;   // al cuadrado: el silencio casi no mueve, el grito sí
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }
}