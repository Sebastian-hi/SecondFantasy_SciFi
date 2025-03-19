using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    [SerializeField] AudioSource levelFailedSource;

    public void HurtPlayer(int value)
    {
        Managers.Player.ChangeShield(value);
    }

    public void PlayAudioDefeat()
    {
        Managers.Audio.StopMusic();

        levelFailedSource.Play();
    }
}
