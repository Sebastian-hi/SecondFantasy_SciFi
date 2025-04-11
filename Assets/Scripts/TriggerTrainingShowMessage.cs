using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrainingShowMessage : MonoBehaviour
{
    [SerializeField] private GameObject _message;

    private bool _isStarted = false;

    private void Start()
    {
        _message.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && !_isStarted)
        {
            StartCoroutine(ShowMessageJetpack());
            _isStarted = true;
        }
    }

    private IEnumerator ShowMessageJetpack()
    {
        _message.SetActive(true);
        yield return new WaitForSeconds(5f);
        _message.SetActive(false);
    }


}
