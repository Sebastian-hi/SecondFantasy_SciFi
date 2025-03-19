using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private AudioSource _finishSource;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            PlayFinish();
        }
    }

    private void PlayFinish()
    {
        Managers.Audio.StopMusic();
        _finishSource.Play();

        Managers.Mission.ReachObjective(); // делает Broadcast LEVEL_COMPLETE
    }
}