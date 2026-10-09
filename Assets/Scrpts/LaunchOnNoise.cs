using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class LaunchOnNoise : MonoBehaviour
{
    [Header("Referencias")]
    public MicLoudness mic;

    [Header("Nombres de los estados en el Animator")]
    public string estadoConfundido = "Confundido";
    public string estadoImpaciente = "Impaciente";
    public string estadoEnojado = "Enojado";

    [Header("Medidor de rabia (0 a 1)")]
    public float velocidadCarga = 0.5f;
    public float velocidadDescarga = 0.1f;
    public float umbralImpaciente = 0.33f;
    public float umbralEnojado = 0.66f;

    [Header("Transición entre animaciones")]
    public float duracionCambio = 0.05f;

    [Header("Lanzamiento")]
    public float fuerzaMinima = 10f;
    public float fuerzaMaxima = 30f;
    public Vector2 direccion = new Vector2(0.3f, 1f);
    public float velocidadGiro = 360f;
    public float temblor = 0.05f;

    Rigidbody2D rb;
    Animator animator;
    Vector3 posicionInicial;
    float rabia;
    float pico;
    int etapaActual = -1;
    bool lanzado;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        posicionInicial = transform.position;
    }

    void Start()
    {
        CambiarEtapa(0);
    }

    void Update()
    {
        if (lanzado)
        {
            transform.Rotate(0, 0, velocidadGiro * Mathf.Max(0.3f, pico) * Time.deltaTime);
            return;
        }

        float ruido = mic.loudness01;
        pico = Mathf.Max(pico, ruido);

        rabia += (ruido * velocidadCarga - velocidadDescarga) * Time.deltaTime;
        rabia = Mathf.Clamp01(rabia);

        int etapa = 0;
        if (rabia >= umbralEnojado) etapa = 2;
        else if (rabia >= umbralImpaciente) etapa = 1;

        if (etapa != etapaActual) CambiarEtapa(etapa);

        transform.position = posicionInicial + (Vector3)(Random.insideUnitCircle * temblor * rabia);

        if (rabia >= 1f) Lanzar();
    }

    void CambiarEtapa(int etapa)
    {
        bool primera = (etapaActual == -1);
        etapaActual = etapa;

        string nombre = estadoConfundido;
        if (etapa == 1) nombre = estadoImpaciente;
        if (etapa == 2) nombre = estadoEnojado;

        Debug.Log("Etapa: " + nombre);

        if (primera)
            animator.Play(nombre);
        else
            animator.CrossFadeInFixedTime(nombre, duracionCambio);
    }

    void Lanzar()
    {
        lanzado = true;
        transform.position = posicionInicial;

        float fuerza = Mathf.Lerp(fuerzaMinima, fuerzaMaxima, pico);
        rb.AddForce(direccion.normalized * fuerza, ForceMode2D.Impulse);
    }
}