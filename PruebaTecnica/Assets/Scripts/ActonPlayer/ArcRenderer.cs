
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class ArcRenderer : MonoBehaviour, IArcRenderer
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // Obtener el LineRenderer adjunto en el mismo GameObject
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("ArcRenderer: No se encontró LineRenderer.");
        }
    }

    /// <summary>Dibuja la línea del arco de trayectoria utilizando los puntos proporcionados.</summary>
    public void RenderArc(System.Collections.Generic.List<Vector3> points)
    {
        if (lineRenderer == null) return;

        if (points != null && points.Count > 0)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = points.Count;
            // Asignar todos los puntos al LineRenderer
            lineRenderer.SetPositions(points.ToArray());
        }
        else
        {
            // Si no hay puntos, ocultar la línea
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }

    /// <summary>Oculta/limpia la representación del arco (sin puntos).</summary>
    public void ClearArc()
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }
}