using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSelector : MonoBehaviour
{
    public double randomPickingPeriod=5;
    private double randomPickingTimer=-1;
    private bool gameMusicPlaying=false;
    // Start is called before the first frame update
    private float _value=0;
    public float floatValue
    {
        get=>_value;
        set=> setFloatValue(value);
    }

    private float _randomValue=0.1f;
    public float randomValue
    {
        get=>randomValue;
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

    public void pickRandom()
    {
        _randomValue=Random.Range(0f,1f);
    }


    public float setFloatValue(float newValue)
    {
        _value=newValue;
        foreach(SelectableSound sound in soundList)
        {
            if (sound!=null)
                sound.setVolumeAccordingToFloatValue(_value,_randomValue);
        }
        return _value;
    }
    public void StopGameMusic()
    {
        floatValue=0;
        gameMusicPlaying=false;
        foreach(SelectableSound sound in soundList)
        {
            if(sound.audioSource!=null)
                sound.audioSource.Stop();
        }
    }
    private SelectableSound [] soundList=null;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        soundList = GetComponentsInChildren<SelectableSound>();
    }
    private void Start()
    {

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
        gameMusicPlaying=true;
        randomPickingTimer=time;

        foreach(SelectableSound sound in soundList)
        {
            if(sound.audioSource!=null)
                sound.audioSource.PlayScheduled(time);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        double gameMusicTimer=AudioSettings.dspTime-randomPickingTimer;
        if(gameMusicPlaying && gameMusicTimer>=randomPickingPeriod)
        {
            pickRandom();
            randomPickingTimer+=randomPickingPeriod;
            setFloatValue(_value);
            print("["+gameObject.name+"] Random solo picked :"+_randomValue);
        }
        if(menuMusicEnd<0 || menuMusic==null)
            return;
        double time=menuMusicEnd-AudioSettings.dspTime;
        if(time<=0)
            return;
        menuMusic.volume=(float)(time/menuMusicFadingTime);
    }
}
