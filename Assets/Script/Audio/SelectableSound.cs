using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableSound : MonoBehaviour
{
    public float minValue=0;
    public float maxValue=1;
    public float fading=0.05f;
    private AudioSource source=null;
    private float volume=0;
    public AudioSource audioSource
    {
        get=>source;
    }
    public bool inRange(float floatValue)
    {
        return floatValue>=minValue && floatValue<=maxValue;
    }
    public void setVolumeAccordingToFloatValue(float floatValue)
    {
        if(floatValue<minValue-fading)
        {
            volume=0;
        } else
        if(floatValue<minValue)
        {
            volume=1-(minValue-floatValue)/fading;
        } else
        if(floatValue<maxValue)
        {
            volume=1;
        } else if(floatValue<maxValue+fading)
        {
            volume=(maxValue+fading-floatValue)/fading;
        } else
        {
            volume=0;
        }

        if(source!=null)
            source.volume=volume;
    }
    // Start is called before the first frame update
    void Awake()
    {
        source=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
