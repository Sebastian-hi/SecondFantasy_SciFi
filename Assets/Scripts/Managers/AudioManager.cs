using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    public AudioSource playOneShotSource;
    public AudioSource ambientSource;
    public AudioSource allMoneyCollected;
    [Space]
    public AudioSource fight1MusicSource;
    public AudioSource fight2MusicSource;
    public AudioSource UltraPowerSource;

    public ManagerStatus status { get; private set; }

    private float _musicVolume;

    public void Startup()
    {
        Debug.Log("AudioManager starting");

        status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip clip)
    {
        playOneShotSource.PlayOneShot(clip);
    }

    public float SoundVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public bool SoundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    public float musicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;
            fight1MusicSource.volume = _musicVolume;
            ambientSource.volume = _musicVolume;
            allMoneyCollected.volume = _musicVolume;
            UltraPowerSource.volume = _musicVolume;
        }
    }
    
    public bool musicMute
    {
        get
        {
            if (fight1MusicSource != null)
            {
                return fight1MusicSource.mute;
            }

            return false;
        }
        set
        {
            if (fight1MusicSource != null)
            {
                fight1MusicSource.mute = value;
                ambientSource.mute = value;
                allMoneyCollected.mute = value;
                UltraPowerSource.mute = value;
            }
        }

    }

    public void StopMusic()
    {
        fight1MusicSource.Stop();
        ambientSource.Stop();
        allMoneyCollected.Stop();
        UltraPowerSource.Stop();
    }
}