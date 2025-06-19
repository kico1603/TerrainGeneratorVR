using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour, IObjectSpawner
{
    [Header("Prefab del objeto a instanciar")]
    [SerializeField] private GameObject objectPrefab;

    public void SpawnObject(Vector3 position, Quaternion rotation)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("ObjectSpawner: Prefab no asignado para instanciar.");
            return;
        }
        // Instanciar el prefab en la posición y rotación dadas
        Instantiate(objectPrefab, position, rotation);
    }
}
