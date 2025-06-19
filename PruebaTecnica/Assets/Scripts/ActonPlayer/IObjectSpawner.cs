using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectSpawner
{
    /// <summary>Instancia el objeto en la posici�n (y rotaci�n) indicada.</summary>
    /// <param name="position">Posici�n donde crear el objeto.</param>
    /// <param name="rotation">Rotaci�n del nuevo objeto.</param>
    void SpawnObject(Vector3 position, Quaternion rotation);
}
