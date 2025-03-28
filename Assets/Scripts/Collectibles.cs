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

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player = other.GetComponent<CharacterController>();

        if (player != null)
        {
            if (_nameItem == "Shield")
            {
                Managers.Player.IsShieldItem = true;

                _shieldUpdated.Play();
                Managers.Player.ChangeShield(100);
            }
            if (_nameItem == "Ammo")
            {
                Managers.Player.IsAmmoItem = true;

                _ammoUpdated.Play();
                Managers.Player.ChangeMaxAmmo(15);
            }

            if (_nameItem == "E-COIN" || _nameItem == "Great E-COIN")
            {
                int coinValue = _nameItem == "E-COIN" ? 1 : 3;
                Managers.Player.ChangeECoin(coinValue);

                Debug.Log($"ECoin: {Managers.Player.ECoin}, NextThreshold: {Managers.Player.NextThreshold}");

                _eCoinUpdated.Play();
                
            }

            Destroy(gameObject);
        }
    }

}
