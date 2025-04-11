using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMadeRoom : MonoBehaviour
{
    [SerializeField] GameObject _madeRoom;
    [SerializeField] GameObject _destroyRoom;
    [SerializeField] GameObject _finishObject;

    private void Start()
    {
        _finishObject.SetActive(false);

        if (_madeRoom != null)
        {
            _madeRoom.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            if (_madeRoom != null)
            {
                _madeRoom.SetActive(true);
            }
            if (_destroyRoom != null)
            {
                _destroyRoom.SetActive(false);
            }
        }
    }
}