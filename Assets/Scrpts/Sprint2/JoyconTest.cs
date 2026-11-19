using UnityEngine;

public class JoyconTest : MonoBehaviour
{
    int handle = -1;
    float tiempoCalibracion = 3f;
    float inicio;
    bool calibrando = false;
    float umbral = 40f;       // ignora movimientos suaves (grados/seg)
    float proximoLog = 0f;

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
            Debug.Log("CALIBRANDO: no muevas el Joy-Con durante " + tiempoCalibracion + " segundos...");
        }
    }

    void Update()
    {
        if (handle < 0) return;

        if (calibrando && Time.time - inicio >= tiempoCalibracion)
        {
            JSL.JslPauseContinuousCalibration(handle);
            calibrando = false;
            Debug.Log("Calibracion terminada. Sostenlo como sostendrias el arma y haz los 4 movimientos.");
        }

        if (calibrando || Time.time < proximoLog) return;

        JSL.IMU_STATE imu = JSL.JslGetIMUState(handle);
        float ax = Mathf.Abs(imu.gyroX);
        float ay = Mathf.Abs(imu.gyroY);
        float az = Mathf.Abs(imu.gyroZ);
        float mayor = Mathf.Max(ax, Mathf.Max(ay, az));

        if (mayor < umbral) return;   // casi quieto, no escribimos nada

        proximoLog = Time.time + 0.3f;

        string eje; float valor;
        if (mayor == ax) { eje = "X"; valor = imu.gyroX; }
        else if (mayor == ay) { eje = "Y"; valor = imu.gyroY; }
        else { eje = "Z"; valor = imu.gyroZ; }

        string signo = valor > 0 ? "POSITIVO (+)" : "NEGATIVO (-)";
        Debug.Log("Eje dominante: " + eje + "  " + signo + "  (" + valor.ToString("F0") + ")");
    }

    void OnApplicationQuit()
    {
        JSL.JslDisconnectAndDisposeAll();
    }
}