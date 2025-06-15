using UnityEngine;
using System.Collections.Generic;

public class ChunkPathGenerator : MonoBehaviour
{
    public int chunkSize = 10;  // Tamaño del chunk (puede ser ajustado entre 5 y 50)

    /// <summary>Genera el camino a través de este chunk, cumpliendo las reglas dadas.</summary>
    /// <param name="entryPoint">
    /// Punto de entrada desde el chunk anterior. Si es null, se asume que este es el primer chunk 
    /// y el camino comenzará en el centro del chunk.
    /// </param>
    /// <returns>Lista de coordenadas (Vector2Int) que conforman el camino dentro del chunk.</returns>
    public List<Vector2Int> GeneratePath(Vector2Int? entryPoint = null)
    {
        // Determinar punto de inicio y borde de entrada
        Vector2Int start;
        Edge entryEdge;
        if (entryPoint.HasValue)
        {
            // Chunk subsiguiente: usar la entrada proporcionada (debe ser una celda de borde)
            start = entryPoint.Value;
            entryEdge = GetEdgeFromPosition(start);
        }
        else
        {
            // Primer chunk: iniciar en el centro del terreno
            int mid = chunkSize / 2;  // ⌊chunkSize/2⌋
            start = new Vector2Int(mid, mid);
            entryEdge = Edge.None;
        }

        // Elegir aleatoriamente un borde de salida distinto al de entrada
        List<Edge> possibleExits = new List<Edge> { Edge.Top, Edge.Bottom, Edge.Left, Edge.Right };
        if (entryEdge != Edge.None)
        {
            possibleExits.Remove(entryEdge);  // quitar el borde de donde vino para no retroceder por el mismo lado
        }
        Edge exitEdge = possibleExits[Random.Range(0, possibleExits.Count)];

        // Elegir una posición de salida sobre el borde seleccionado, evitando esquinas 
        // (mantener al menos 1 unidad de distancia de las esquinas)
        Vector2Int exit;
        switch (exitEdge)
        {
            case Edge.Top:
                exit = new Vector2Int(Random.Range(1, chunkSize - 1), chunkSize - 1);
                break;
            case Edge.Bottom:
                exit = new Vector2Int(Random.Range(1, chunkSize - 1), 0);
                break;
            case Edge.Left:
                exit = new Vector2Int(0, Random.Range(1, chunkSize - 1));
                break;
            case Edge.Right:
                exit = new Vector2Int(chunkSize - 1, Random.Range(1, chunkSize - 1));
                break;
            default:
                exit = new Vector2Int(0, 0);  // caso no debería ocurrir
                break;
        }

        // Calcular longitud minima requerida (50% del tamano del chunk, usando distancia Manhattan)
        int minSteps = Mathf.CeilToInt(chunkSize * 0.5f);  // redondear hacia arriba

        // Estructuras para la generacion del camino
        List<Vector2Int> path = new List<Vector2Int>();
        bool[,] visited = new bool[chunkSize, chunkSize];  // grid de celdas visitadas

        // Marcar el inicio
        path.Add(start);
        visited[start.x, start.y] = true;

        // Definir las cuatro direcciones ortogonales posibles
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,    // (0, 1)
            Vector2Int.down,  // (0, -1)
            Vector2Int.left,  // (-1, 0)
            Vector2Int.right  // (1, 0)
        };

        // Funcion recursiva interna para buscar un camino valido mediante backtracking
        bool Backtrack(Vector2Int current)
        {
            // Si hemos llegado a la salida...
            if (current == exit)
            {
                // Comprobar que se cumplio la longitud minima (pasos Manhattan >= 50% chunkSize)
                int stepsCount = path.Count - 1;  // número de movimientos realizados
                return (stepsCount >= minSteps);
            }

            // Construir lista aleatoria de movimientos posibles desde la celda actual
            // (se mezclan las direcciones en orden aleatorio para diversificar los caminos probados)
            System.Random rng = new System.Random();
            Vector2Int[] dirsShuffled = (Vector2Int[])directions.Clone();
            // Mezclar el arreglo de direcciones (Fisher-Yates shuffle)
            for (int i = dirsShuffled.Length - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                Vector2Int temp = dirsShuffled[i];
                dirsShuffled[i] = dirsShuffled[j];
                dirsShuffled[j] = temp;
            }

            foreach (Vector2Int dir in dirsShuffled)
            {
                Vector2Int next = current + dir;

                // 1. Verificar que next esté dentro de los límites del chunk
                if (next.x < 0 || next.x >= chunkSize || next.y < 0 || next.y >= chunkSize)
                    continue;
                // 2. Evitar visitar una celda ya recorrida (sin loops)
                if (visited[next.x, next.y])
                    continue;
                // 3. Respetar la restricción de no tocar bordes, a menos que sea la salida
                bool isExitCell = (next == exit);
                if (!isExitCell)
                {
                    // Si la siguiente celda está en el borde y no es la meta, se omite
                    if (next.x == 0 || next.x == chunkSize - 1 || next.y == 0 || next.y == chunkSize - 1)
                        continue;
                }
                // 4. No retroceder inmediatamente al punto de entrada del chunk:
                //    Si estamos en la celda de inicio (entrada) y next apunta otra vez fuera del chunk 
                //    por el mismo borde de entrada, lo evitamos. (Esto implícitamente ya está manejado 
                //    por las condiciones anteriores, pues esa celda de salida estaría marcada o fuera de rango.)

                // Tomar el paso a la siguiente celda valida
                path.Add(next);
                visited[next.x, next.y] = true;

                // Recursión: intentar continuar desde la nueva posición
                if (Backtrack(next))
                {
                    return true;  // si la recursión encuentra un camino válido hasta la salida, propagar éxito
                }

                // Si no se encontró camino desde 'next', hacer *backtracking*: deshacer el paso
                path.RemoveAt(path.Count - 1);
                visited[next.x, next.y] = false;
            }

            // No se encontró camino válido desde esta celda
            return false;
        }

        // Ejecutar la búsqueda backtracking desde el punto inicial
        bool foundPath = Backtrack(start);
        if (!foundPath)
        {
            Debug.LogWarning("No se encontró un camino que cumpla las restricciones en este chunk. " +
                             "Considera reintentarlo con otra salida.");
        }

        // (Opcional) Guardar la salida calculada para uso externo, por ejemplo:
        //this.exitPoint = exit;
        //this.exitEdge = exitEdge;
        // Devolver la lista de puntos del camino encontrado
        return path;
    }

    /// <summary>Determina qué borde representa una posición dada (se asume que está en el borde).</summary>
    private Edge GetEdgeFromPosition(Vector2Int pos)
    {
        if (pos.y == chunkSize - 1) return Edge.Top;
        if (pos.y == 0) return Edge.Bottom;
        if (pos.x == 0) return Edge.Left;
        if (pos.x == chunkSize - 1) return Edge.Right;
        return Edge.None;
    }

    // Definición de bordes para claridad
    private enum Edge { None, Top, Bottom, Left, Right }
}
