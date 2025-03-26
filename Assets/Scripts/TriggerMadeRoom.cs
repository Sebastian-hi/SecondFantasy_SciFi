using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMadeRoom : MonoBehaviour
{
    [SerializeField] GameObject _madeRoom;
    [SerializeField] GameObject _destroyRoom;

    private void Awake()
    {
        _madeRoom.SetActive(false);

        if (_destroyRoom != null)
        {
            _destroyRoom.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            _madeRoom.SetActive(true);

            if (_destroyRoom != null)
            {
                _destroyRoom.SetActive(false);
            }
        }
    }
}