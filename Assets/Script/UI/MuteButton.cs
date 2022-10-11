using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MuteButton : MonoBehaviour, iButton
{
    public AudioMixer controlled;
    public string ParameterName;
    public float ParameterDefaultValue;

    bool muted = false;
    public void Pressed()
    {
        muted = !muted;
        controlled.SetFloat (ParameterName, muted ? 0 : ParameterDefaultValue);
    }
}
