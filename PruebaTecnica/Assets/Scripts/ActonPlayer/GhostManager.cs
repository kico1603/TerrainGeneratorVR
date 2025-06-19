using UnityEngine;

public class GhostManager : MonoBehaviour, IGhostManager
{
    [Header("Referencia del objeto Ghost (marcador)")]
    [SerializeField] private GameObject ghostObject;
    [Tooltip("Máscara de capas para el raycast (ej: suelo).")]
    [SerializeField] private LayerMask raycastLayers = ~0;  // por defecto, todas las capas

    private bool ghostActive = false;

    /// <summary>Raycast vertical hacia abajo desde el punto final de la trayectoria 
    /// para colocar el ghost en la superficie impactada.</summary>
    public bool PlaceGhost(Vector3 trajectoryEndPoint)
    {
        if (ghostObject == null)
        {
            Debug.LogWarning("GhostManager: ghostObject no asignado.");
            ghostActive = false;
            return false;
        }

        RaycastHit hit;
        // Lanzar un rayo desde un poco arriba del punto final hacia abajo
        Vector3 rayStart = trajectoryEndPoint + Vector3.up * 3f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 5f, raycastLayers))
        {
            // Colocar el objeto ghost en la posición del impacto dentro de la cuadricula
            ghostObject.transform.position = hit.collider.transform.position + Vector3.up * hit.collider.bounds.size.y;

            ghostObject.transform.rotation = hit.collider.transform.rotation;

            // Activar/mostrar el ghost
            if (!ghostObject.activeSelf) 
                ghostObject.SetActive(true);

            ghostActive = true;
        }
        else
        {
            // Si no se impactó nada (por ejemplo, fuera de rango o sin suelo), ocultar el ghost
            if (ghostObject.activeSelf) 
                ghostObject.SetActive(false);

            ghostActive = false;
        }

        return ghostActive;
    }

    public bool IsGhostActive()
    {
        return ghostActive;
    }

    public Vector3 GetGhostPosition()
    {
        if (ghostObject != null)
            return ghostObject.transform.position;
        return Vector3.zero;
    }

    public Quaternion GetGhostRotation()
    {
        if (ghostObject != null)
            return ghostObject.transform.rotation;
        return Quaternion.identity;
    }

    public void HideGhost()
    {
        if (ghostObject != null)
        {
            ghostObject.SetActive(false);
        }
        ghostActive = false;
    }
}