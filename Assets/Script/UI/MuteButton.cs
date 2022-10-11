using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MuteButton : MonoBehaviour, iButton
{
    public AudioMixer controlled;
    public string ParameterName;
    public float ValueHigh;
    public float ValueLow;

    bool muted = false;
    public void Pressed()
    {
        muted = !muted;
        controlled.SetFloat (ParameterName, muted ? ValueLow : ValueHigh);
    }
}
