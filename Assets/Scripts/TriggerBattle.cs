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

    private float _fadeDuration = 1.2f; // было 2
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
        yield return StartCoroutine(Managers.Audio.FadeOut(Managers.Audio.ambientSource, _fadeDuration));

        Managers.Audio.PlayRandomFightMusic();

        _activeBattleSource = Managers.Audio.fight1MusicSource.isPlaying
           ? Managers.Audio.fight1MusicSource
           : Managers.Audio.fight2MusicSource;

        yield return StartCoroutine(Managers.Audio.FadeIn(_activeBattleSource, _fadeDuration));

        yield return new WaitForSeconds(_secondsBattleMusic); // пока закончится бой ждём

        yield return StartCoroutine(Managers.Audio.FadeOut(_activeBattleSource, _fadeDuration));

        Managers.Audio.PlayAmbientSound();

        yield return StartCoroutine(Managers.Audio.FadeIn(Managers.Audio.ambientSource, _fadeDuration));
    }


}