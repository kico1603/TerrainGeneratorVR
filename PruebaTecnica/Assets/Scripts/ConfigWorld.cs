using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Utils/ConfigWorld")]
public class ConfigWorld : ScriptableObject
{
    [Range(5, 50)]  public int sizeChunks;
    [Range(1, 10)] public int numChunks;
    public int seed;
    [Range(1, 5)] public int minimHeight;
    public int maxHeight;
    [Range(0.01f, 0.5f)] public float scaleNoise;
    public int offsetNoise;

}
