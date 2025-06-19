using UnityEngine;

public interface IArcRenderer
{
    /// <summary>Renderiza (dibuja) la trayectoria dada una lista de puntos.</summary>
    /// <param name="points">Puntos que definen la trayectoria a dibujar.</param>
    void RenderArc(System.Collections.Generic.List<Vector3> points);

    /// <summary>Limpia el renderizado del arco (por ejemplo, para ocultar la línea cuando no haya trayectoria válida).</summary>
    void ClearArc();
}