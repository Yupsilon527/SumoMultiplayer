using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSelector : MonoBehaviour
{
    // Start is called before the first frame update
    private float _value=0;
    public float floatValue
    {
        get=>_value;
        set=> setFloatValue(value);
    }

    public AudioSource menuMusic=null;

    public bool playAtStart=false;
    public bool playMenuMusicAtStart=true;
    public double menuMusicFadingTime=2;
    private double menuMusicEnd=-1;
    public double startDelay=2;

    private static SoundSelector instance=null;
    public static SoundSelector Instance
    {
        get=>instance;
    }


    public float setFloatValue(float newValue)
    {
        _value=newValue;
        print("New float arrived "+newValue);
        foreach(SelectableSound sound in soundList)
        {
            sound.setVolumeAccordingToFloatValue(_value);
        }
        return _value;
    }
    public void StopGameMusic()
    {
        floatValue=0;
        foreach(SelectableSound sound in soundList)
        {
            if(sound.audioSource!=null)
                sound.audioSource.Stop();
        }
    }
    private SelectableSound [] soundList=null;
    void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
            return;
        }

        instance=this;
        DontDestroyOnLoad(this.gameObject);
            
        soundList=GetComponentsInChildren<SelectableSound>();
        floatValue=0;
        if(playAtStart)
            PlayGameMusicScheduled(AudioSettings.dspTime+startDelay);
        else
            if(playMenuMusicAtStart && menuMusic!=null)
                menuMusic.Play();
    }

    public void PlayMenuMusic()
    {
        StopGameMusic();
        menuMusicEnd=-1;
        if(menuMusic!=null)
        {
            menuMusic.volume=1;
            menuMusic.Play();
        }
    }

    public void PlayGameMusic()
    {
        PlayGameMusicScheduled(AudioSettings.dspTime+menuMusicFadingTime);
    }

    public void PlayGameMusicScheduled(double time)
    {
        if(menuMusic!=null)
        {
            menuMusicFadingTime=time - AudioSettings.dspTime;
            menuMusicEnd=time;
            menuMusic.SetScheduledEndTime(time);
        }
        foreach(SelectableSound sound in soundList)
        {
            if(sound.audioSource!=null)
                sound.audioSource.PlayScheduled(time);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(menuMusicEnd<0 || menuMusic==null)
            return;
        double time=menuMusicEnd-AudioSettings.dspTime;
        if(time<=0)
            return;
        menuMusic.volume=(float)(time/menuMusicFadingTime);
    }
}
