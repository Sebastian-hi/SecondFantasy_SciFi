using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBattle : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform[] points;

    private bool _isStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (!_isStarted)
        {
            if (player != null)
            {
                _isStarted = true;

                foreach (var point in points)
                {
                    GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);

                    SoldierEnemy soldier = enemy.GetComponent<SoldierEnemy>();
                }  
            }
        }
    }
}