using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] private string _nameItem;
    [SerializeField] private AudioSource _eCoinUpdated;
    [SerializeField] private AudioSource _ammoUpdated;
    [SerializeField] private AudioSource _shieldUpdated;

    private static bool _allCoinsCollectedYet = false;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            if (_nameItem == "Shield")
            {
                Managers.Player.isShieldItem = true;

                _shieldUpdated.Play();
                Managers.Player.ChangeShield(100);
            }
            if (_nameItem == "Ammo")
            {
                Managers.Player.isAmmoItem = true;

                _ammoUpdated.Play();
                Managers.Player.ChangeMaxAmmo(15);
            }

            if (_nameItem == "E-COIN" || _nameItem == "Great E-COIN")
            {
                int previousCoinCount = Managers.Player.ECoin; // количество Коинов до изменений.

                if (previousCoinCount < Managers.Player.MaxECoin)
                {
                    if (_nameItem == "E-COIN")
                    {
                        Managers.Player.ChangeECoin(1);
                    }
                    else if (_nameItem == "Great E-COIN")
                    {
                        Managers.Player.ChangeECoin(3);
                    }
                }

                int newCoinCount = Managers.Player.ECoin;

                if (newCoinCount == Managers.Player.MaxECoin && !_allCoinsCollectedYet)
                {
                    StartCoroutine(playAllCoinsCollected());

                    Managers.Battle.haveUltraPower = true;
                    _allCoinsCollectedYet = true;

                }
                else
                {
                    _eCoinUpdated.Play();
                }
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator playAllCoinsCollected()
    {
        Managers.Audio.StopMusic();
        Managers.Audio.allMoneyCollected.Play();
        yield return new WaitForSeconds(Managers.Audio.allMoneyCollected.clip.length);
        Managers.Audio.ambientSource.Play();
    }
}
