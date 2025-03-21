using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSkyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null )
        {
            Messenger.Broadcast(GameEvent.LEVEL_FAILED);
        }
    }
}