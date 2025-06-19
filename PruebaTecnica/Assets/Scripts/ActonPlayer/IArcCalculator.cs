using UnityEngine;

public interface IArcCalculator
{
    /// <summary>Calcula los puntos de la trayectoria parabólica del proyectil dado un origen y una dirección inicial.</summary>
    /// <param name="origin">Punto de origen del lanzamiento.</param>
    /// <param name="direction">Dirección de lanzamiento (vector unitario).</param>
    /// <returns>Lista de puntos Vector3 que describen la trayectoria del proyectil.</returns>
    System.Collections.Generic.List<Vector3> CalculateArcPoints(Vector3 origin, Vector3 direction);
}