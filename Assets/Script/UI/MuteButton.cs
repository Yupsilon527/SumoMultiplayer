using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MuteButton : MonoBehaviour, iButton
{
    public AudioMixer controlled;
    public GameObject DisabledSprite;
    public string ParameterName;

    float ParameterDefaultValue;

    bool muted = false;

    private void Awake()
    {
        if (controlled.GetFloat(ParameterName, out float value))
        {
            ParameterDefaultValue = value;
        }
        else
        {
            ParameterDefaultValue = 0;
        }
        if (PlayerPrefs.HasKey(ParameterName + "_muted"))
        {
            SetMuted(PlayerPrefs.GetInt(ParameterName + "_muted") == 1);
        }
    }

    public void Pressed()
    {
        SetMuted(!muted);
    }

    void SetMuted(bool value)
    {
        muted = value;
        PlayerPrefs.SetInt(ParameterName + "_muted", muted ? 1 : 0);
        controlled.SetFloat(ParameterName, muted ? -80 : ParameterDefaultValue);
        DisabledSprite.SetActive(muted);
    }

    private void OnDisable()
    {
        SetMuted(false);
    }
}
