using UnityEngine;

public interface IArcRenderer
{
    /// <summary>Renderiza (dibuja) la trayectoria dada una lista de puntos.</summary>
    /// <param name="points">Puntos que definen la trayectoria a dibujar.</param>
    void RenderArc(System.Collections.Generic.List<Vector3> points);

    /// <summary>Limpia el renderizado del arco (por ejemplo, para ocultar la l�nea cuando no haya trayectoria v�lida).</summary>
    void ClearArc();
}