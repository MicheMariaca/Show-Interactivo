using UnityEngine;
using UnityEngine.Events;

public class JuegoManager : MonoBehaviour
{
    public enum Fase { Calibrando, Jugando, Capturado }

    [Header("Referencias")]
    public PunteroJoycon puntero;
    public ObjetivoMovil objetivo;

    [Header("Eventos (para conectar cosas despues)")]
    public UnityEvent alEmpezar;
    public UnityEvent alCapturar;

    [Header("Al capturar")]
    public bool ocultarAlCapturar = true;

    [Header("Estado (solo lectura)")]
    public Fase fase = Fase.Calibrando;

    void Start()
    {
        objetivo.gameObject.SetActive(true);
        objetivo.Observar();
    }

    void Update()
    {
        // Espera a que el control este conectado y calibrado
        if (fase == Fase.Calibrando && puntero.Conectado && !puntero.Calibrando)
        {
            fase = Fase.Jugando;
            objetivo.Soltar();
            alEmpezar.Invoke();
            Debug.Log("EMPIEZA.");
        }
    }

    public void AlDisparar(Vector2 posicion)
    {
        if (fase != Fase.Jugando) return;

        if (objetivo.Disparar(posicion))
        {
            fase = Fase.Capturado;
            if (ocultarAlCapturar) objetivo.gameObject.SetActive(false);
            alCapturar.Invoke();
            Debug.Log("CAPTURADO.");
        }
    }
}