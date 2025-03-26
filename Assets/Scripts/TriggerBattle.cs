using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBattle : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform[] points;
    [Space]
    [Space]
    [SerializeField] private bool _needBatteMusic;
    [SerializeField] private float _secondsBattleMusic;

    private float _fadeDuration = 1.5f; // было 2
    private AudioSource _activeBattleSource;


    private bool _isStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (!_isStarted)
        {
            if (player != null)
            {
                _isStarted = true;

                if (!Managers.Battle.UseUltraPower && _needBatteMusic)
                {
                    StartCoroutine(PlayTriggerMusic());
                }
                
                foreach (var point in points)
                {
                    GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);

                    SoldierEnemy soldier = enemy.GetComponent<SoldierEnemy>();
                }  
            }
        }
    }
    private IEnumerator PlayTriggerMusic()
    {
        yield return StartCoroutine(FadeOut(Managers.Audio.ambientSource, _fadeDuration));

        Managers.Audio.PlayRandomFightMusic();

        _activeBattleSource = Managers.Audio.fight1MusicSource.isPlaying
           ? Managers.Audio.fight1MusicSource
           : Managers.Audio.fight2MusicSource;

        yield return StartCoroutine(FadeIn(_activeBattleSource, _fadeDuration));

        yield return new WaitForSeconds(_secondsBattleMusic); // пока закончится бой ждём

        yield return StartCoroutine(FadeOut(_activeBattleSource, _fadeDuration));

        Managers.Audio.PlayAmbientSound();

        yield return StartCoroutine(FadeIn(Managers.Audio.ambientSource, _fadeDuration));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            yield break;
        }

        float startVolume = audioSource.volume;

        for (float t=0; t < duration; t+= Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null; // Чтобы не было моментально, а покадрово.
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        if (audioSource == null)
            yield break;

        float targetVolume = 1f;
        audioSource.volume = 0;
        audioSource.Play();

        for (float t = 0; t < duration;t+= Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }


}