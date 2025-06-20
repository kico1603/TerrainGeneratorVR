using UnityEngine;

public class PanelConfigWorld : MonoBehaviour
{
    [SerializeField] private ConfigWorld configWorld;

    

    public void OnChangeSizeChunks(float value)
    {
        configWorld.sizeChunks = Mathf.RoundToInt(value);
    }

    public void OnChangeNumChunks(float value)
    {
        configWorld.numChunks = Mathf.RoundToInt(value);
    }

    public void OnChangeSeed(float value)
    {
        configWorld.seed = Mathf.RoundToInt(value);
    }

    public void OnChangeMinimHeight(float value)
    {
        configWorld.minimHeight = Mathf.RoundToInt(value);
    }

    public void OnChangeScaleNoise(float value)
    {
        configWorld.scaleNoise = value;
    }

    public void OnChangeOffsetNoise(float value)
    {
        configWorld.offsetNoise = Mathf.RoundToInt(value);
    }

  
}
