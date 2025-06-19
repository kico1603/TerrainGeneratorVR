using UnityEngine;

public interface IGhostManager
{
    /// <summary>Actualiza la posición del objeto ghost realizando un raycast vertical hacia abajo 
    /// desde el punto final de la trayectoria.</summary>
    /// <param name="trajectoryEndPoint">Punto final de la trayectoria calculada.</param>
    /// <returns>true si encontró una superficie válida debajo y colocó el ghost, false en caso contrario.</returns>
    bool PlaceGhost(Vector3 trajectoryEndPoint);

    /// <summary>Devuelve true si el ghost está actualmente colocado en una posición válida (visible).</summary>
    bool IsGhostActive();

    /// <summary>Obtiene la posición actual del objeto ghost.</summary>
    Vector3 GetGhostPosition();

    /// <summary>Obtiene la rotación actual del objeto ghost.</summary>
    Quaternion GetGhostRotation();

    /// <summary>Oculta o desactiva el ghost (por ejemplo, cuando no hay impacto válido).</summary>
    void HideGhost();
}
