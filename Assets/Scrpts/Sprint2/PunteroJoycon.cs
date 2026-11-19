using UnityEngine;

public class PunteroJoycon : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform puntero;
    public JuegoManager juego;

    [Header("Ajustes")]
    public float gradosParaCruzarPantalla = 50f;
    public float zonaMuerta = 0.5f;
    public float tiempoCalibracion = 3f;
    public int botonRecentrar = JSL.ButtonMaskS;
    public int botonDisparar = JSL.ButtonMaskZR;

    [Header("Ayuda")]
    public bool mostrarBotones = false;

    public bool Calibrando { get { return calibrando; } }
    public bool Conectado { get { return handle >= 0; } }

    int handle = -1;
    Vector2 posicion;
    float inicio;
    bool calibrando = false;
    int botonesAntes = 0;

    void Start()
    {
        int count = JSL.JslConnectDevices();
        Debug.Log("Controles encontrados: " + count);

        if (count > 0)
        {
            int[] handles = new int[count];
            JSL.JslGetConnectedDeviceHandles(handles, count);
            handle = handles[0];

            JSL.JslResetContinuousCalibration(handle);
            JSL.JslStartContinuousCalibration(handle);
            calibrando = true;
            inicio = Time.time;
        }

        Centrar();
    }

    void Update()
    {
        if (handle < 0 || puntero == null) return;

        if (calibrando)
        {
            if (Time.time - inicio >= tiempoCalibracion)
            {
                JSL.JslPauseContinuousCalibration(handle);
                calibrando = false;
                Centrar();
            }
            return;
        }

        JSL.IMU_STATE imu = JSL.JslGetIMUState(handle);
        float giroVertical = Mathf.Abs(imu.gyroX) < zonaMuerta ? 0f : imu.gyroX;
        float giroHorizontal = Mathf.Abs(imu.gyroY) < zonaMuerta ? 0f : imu.gyroY;

        float pixelesPorGrado = Screen.width / gradosParaCruzarPantalla;
        posicion.x -= giroHorizontal * Time.deltaTime * pixelesPorGrado;
        posicion.y += giroVertical * Time.deltaTime * pixelesPorGrado;

        posicion.x = Mathf.Clamp(posicion.x, 0f, Screen.width);
        posicion.y = Mathf.Clamp(posicion.y, 0f, Screen.height);
        puntero.position = posicion;

        int botones = JSL.JslGetSimpleState(handle).buttons;

        if (BotonApretado(botones, botonRecentrar)) Centrar();

        if (BotonApretado(botones, botonDisparar) && juego != null)
            juego.AlDisparar(posicion);

        if (mostrarBotones)
        {
            for (int i = 0; i < 20; i++)
                if (BotonApretado(botones, i))
                    Debug.Log("Boton presionado: indice " + i);
        }

        botonesAntes = botones;
    }

    bool BotonApretado(int botones, int indice)
    {
        bool ahora = (botones & (1 << indice)) != 0;
        bool antes = (botonesAntes & (1 << indice)) != 0;
        return ahora && !antes;
    }

    void Centrar()
    {
        posicion = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (puntero != null) puntero.position = posicion;
    }

    void OnApplicationQuit()
    {
        JSL.JslDisconnectAndDisposeAll();
    }
}