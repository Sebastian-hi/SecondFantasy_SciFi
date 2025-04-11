using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public int Shield { get; private set; }

    public int MaxShield { get; private set; }

    public int ECoin { get; private set; }

    public int PriceUltraDamage { get; private set; } = 12;

    public int NextThreshold { get; set; } = 12;

    public int CurAmmo { get; private set; }

    public int MaxAmmo { get; private set; }

    public int AmmoInRifleMagazine { get; private set; } = 7;

    public bool IsShieldItem { get;  set; }

    public bool IsAmmoItem { get;  set; }

    [NonSerialized] public Transform playerTransform;

    [NonSerialized] public bool PlayerIsDead = false;

    public bool InMenu { get; set; } = false;

    public void Startup()
    {
        Debug.Log("Player manager starting");

        Respawn();

        FindPlayer();

        status = ManagerStatus.Started;
    }

    public void Respawn()
    {
        UpdateData(200, 350, 0, 12, 7, 20);
    }

    public void UpdateData(int Shield, int MaxShield, int ECoin, int PriceUltraDamage, int CurAmmo, int MaxAmmo)
    {
        this.Shield = Shield;
        this.MaxShield = MaxShield;
        this.ECoin = ECoin;
        this.PriceUltraDamage = PriceUltraDamage;
        this.CurAmmo = CurAmmo;
        this.MaxAmmo = MaxAmmo;
    }

    public void ChangeECoin(int value)
    {
        ECoin += value;

        while (ECoin >= NextThreshold)
        {
            Messenger.Broadcast(GameEvent.ALL_MONEY_COLLECTED);

            Managers.Audio.allMoneyCollected.Play();

            NextThreshold += PriceUltraDamage;
        }

        Messenger.Broadcast(GameEvent.MONEY_UPDATED);
    }
    

    public void ChangeShield(int value)
    {
        Shield += value;

        if (Shield > MaxShield)
        {
            Shield = MaxShield;
        }
        if (Shield < 0)
        {
            Shield = 0;
        }

        if (Shield == 0)
        {
            Shield = 0;

            Debug.Log("Щит прорван, Айзек is dead.");

            Messenger.Broadcast(GameEvent.LEVEL_FAILED);
        }
        Messenger.Broadcast(GameEvent.SHIELD_UPDATED);
    }

    public void ChangeAmmo(int value)
    {
        if (value < 0) // value с минусом при стрельбе, с плюсом при перезарядке
        {
            CurAmmo += value;

            if (CurAmmo < 0)
                CurAmmo = 0;
        }
        else
        {
            int ammoToLoad = Mathf.Min(value, MaxAmmo);
            CurAmmo = Mathf.Min(CurAmmo + ammoToLoad, AmmoInRifleMagazine);
            MaxAmmo -= ammoToLoad;
        }

        if (Managers.Player.MaxAmmo <= 0)
        {
            MaxAmmo = 0;
        }

        Messenger.Broadcast(GameEvent.AMMO_UPDATED);
    }

    public void ChangeMaxAmmo(int value)
    {
        MaxAmmo += value;
        Messenger.Broadcast(GameEvent.AMMO_UPDATED);
    }

    public void UseUltraDamageMinCoin()
    {
        ECoin -= PriceUltraDamage;

        if (ECoin < NextThreshold - PriceUltraDamage)
        {
            NextThreshold = Mathf.Max(PriceUltraDamage, (ECoin / PriceUltraDamage + 1) * PriceUltraDamage); 
                                                // если ECOIN = 15, а Price 12, то 15/12 = 1.
                                                // Далее + 1 = 2.
                                                // и *12 = 24.
                                                // Max(12/24) = 24. Это следующий порог!
        }
    }

    public void FindPlayer()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
       
        if (playerTransform == null)
        {
            Debug.LogWarning("Игрок не найден!");
        }
    }
}
