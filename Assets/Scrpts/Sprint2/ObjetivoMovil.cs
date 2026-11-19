using UnityEngine;

public class ObjetivoMovil : MonoBehaviour
{
    public enum Estado { Observando, Huyendo, Reposo }

    [Header("Referencias")]
    public RectTransform puntero;

    [Header("Huida")]
    public float velocidad = 700f;
    public float duracionHuidaMin = 1.2f;
    public float duracionHuidaMax = 2.5f;
    public float suavidadGiro = 6f;

    [Header("Reposo")]
    public float duracionReposo = 1f;

    [Header("Bordes")]
    public float margenBorde = 200f;
    public float fuerzaBorde = 1.5f;

    [Header("Estado (solo lectura)")]
    public Estado estado = Estado.Observando;
    public bool mirandoDerecha = true;

    RectTransform rt;
    Vector2 direccion;
    float finDeEstado;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (puntero == null) return;

        // Observando: se queda quieto hasta que el juego lo suelte
        if (estado == Estado.Observando) return;

        if (Time.time >= finDeEstado)
        {
            if (estado == Estado.Huyendo) EntrarEnReposo();
            else EntrarEnHuida();
        }

        if (estado == Estado.Huyendo) Huir();
    }

    void Huir()
    {
        Vector2 pos = rt.position;
        Vector2 posPuntero = puntero.position;

        Vector2 alejarse = pos - posPuntero;
        if (alejarse.sqrMagnitude < 1f) alejarse = Random.insideUnitCircle;
        alejarse.Normalize();

        Vector2 empuje = Vector2.zero;
        if (pos.x < margenBorde) empuje.x += 1f - pos.x / margenBorde;
        if (pos.x > Screen.width - margenBorde) empuje.x -= 1f - (Screen.width - pos.x) / margenBorde;
        if (pos.y < margenBorde) empuje.y += 1f - pos.y / margenBorde;
        if (pos.y > Screen.height - margenBorde) empuje.y -= 1f - (Screen.height - pos.y) / margenBorde;

        Vector2 deseada = (alejarse + empuje * fuerzaBorde).normalized;
        direccion = Vector2.Lerp(direccion, deseada, suavidadGiro * Time.deltaTime).normalized;

        pos += direccion * velocidad * Time.deltaTime;

        float mitadX = rt.sizeDelta.x / 2f;
        float mitadY = rt.sizeDelta.y / 2f;
        pos.x = Mathf.Clamp(pos.x, mitadX, Screen.width - mitadX);
        pos.y = Mathf.Clamp(pos.y, mitadY, Screen.height - mitadY);
        rt.position = pos;

        if (Mathf.Abs(direccion.x) > 0.1f) mirandoDerecha = direccion.x > 0f;
    }

    void EntrarEnHuida()
    {
        estado = Estado.Huyendo;
        finDeEstado = Time.time + Random.Range(duracionHuidaMin, duracionHuidaMax);
    }

    void EntrarEnReposo()
    {
        estado = Estado.Reposo;
        finDeEstado = Time.time + duracionReposo;
    }

    // El juego llama a esto mientras se calibra
    public void Observar()
    {
        estado = Estado.Observando;
    }

    // El juego llama a esto cuando termina la calibracion
    public void Soltar()
    {
        Vector2 alejarse = (Vector2)rt.position - (Vector2)puntero.position;
        direccion = alejarse.sqrMagnitude < 1f ? Random.insideUnitCircle.normalized : alejarse.normalized;
        EntrarEnHuida();
    }

    // Devuelve true si el disparo acerto
    public bool Disparar(Vector2 puntoEnPantalla)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, puntoEnPantalla, null);
    }
}