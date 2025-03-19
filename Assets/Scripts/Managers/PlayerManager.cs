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

    public int MaxECoin { get; private set; }

    public int CurAmmo { get; private set; }   // может быть не больше 4/5.

    public int MaxAmmo { get; private set; }        // в начале 20

    private Animator _animator;

    public bool isShieldItem { get;  set; }

    public bool isAmmoItem { get;  set; }

    public void Startup()
    {
        Debug.Log("Player manager starting");
        _animator = GetComponent<Animator>();

        Respawn();
        status = ManagerStatus.Started;
    }

    public void Respawn()
    {
        UpdateData(200, 350, 0, 15, 5, 20);
    }

    public void UpdateData(int Shield, int MaxShield, int ECoin, int MaxECoin, int CurAmmo, int MaxAmmo)
    {
        this.Shield = Shield;
        this.MaxShield = MaxShield;
        this.ECoin = ECoin;
        this.MaxECoin = MaxECoin;
        this.CurAmmo = CurAmmo;
        this.MaxAmmo = MaxAmmo;
    }

    public void ChangeECoin(int value)
    {
        ECoin += value;

        if (ECoin > MaxECoin)
        {
            ECoin = MaxECoin;
        }
        if (ECoin == MaxECoin)
        {
            Messenger.Broadcast(GameEvent.ALL_MONEY_COLLECTED);
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

            Debug.Log("ўит прорван, јйзек is dead.");
            _animator.SetBool("DeadPlayer", true);

            Messenger.Broadcast(GameEvent.LEVEL_FAILED);
        }
        Messenger.Broadcast(GameEvent.SHIELD_UPDATED);
    }

    public void ChangeAmmo(int value)
    {
        if (value < 0) // value с минусом при стрельбе, с плюсом при перезар€дке
        {
            CurAmmo += value;
            if (CurAmmo < 0)
                CurAmmo = 0;
        }
        else
        {
            int ammoToLoad = Mathf.Min(value, MaxAmmo);
            CurAmmo = Mathf.Min(CurAmmo + ammoToLoad, 5);
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
}
