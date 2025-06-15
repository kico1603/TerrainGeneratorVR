using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Prefabs cube")]
    public GameObject terrainCubePrefab;
    public GameObject pathCubePrefab;
    public GameObject groundCubePrefab;

    [Header("Terrain Settings")]
    [Range(5, 50)] public int chunkSize = 13;
    [Range(1, 10)] public int numChunksX = 2; // Ahora cuadrícula X x Z
    [Range(1, 10)] public int numChunksZ = 2;
    [Range(1, 5)] public int heightBase = 2;
    [Range(0, 10)] public int heightMax = 5;
    [Range(0.01f, 0.5f)] public float noiseScale = 0.1f;
    public int seed = 42;
    public float noiseOffset = 0f;

    [Header("Path Settings")]
    [Range(0, 1)] public float pathExpansion = 0.5f;
    [Range(0, 1)] public float pathIrregularity = 0.3f;

    [ContextMenu(nameof(GenerateTerrainGenerator))]
    void GenerateTerrainGenerator()
    {
        ClearTerrain();
        Random.InitState(seed);
        float offsetX = seed * 37.123f + noiseOffset;
        float offsetZ = seed * 19.456f + noiseOffset;

        // Para mantener el estado de la conexión del camino entre chunks
        // Usamos un array 2D para la entrada/salida
        Vector2Int?[,] entradas = new Vector2Int?[numChunksX, numChunksZ];
        Vector2Int?[,] salidas = new Vector2Int?[numChunksX, numChunksZ];

        // Recorremos los chunks en orden (por ejemplo de arriba-izquierda a abajo-derecha)
        for (int cx = 0; cx < numChunksX; cx++)
        {
            for (int cz = 0; cz < numChunksZ; cz++)
            {
                // Definir la posición de entrada para el camino de este chunk
                Vector2Int? entrada = null;
                Vector2Int? ladoEntrada = null; // Qué borde es (N/S/E/O)
                if (cx == 0 && cz == 0)
                {
                    // Primer chunk, el camino empieza en el centro
                    entrada = new Vector2Int(chunkSize / 2, chunkSize / 2);
                }
                else if (cx > 0)
                {
                    // Chunk a la derecha de otro, entra por el borde izquierdo
                    entrada = salidas[cx - 1, cz];
                    ladoEntrada = Vector2Int.left;
                }
                else if (cz > 0)
                {
                    // Chunk arriba/abajo de otro, entra por el borde inferior
                    entrada = salidas[cx, cz - 1];
                    ladoEntrada = Vector2Int.down;
                }

                // Generar el chunk con camino, devolviendo el punto de salida
                Vector2Int? salida;
                GenerateChunkWithPath(cx, cz, offsetX, offsetZ, entrada, ladoEntrada, out salida);

                // Guardar la salida para conectar el siguiente chunk
                salidas[cx, cz] = salida;
            }
        }
    }

    void GenerateChunkWithPath(int chunkX, int chunkZ, float offsetX, float offsetZ, Vector2Int? entrada, Vector2Int? ladoEntrada, out Vector2Int? salida)
    {
        Vector3 offset = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);

        // 1. Calcular alturas máximas del terreno
        int[,] topHeights = new int[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float xGlobal = chunkX * chunkSize + x;
                float zGlobal = chunkZ * chunkSize + z;
                float noise = Mathf.PerlinNoise(offsetX + xGlobal * noiseScale, offsetZ + zGlobal * noiseScale);
                int height = heightBase + Mathf.RoundToInt(noise * heightMax);
                topHeights[x, z] = height;
            }
        }

        // 2. Generar el camino: marcamos dónde va el camino y obtenemos el punto de salida
        bool[,] isPath = new bool[chunkSize, chunkSize];
        Vector2Int salidaLocal;
        GeneratePath(chunkSize, isPath, entrada, ladoEntrada, out salidaLocal);

        salida = new Vector2Int(
            salidaLocal.x + chunkX * chunkSize,
            salidaLocal.y + chunkZ * chunkSize
        );

        // 3. Instanciar cubos según si es camino o terreno
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int height = topHeights[x, z];
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = offset + new Vector3(x, y, z);
                    if (y == height - 1)
                    {
                        if (isPath[x, z])
                            Instantiate(pathCubePrefab, pos, Quaternion.identity, this.transform);
                        else
                            Instantiate(terrainCubePrefab, pos, Quaternion.identity, this.transform);
                    }
                    else
                    {
                        Instantiate(groundCubePrefab, pos, Quaternion.identity, this.transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Marca el camino en isPath y devuelve el punto de salida en el chunk (coordenadas locales)
    /// </summary>

    void GeneratePath(
       int chunkSize,
       bool[,] isPath,
       Vector2Int? entrada,
       Vector2Int? ladoEntrada,
       out Vector2Int salida)
    {
        int minMargin = 1;
        int minLength = Mathf.CeilToInt(chunkSize * 0.5f);

        // Validar entrada
        Vector2Int posActual = entrada ?? new Vector2Int(chunkSize / 2, chunkSize / 2);
        posActual.x = Mathf.Clamp(posActual.x, 0, chunkSize - 1);
        posActual.y = Mathf.Clamp(posActual.y, 0, chunkSize - 1);

        isPath[posActual.x, posActual.y] = true;

        // Definir bordes posibles para salida que NO sean igual a entrada
        List<Vector2Int> possibleBorders = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        if (ladoEntrada.HasValue)
            possibleBorders.Remove(-ladoEntrada.Value);

        // Elegir aleatoriamente el borde de salida
        Vector2Int bordeSalida = possibleBorders[Random.Range(0, possibleBorders.Count)];

        // Rango seguro para salida
        int min = minMargin;
        int max = chunkSize - minMargin;
        if (max <= min) max = min + 1;

        Vector2Int salidaLocal = bordeSalida switch
        {
            Vector2Int v when v == Vector2Int.up => new Vector2Int(Random.Range(min, max), chunkSize - 1),
            Vector2Int v when v == Vector2Int.down => new Vector2Int(Random.Range(min, max), 0),
            Vector2Int v when v == Vector2Int.left => new Vector2Int(0, Random.Range(min, max)),
            Vector2Int v when v == Vector2Int.right => new Vector2Int(chunkSize - 1, Random.Range(min, max)),
            _ => new Vector2Int(chunkSize / 2, chunkSize - 1)
        };
        // Clamp también aquí por seguridad
        salidaLocal.x = Mathf.Clamp(salidaLocal.x, 0, chunkSize - 1);
        salidaLocal.y = Mathf.Clamp(salidaLocal.y, 0, chunkSize - 1);

        // Ruta básica (Primero moverse hacia centro hasta cumplir minLength)
        List<Vector2Int> pathPoints = new List<Vector2Int> { posActual };
        int steps = 0;

        while (steps < minLength || posActual != salidaLocal)
        {
            Vector2Int dir = Vector2Int.zero;

            if (posActual != salidaLocal)
            {
                if (posActual.x != salidaLocal.x)
                    dir.x = (salidaLocal.x > posActual.x) ? 1 : -1;
                else if (posActual.y != salidaLocal.y)
                    dir.y = (salidaLocal.y > posActual.y) ? 1 : -1;
            }

            // Introducir irregularidad
            if (Random.value < pathIrregularity)
            {
                List<Vector2Int> dirsAlternativas = new List<Vector2Int>();
                if (posActual.x > minMargin) dirsAlternativas.Add(Vector2Int.left);
                if (posActual.x < chunkSize - minMargin - 1) dirsAlternativas.Add(Vector2Int.right);
                if (posActual.y > minMargin) dirsAlternativas.Add(Vector2Int.down);
                if (posActual.y < chunkSize - minMargin - 1) dirsAlternativas.Add(Vector2Int.up);

                if (dirsAlternativas.Count > 0)
                    dir = dirsAlternativas[Random.Range(0, dirsAlternativas.Count)];
            }

            // Siguiente paso seguro
            Vector2Int nextPos = posActual + dir;

            // Clamp para seguridad absoluta
            nextPos.x = Mathf.Clamp(nextPos.x, 0, chunkSize - 1);
            nextPos.y = Mathf.Clamp(nextPos.y, 0, chunkSize - 1);

            // Evitar loops infinitos
            if (nextPos == posActual)
                break;

            posActual = nextPos;
            if (posActual.x >= 0 && posActual.x < chunkSize && posActual.y >= 0 && posActual.y < chunkSize && !pathPoints.Contains(posActual))
            {
                pathPoints.Add(posActual);
                isPath[posActual.x, posActual.y] = true;
                steps++;
            }

            if (posActual == salidaLocal && steps >= minLength)
                break;
        }

        salida = salidaLocal;
    }






    [ContextMenu(nameof(ClearTerrain))]
    void ClearTerrain()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
