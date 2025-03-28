using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayAmbient : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>())
        {
            Managers.Audio.PlayAmbientSound();
        }
    }
}
