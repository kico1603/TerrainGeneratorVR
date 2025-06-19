using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectSpawner
{
    /// <summary>Instancia el objeto en la posición (y rotación) indicada.</summary>
    /// <param name="position">Posición donde crear el objeto.</param>
    /// <param name="rotation">Rotación del nuevo objeto.</param>
    void SpawnObject(Vector3 position, Quaternion rotation);
}
