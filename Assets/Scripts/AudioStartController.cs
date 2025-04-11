using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStartController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StartAmbient());
    }

    private IEnumerator StartAmbient()
    {
        yield return new WaitForSeconds(1f);
        Managers.Audio.PlayAmbientSound();
    }
}
