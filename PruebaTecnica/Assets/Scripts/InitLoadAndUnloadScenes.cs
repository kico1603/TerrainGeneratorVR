using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/***
 * 
 * InitLoadAndUnloadScenes es una clase que se encarga de cargar y descargar escenas de forma asíncrona. A continuación, se detallan sus partes principales:
 * 
 * Variables y listas:
 * 
 * nameScenesLoads y nameScenesUnloads son listas que almacenan los nombres de las escenas que se deben cargar y descargar, respectivamente.
 * isRunning indica si actualmente se está cargando o descargando una escena.
 * isRunningSceneChecker indica si el verificador de escenas está en ejecución.
 * actualSceneInLoad y actualSceneInUnload almacenan el nombre de la escena que se está cargando o descargando actualmente.
 *
 * Método Awake: Este método se ejecuta al inicio y crea una instancia única de esta clase, asegurándose de que solo haya una instancia en todo el proyecto. Además, inicializa las listas y variables mencionadas anteriormente.
 *
 * Método StartInitLoadScenes: Este método acepta un nombre de escena y, si no se está cargando actualmente, inicia el proceso de carga de la escena. Si la escena ya se está cargando, muestra un mensaje de error.
 * 
 * Método StartInitUnloadScenes: Similar al método anterior, pero para descargar escenas. Acepta un nombre de escena y, si no se está descargando actualmente, inicia el proceso de descarga de la escena. Si la escena ya se está descargando, muestra un mensaje de error.
 *
 * Corrutina ScenesChecker: Esta corrutina verifica si hay escenas en las listas nameScenesLoads y nameScenesUnloads que necesiten ser cargadas o descargadas. Si es así, llama a las corrutinas InitLoadScene o InitUnloadScene, según corresponda.
 * 
 * Corrutina InitLoadScene: Esta corrutina acepta un nombre de escena y carga la escena de forma asíncrona. Cuando la escena termina de cargarse, elimina el nombre de la escena de la lista nameScenesLoads.
 *
 * Corrutina InitUnloadScene: Similar a la corrutina anterior, pero para descargar escenas. Acepta un nombre de escena y descarga la escena de forma asíncrona. Cuando la escena termina de descargarse, elimina el nombre de la escena de la lista nameScenesUnloads.
 * 
 * En resumen, esta clase permite cargar y descargar escenas de forma asíncrona en Unity, evitando bloquear la ejecución del juego mientras se realizan estas operaciones.
 * 
 * Ejemplo:
 *
 * InitLoadAndUnloadScenes.Instance.StartInitUnloadScenes("Login");
 * 
 * InitLoadAndUnloadScenes.Instance.StartInitLoadScenes("Welcome");
 * 
 * 
***/


public class InitLoadAndUnloadScenes : MonoBehaviour
{
    private static List<string> nameScenesLoads;
    private static List<string> nameScenesUnloads;
    public UnityAction<string, bool> actionScene;


    public bool isRunning = false;
    public bool isRunningSceneChecker = false;

    public string actualSceneInLoad;

    public string actualSceneInUnload;

    public static InitLoadAndUnloadScenes Instance;

    public bool loadSceneInStart = false;

    public string sceneLoadedInStart;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;

        }
        else
        {
            Destroy(this.gameObject);
        }


        isRunning = false;
        isRunningSceneChecker = false;
        nameScenesLoads = new List<string>();
        nameScenesUnloads = new List<string>();
    }

    private void Start()
    {
        if (loadSceneInStart)
        {
            StartCoroutine(StartInitLoadScenes(sceneLoadedInStart));
        }
    }

    [ContextMenu(nameof(DEBUG_StartInitLoadScenes))]
    public void DEBUG_StartInitLoadScenes()
    {
        StartInitLoadScenes(actualSceneInLoad);
    }

    public IEnumerator StartInitLoadScenes(string sceneLoads)
    {
        Debug.Log(" (!) >>>> Se pretende cargar la escena " + sceneLoads);

        if (isRunning && actualSceneInLoad.Equals(sceneLoads))
        {
            Debug.LogError(" !!!!!! Se pretende cargar la escena que ya esta en proceso! " + sceneLoads);

            if (nameScenesLoads.Contains(actualSceneInLoad))
                nameScenesLoads.Remove(actualSceneInLoad);

            yield break;
        }

        if (isRunning)
        {
            nameScenesLoads.Add(sceneLoads);

            if (!isRunningSceneChecker)
            {
                yield return StartCoroutine(ScenesChecker());
            }

        }
        else
        {
            if (SceneManager.GetSceneByName(sceneLoads).isLoaded)
            {
                Debug.LogError("!!!! Se intenta cargar una escena ya cargada Escena: " + sceneLoads);
            }
            else
            {
                yield return StartCoroutine(InitLoadScene(sceneLoads));
            }
        }

        yield return null;
    }

    [ContextMenu(nameof(DEBUG_StartInitUnloadScenes))]
    public void DEBUG_StartInitUnloadScenes()
    {
        StartInitUnloadScenes(actualSceneInUnload);
    }

    public void StartInitUnloadScenes(string sceneUnloads)
    {
        Debug.Log(" (¡) <<<< Se pretende descargar la escena " + sceneUnloads);

        if (isRunning && actualSceneInUnload.Equals(sceneUnloads))
        {
            Debug.LogError(" ¡¡¡¡¡¡ Se pretende descargar la escena que ya esta en proceso! " + sceneUnloads);

            if (nameScenesUnloads.Contains(actualSceneInUnload))
                nameScenesUnloads.Remove(actualSceneInUnload);

            return;
        }


        if (isRunning)
        {
            nameScenesUnloads.Add(sceneUnloads);

            if (!isRunningSceneChecker)
            {
                StartCoroutine(ScenesChecker());
            }

        }
        else
        {
            if (!SceneManager.GetSceneByName(sceneUnloads).isLoaded)
            {
                Debug.LogError("!!!! Se intenta descargar una escena ya descargada Escena: " + sceneUnloads);
            }
            else
            {
                StartCoroutine(InitUnloadScene(sceneUnloads));
            }
        }


    }


    /**
     * 
     * Este código muestra un ciclo while que se ejecuta mientras nameScenesLoads tenga elementos.
     * Si isRunning es verdadero, se espera un segundo y se continúa con el ciclo while. 
     * Si es falso, se toma el primer elemento de nameScenesLoads y se comprueba si la escena está cargada. 
     * Si es así, se muestra un mensaje de error y se elimina el elemento de la lista. 
     * Si no está cargado, se llama a la función InitLoadScene con el elemento como parámetro. 
     * Después de esto, se espera dos segundos y se continúa con el ciclo while.
     *
     */

    IEnumerator ScenesChecker()
    {
        isRunningSceneChecker = true;

        while (nameScenesLoads.Count > 0)
        {
            if (isRunning)
            {
                yield return new WaitForSeconds(1f);

                continue;
            }

            string sceneLoads = nameScenesLoads[0];

            if (SceneManager.GetSceneByName(sceneLoads).isLoaded)
            {
                Debug.LogError("!!!! Se intenta cargar una escena ya cargada Escena: " + sceneLoads);
                nameScenesLoads.Remove(sceneLoads);

            }
            else
            {
                StartCoroutine(InitLoadScene(sceneLoads));
                nameScenesLoads.Remove(sceneLoads);
            }

            yield return new WaitForSeconds(2f);

            continue;

        }

        while (nameScenesUnloads.Count > 0)
        {
            if (isRunning)
            {
                yield return new WaitForSeconds(1f);

                continue;
            }

            string sceneUnloads = nameScenesUnloads[0];

            if (!SceneManager.GetSceneByName(sceneUnloads).isLoaded)
            {
                Debug.LogError("!!!! Se intenta descargar una escena ya descargada Escena: " + sceneUnloads);
                nameScenesUnloads.Remove(sceneUnloads);

            }
            else
            {
                StartCoroutine(InitUnloadScene(sceneUnloads));
                nameScenesUnloads.Remove(sceneUnloads);
            }

            yield return new WaitForSeconds(2f);

            continue;

        }

        if (nameScenesLoads.Count > 0 || nameScenesUnloads.Count > 0)
        {
            StartCoroutine(ScenesChecker());
        }

        isRunningSceneChecker = false;
    }

    IEnumerator InitLoadScene(string sceneName)
    {
        isRunning = true;
        actualSceneInLoad = sceneName;

        // Verifica si la escena está en los Build Settings
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) < 0)
        {
            Debug.LogError($"Error en la lectura de la escena {sceneName}. Comprobar que está en el Build Settings y que el nombre está bien escrito y es correcto.");
            isRunning = false;
            yield break; // Salir de la coroutine si la escena no está en Build Settings
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Espera hasta que la carga esté casi completa
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // La carga está completa al 90%, solo falta la activación
            {
                // Aquí podrías mostrar un mensaje al usuario o hacer una cuenta regresiva antes de activar la escena
                asyncLoad.allowSceneActivation = true; // Activar la escena cuando estés listo
            }
            yield return null;
        }

        // Limpieza tras la carga de la escena
        if (nameScenesLoads.Contains(sceneName))
        {
            nameScenesLoads.Remove(sceneName);
        }

        isRunning = false;
        actualSceneInLoad = "";
        actionScene?.Invoke(sceneName, true);
    }


    IEnumerator InitUnloadScene(string sceneUnload)
    {
        isRunning = true;

        actualSceneInUnload = sceneUnload;

        AsyncOperation asyncLoad = null;

        if (asyncLoad == null)
        {
            asyncLoad = SceneManager.UnloadSceneAsync(sceneUnload);
        }

        if (asyncLoad == null)
        {
            Debug.LogError($"Error en la lectura de la escena {sceneUnload} comprobar que esta en el buildsettings y que el nombre esta bien escrito y es correcto");

            if (nameScenesUnloads.Contains(sceneUnload))
            {
                nameScenesUnloads.Remove(sceneUnload);
            }

            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (nameScenesUnloads.Contains(sceneUnload))
        {
            nameScenesUnloads.Remove(sceneUnload);
        }

        actualSceneInUnload = "";
        isRunning = false;
        actionScene?.Invoke(sceneUnload, false);
        yield break;
    }

}