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
        yield return new WaitForSeconds(2f);
        Managers.Audio.ambientSource.Play();
        yield return new WaitForSeconds(Managers.Audio.ambientSource.clip.length);
        yield return new WaitForSeconds(5f);
        StartCoroutine(StartAmbient());
    }
}
