using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


/**
 * 
 * Ejemplo de uso:
 * 
 * Para invocar un GameObject de la ObjectsPool:
 * 
 * GameObject go = ObjectsPoolTypes.Instance.InstanciteObjectPool(ObjectsPoolTypes.TypeGameObjectIDKey.objeto, positionRespawn, Quaternion.identity);
 * 
 * Para retornar un GameObject en la ObjectsPool:
 * 
 * ObjectsPoolTypes.Instance.ReturnObjectToObjectPool(ObjectsPoolTypes.TypeGameObjectIDKey.objeto, gameObject);
 * 
 * Para retornar el Gameobject una vez que se ha terminado o Muerto se debe borrar las variables de sus respectivos componentes que deban limpiarse.
 * Por ejemplo: Rigibody.Velocity = Vector3.zero;
 * 
 * Saber existencias en activo en la escena
 * ObjectsPoolTypes.Instance.GetNumberInventoryAccountOfExistingObjects(ObjectsPoolTypes.TypeGameObjectIDKey.objeto);
 * 
 */


public class ObjectsPoolTypes : MonoBehaviour
{

    [Header("Lista de objetos")]
    [Tooltip("Esta lista debe contener todos los tipos de TypeGameObjectIDKey siendo unicos")]

    public TypeGameObjectPool[] pooledObjects;

    public event Action OnObjectPoolGenerationCompleted;

    //public int amountToPool;

    [System.Serializable]
    public enum TypeGameObjectIDKey
    {
      Test

    }



    [System.Serializable]
    public struct TypeGameObjectPool
    {
        [Header("Name")]
        public string text;

        [Header("Configurable")]
        public TypeGameObjectIDKey type;
        public int numToGenerate;
        public List<GameObject> prefabsGo;

        [Header("OnlyRead")]
        public GameObject containerGos;
        public int numActiveExistingObject;
    }

    private static ObjectsPoolTypes _instance;
    public static ObjectsPoolTypes Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"La variable _instance en el metodo {System.Reflection.MethodBase.GetCurrentMethod().Name}() de la clase {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name} no ha sido inicializado. Aseg�rate de que el GameObject con el script {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name} esta activo y se inicialice antes de intentar acceder a la instancia.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            DestroyImmediate(this.gameObject);
        }

#if UNITY_EDITOR
        pooledObjects = ComprobateAndRepairObjectsPoolTypes();
#endif

    }

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        StartCoroutine(GenerateAllObjectPool());
    }


    [ContextMenu(nameof(SetNames))]
    public void SetNames()
    {
        for (int i = 0; i < pooledObjects.Length; i++)
        {
            pooledObjects[i].text = $"{pooledObjects[i].type.ToString()}_ObjPool";
        }
    }



    IEnumerator GenerateAllObjectPool()
    {

        yield return null;

        for (int i = 0; i < pooledObjects.Length; i++)
        {
            yield return null;

            if (pooledObjects[i].numToGenerate > 0)
            {
                pooledObjects[i].numActiveExistingObject = 0;
                pooledObjects[i].text = "GameObjectPool_" + pooledObjects[i].type.ToString();
                pooledObjects[i].containerGos = new GameObject("Container_" + pooledObjects[i].text);

                //MOVER A ESCENA
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pooledObjects[i].containerGos, this.gameObject.scene);

                pooledObjects[i].containerGos.transform.SetParent(this.transform);

                //Debug.Log($"Se crea {pooledObjects[i].text}");

                GameObject tmp;

                for (int j = 0; j < pooledObjects[i].numToGenerate; j++)
                {

                    tmp = Instantiate(pooledObjects[i].prefabsGo[UnityEngine.Random.Range(0, pooledObjects[i].prefabsGo.Count)], Vector3.zero, Quaternion.identity);
                    tmp.SetActive(false);
                    tmp.transform.SetParent(pooledObjects[i].containerGos.transform);
                    tmp.name = tmp.name + "_" + j;
                    yield return null;

                    //pooledObjects.Add(tmp);
                }
            }
        }

        OnObjectPoolGenerationCompleted?.Invoke();
    }

   

    [ContextMenu("Comprobate_And_Repair_ObjectsPoolTypes")]
    [ExecuteInEditMode]
    public TypeGameObjectPool[] ComprobateAndRepairObjectsPoolTypes()
    {
        var referenceDict = new Dictionary<TypeGameObjectIDKey, TypeGameObjectPool>();
        foreach (var pool in pooledObjects)
        {
            if (!referenceDict.ContainsKey(pool.type))
            {
                referenceDict[pool.type] = pool;
            }
            else
            {
                Debug.LogError("Error => Tipo duplicado en la ObjectPool: " + pool.type);
            }
        }

        var allTypes = new HashSet<TypeGameObjectIDKey>(Enum.GetValues(typeof(TypeGameObjectIDKey)).Cast<TypeGameObjectIDKey>());
        foreach (var type in allTypes)
        {
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict[type] = new TypeGameObjectPool { type = type, numToGenerate = 0, text = "AUTO_GENERATED_" + type };
                Debug.LogWarning("Error => Mal uso de la ObjectPool: Se debe tener en el array pooledObjects[] todos los tipos de objetos ordenados segun orden en el enum, aunque numToGenerate esten a 0. Agregando elemento faltante al ObjectPool: " + type);
            }
        }

        /**
         * Ordena el array por el mismo orden que el enumerador TypeGameObjectIDKey Para que? Para que cuando se tenga que retornar en GetNumberInventoryAccountOfExistingObjects(),
         * solo dando el tipo coincida con su int en el index del array de los objetos, asi no tiene que listar y recorrer una lista que puede ser inmensa. Si no se entiende preguntar a Francisco J. Ponce Martinez.
         * por ejemplo, en => " return pooledObjects[(int) _type].numToGenerate; " esto con un array no ordenado igual que el enumerador, daria objetos equivocados
         * como buscar en el array el objeto por el tipo cada vez que se llama al metodo seria poco optimo, se ordena antes de iniciar.asi cada llamada al metodo solo devuelve el valor y no hay logica ni busqueda en su interior
        **/

        var newPooledObjects = referenceDict.Values.OrderBy(p => (int)p.type).ToList();
        pooledObjects = newPooledObjects.ToArray();

        return pooledObjects;
    }


    public GameObject GetPooledObject(TypeGameObjectIDKey _type)
    {
        int index = (int)_type;

        if (pooledObjects[index].numToGenerate == 0 || pooledObjects[index].prefabsGo == null)
        {
            Debug.LogError("ERROR en la ObjectPool se ha pedido un objeto del tipo " + _type.ToString() + " que no se ha generado. Poner el valor numToGenerate a mas de 0 y/o rellenar el prefab");

            GameObject errorGameObject = new GameObject("ERROR_OBJECPOOL_GameObject");

            Destroy(errorGameObject, 5f);

            return errorGameObject;
        }

        List<GameObject> goCollection = GetChildren(pooledObjects[index].containerGos);

        for (int i = 0; i < goCollection.Count; i++)
        {
            if (!goCollection[i].gameObject.activeSelf)
            {
                pooledObjects[index].numActiveExistingObject++;
                goCollection[i].transform.SetParent(null);
                return goCollection[i].gameObject;
            }
        }

        pooledObjects[index].numActiveExistingObject++;
        pooledObjects[index].numToGenerate++;

        GameObject tmp;
        tmp = Instantiate(pooledObjects[index].prefabsGo[UnityEngine.Random.Range(0, pooledObjects[index].prefabsGo.Count)], Vector3.zero, Quaternion.identity);
        tmp.SetActive(false);
        tmp.transform.SetParent(pooledObjects[index].containerGos.transform);
        tmp.name = tmp.name + "_" + pooledObjects[index].numActiveExistingObject;


        Debug.LogWarning($"Sin Espacio creado un nuevo prefab - existen ahora {pooledObjects[index].numActiveExistingObject} de {_type.ToString()}");

        return tmp;
    }

    public static List<GameObject> GetChildren(GameObject go)
    {
        List<GameObject> children = new List<GameObject>();

        if (go == null)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            System.Diagnostics.StackFrame frame = stackTrace.GetFrame(1);
            string fileName = frame.GetFileName();
            int line = frame.GetFileLineNumber();

            Debug.LogError($"NullReferenceException ObjectsPoolTypes.GetChildren(UnityEngine.GameObject go)(at Assets / Scripts /  {fileName} linea {line}) \n Posible causa: \n Se ha pedido un objeto en la objectpool ANTES de que se construyan aproximadamente se tardan unos 10 segundos al inicio de la aplicacion. \n Recomendacion se puede comprobar si la aplicacion esta recien iniciada con if(Time.realtimeSinceStartup > 20f) (Si la plicacion tiene mas de 10 segundos) ");

            return children;
        }


        foreach (Transform tran in go.transform)
        {
            children.Add(tran.gameObject);
        }
        return children;
    }

    public GameObject InstanciteObjectPool(TypeGameObjectIDKey _type, Vector3 _pos, Quaternion _rot)
    {
        GameObject _objectPool = GetPooledObject(_type);
        _objectPool.transform.position = _pos;
        _objectPool.transform.rotation = _rot;
        _objectPool.SetActive(true);


        return _objectPool;
    }


    public int GetNumberInventoryAccountOfExistingObjects(TypeGameObjectIDKey _type)
    {
        return pooledObjects[(int)_type].numActiveExistingObject;
    }



    /**
     * 
     * Para retornar el Gameobject una vez que se ha terminado o Muerto. Hay que borrar las variables de sus respectivos componentes que deban limpiarse. 
     * Por ejemplo Rigibody.Velocity = Vector3.zero;
     * 
     **/
    public void ReturnObjectToObjectPool(TypeGameObjectIDKey _type, GameObject go)
    {
        //Debug.Log($"Se retorna el objeto {go.name} del root {go.transform.root.name} a la etiqueta {_type.ToString()}");

        int index = (int)_type;

        pooledObjects[index].numActiveExistingObject--;

        //Caso: un decal se emparenta a un objeto, luego el objeto se destruye. el decal tambien se destruye; Como se descuenta su existencia no debe afectar
        if (!go)
        {
            pooledObjects[index].numToGenerate = -1;
            return;
        }

        go.transform.SetParent(null);

        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, this.gameObject.scene);  // SceneManager.GetSceneByName(m_Scene))

        go.SetActive(false);
        go.transform.SetParent(pooledObjects[index].containerGos.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
    }

    public void ReturnObjectToObjectPoolWaitSeconds(TypeGameObjectIDKey _type, GameObject go, float time)
    {
        StartCoroutine(CorroutineReturnObjectToObjectPoolWaitSeconds(_type, go, time));
    }

    IEnumerator CorroutineReturnObjectToObjectPoolWaitSeconds(TypeGameObjectIDKey _type, GameObject go, float time)
    {

        yield return new WaitForSeconds(time);

        ReturnObjectToObjectPool(_type, go);

    }
}