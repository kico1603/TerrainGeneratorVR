using UnityEngine;

public interface IGhostManager
{
    /// <summary>Actualiza la posici�n del objeto ghost realizando un raycast vertical hacia abajo 
    /// desde el punto final de la trayectoria.</summary>
    /// <param name="trajectoryEndPoint">Punto final de la trayectoria calculada.</param>
    /// <returns>true si encontr� una superficie v�lida debajo y coloc� el ghost, false en caso contrario.</returns>
    bool PlaceGhost(Vector3 trajectoryEndPoint);

    /// <summary>Devuelve true si el ghost est� actualmente colocado en una posici�n v�lida (visible).</summary>
    bool IsGhostActive();

    /// <summary>Obtiene la posici�n actual del objeto ghost.</summary>
    Vector3 GetGhostPosition();

    /// <summary>Obtiene la rotaci�n actual del objeto ghost.</summary>
    Quaternion GetGhostRotation();

    /// <summary>Oculta o desactiva el ghost (por ejemplo, cuando no hay impacto v�lido).</summary>
    void HideGhost();
}
