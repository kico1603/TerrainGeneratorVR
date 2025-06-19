using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour, IObjectSpawner
{
    [Header("Prefab del objeto a instanciar")]
    [SerializeField] private GameObject objectPrefab;
    private ObjectsPoolTypes objectsPool;

    public ObjectsPoolTypes.TypeGameObjectIDKey objectToInstance = ObjectsPoolTypes.TypeGameObjectIDKey.CubeMetal;

    private List<GameObject> gameObjectsInstanced;


    private void Start()
    {
        gameObjectsInstanced = new List<GameObject>();
        objectsPool = ObjectsPoolTypes.Instance;
    }

    public void SpawnObject(Vector3 position, Quaternion rotation)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("ObjectSpawner: Prefab no asignado para instanciar.");
            return;
        }
        // Instanciar el prefab en la posición y rotación dadas
        gameObjectsInstanced.Add(objectsPool.InstanciteObjectPool(objectToInstance, position, rotation));
    }

    public void ClearObjects()
    {
        foreach (GameObject item in gameObjectsInstanced)
        {
            objectsPool.ReturnObjectToObjectPool(objectToInstance, item);
        }
    }
}
