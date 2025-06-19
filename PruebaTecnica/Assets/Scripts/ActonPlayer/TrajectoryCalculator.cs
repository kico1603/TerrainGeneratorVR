using UnityEngine;
using System.Collections.Generic;

public class TrajectoryCalculator : MonoBehaviour, IArcCalculator
{
    [Header("Par�metros de la trayectoria")]
    [Tooltip("Velocidad inicial de lanzamiento del proyectil (m/s).")]
    [SerializeField] private float initialSpeed = 10f;
    [Tooltip("Valor de la gravedad (positivo). Ej: 9.81 m/s�")]
    [SerializeField] private float gravity = 9.81f;
    [Tooltip("Intervalo de tiempo entre puntos de la trayectoria (resoluci�n del arco).")]
    [SerializeField] private float timeStep = 0.1f;

    /// <summary>
    /// Calcula la trayectoria del proyectil siguiendo las ecuaciones de movimiento parab�lico, 
    /// replicando la l�gica del ActionPlayerOLD. Incluye c�lculo del tiempo de vuelo y factor gravedad.
    /// Se detiene cuando el proyectil toca el suelo (y <= 0.01f).
    /// </summary>
    public List<Vector3> CalculateArcPoints(Vector3 origin, Vector3 direction)
    {
        var points = new System.Collections.Generic.List<Vector3>();
        if (direction.sqrMagnitude < 0.0001f)
        {
            return points; // direcci�n inv�lida, retornar lista vac�a
        }

        // Asegurarse de que la direcci�n est� normalizada
        Vector3 dir = direction.normalized;
        // Calcular la velocidad inicial en vector
        Vector3 initialVelocity = dir * initialSpeed;

        // Calcular el tiempo de vuelo total te�rico hasta y=0 (suelo) usando f�rmula f�sica:
        float vy = initialVelocity.y;
        float initialHeight = origin.y;
        float g = gravity;
        // F�rmula cuadr�tica: y(t) = y0 + vy*t - 0.5*g*t^2 = 0  =>  t = (vy + sqrt(vy^2 + 2*g*y0)) / g 
        float timeOfFlight = 0f;
        if (g > 0.0f)
        {
            // Prevenir ra�z de n�mero negativo
            float underRoot = vy * vy + 2 * g * initialHeight;
            timeOfFlight = (vy + Mathf.Sqrt(Mathf.Max(underRoot, 0f))) / g;
        }
        else
        {
            timeOfFlight = 0f;
        }

        // Iterar en pasos de tiempo hasta alcanzar el tiempo de vuelo o hasta tocar el suelo antes.
        float t = 0f;
        while (t <= timeOfFlight)
        {
            // Calcular posici�n para el tiempo t
            float x = origin.x + initialVelocity.x * t;
            float y = origin.y + initialVelocity.y * t - 0.5f * g * t * t;
            float z = origin.z + initialVelocity.z * t;
            Vector3 point = new Vector3(x, y, z);

            // Agregar el punto a la trayectoria
            points.Add(point);

            // Verificar si toc� el suelo (y muy cerca de 0)
            if (point.y <= 0.01f)
            {
                // Detener la generaci�n de puntos si el proyectil alcanza el suelo
                break;
            }

            // Incrementar el tiempo y continuar loop
            t += timeStep;
        }

        return points;
    }
}