using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    public AudioSource playOneShotSource;
    public AudioSource ambientSource;
    public AudioSource allMoneyCollected;
    public AudioSource levelFailedSource;

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
            fight2MusicSource.volume= _musicVolume;
            ambientSource.volume = _musicVolume;
            allMoneyCollected.volume = _musicVolume;
            UltraPowerSource.volume = _musicVolume;
            levelFailedSource.volume = _musicVolume;
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
                fight2MusicSource.mute = value;
                ambientSource.mute = value;
                allMoneyCollected.mute = value;
                UltraPowerSource.mute = value;
                levelFailedSource.mute = value;
            }
        }

    }

    public void StopMusic()
    {
        fight1MusicSource.Stop();
        fight2MusicSource.Stop();
        ambientSource.Stop();
        allMoneyCollected.Stop();
        UltraPowerSource.Stop();
        levelFailedSource.Stop();
    }

    public void PlayRandomFightMusic()
    {
        int randomMusic = Random.Range(0, 2); // 0 или 1.
            
        if (randomMusic == 0) 
            fight1MusicSource.Play();
        
        else fight2MusicSource.Play();
    }
}