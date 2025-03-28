using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TerminalGo : MonoBehaviour
{
    [SerializeField] GameObject[] _objectToDestroy;
    [SerializeField] GameObject[] _effectsTeleport;
    [SerializeField] AudioClip _clipForSend;
    [SerializeField] GameObject _createObject;

    private bool _playerInRange = false;
    private bool _alreadyStarted = false;

    private void Start()
    {
        _createObject.SetActive(false);

        foreach (GameObject effect in _effectsTeleport)
        {
            effect.SetActive(false);
        }
    }

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !_alreadyStarted)
        {
            _alreadyStarted = true;
            Managers.Audio.PlaySound(_clipForSend);

            StartCoroutine(SendToSpaceObjects());

            if (_createObject != null)
            {
                _createObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null) 
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            _playerInRange = false;
        }
    }

    private IEnumerator SendToSpaceObjects()
    {
        foreach (GameObject effect in _effectsTeleport)
        {
            effect.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (GameObject objects in _objectToDestroy)
        {
            Destroy(objects);
        }
    }
}