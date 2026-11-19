using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class FinDeCinematica : MonoBehaviour
{
    public string siguienteEscena = "Minijuego2";
    public VideoPlayer video;

    bool cargando = false;

    void Start()
    {
        if (video != null) video.loopPointReached += (v) => Cargar();
    }

    void Update()
    {
        // Atajo para probar: N salta a la siguiente escena
        if (Input.GetKeyDown(KeyCode.N)) Cargar();
    }

    void Cargar()
    {
        if (cargando) return;
        cargando = true;
        SceneManager.LoadScene(siguienteEscena);
    }
}