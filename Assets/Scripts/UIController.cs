using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _HUDcanvas;
    [SerializeField] private GameObject _BackGroundHUD1;
    [SerializeField] private TMP_Text _shieldText;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private GameObject _levelFailedLabel;
    [SerializeField] private GameObject _allCoinsCollected;
    [SerializeField] private GameObject _UltraDamageLabel;
    [SerializeField] private GameObject _UltraDamageLabel2;
    [SerializeField] private GameObject _AimPlayer;
    [Space]
    [Space]
    [SerializeField] private GameObject _mainCam;
    [SerializeField] private GameObject _skyCam;
    [Space]
    [Space]
    [SerializeField] Hurt hurt;
    [Space]
    [Space]
    [SerializeField] private GameObject _plusShield;
    [SerializeField] private GameObject _DangerMinShield;
    [Space]
    [Space]
    [SerializeField] private GameObject _plusAmmo;
    [Space]
    [SerializeField] private GameObject _levelCompleteLabel;
    [SerializeField] private GameObject _GameComplete;

    private bool _isWarningActive = false;


    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.SHIELD_UPDATED, OnShieldUpdated);
        Messenger.AddListener(GameEvent.MONEY_UPDATED, OnMoneyUpdated);
        Messenger.AddListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvent.GAME_COMPLETE, OnGameComplete);
        Messenger.AddListener(GameEvent.AMMO_UPDATED, OnAmmoUpdated);
        Messenger.AddListener(GameEvent.ALL_MONEY_COLLECTED, OnAllMoneyCollected);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.SHIELD_UPDATED, OnShieldUpdated);
        Messenger.RemoveListener(GameEvent.MONEY_UPDATED, OnMoneyUpdated);
        Messenger.RemoveListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.RemoveListener(GameEvent.GAME_COMPLETE, OnGameComplete);
        Messenger.RemoveListener(GameEvent.AMMO_UPDATED, OnAmmoUpdated);
        Messenger.RemoveListener(GameEvent.ALL_MONEY_COLLECTED, OnAllMoneyCollected);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
    }

    private void Start()
    {
        _mainCam.SetActive(true);
        _skyCam.SetActive(false);
        _HUDcanvas.SetActive(true);
        _levelCompleteLabel.SetActive(false);
        _GameComplete.SetActive(false);
        _plusShield.SetActive(false);
        _plusAmmo.SetActive(false);
        _UltraDamageLabel.SetActive(false);
        _UltraDamageLabel2.SetActive(false);
        _allCoinsCollected.SetActive(false);
        _levelFailedLabel.SetActive(false);
        _BackGroundHUD1.SetActive(true);
        _AimPlayer.SetActive(true);

        OnShieldUpdated();
        OnMoneyUpdated();
    }

    private void Update()
    {
        OnAmmoUpdated();

        CheckUltraPower();

        if (Managers.Player.Shield < 100 && Managers.Player.Shield > 0)
        {
            if (!_isWarningActive)
            {
                StartCoroutine(CheckMinShield());
            }
        }
        else 
        { 
            _DangerMinShield.SetActive(false);
            _isWarningActive = false;
        }

        if (Managers.Battle.UseUltraPower)
        {
            OnMoneyUpdated();
        }
    }

    private void CheckUltraPower()
    {
        if (Managers.Battle.haveUltraPower)
            _UltraDamageLabel.SetActive(true);

        else if (Managers.Battle.UseUltraPower)
        {
            _UltraDamageLabel.SetActive(false);
            _UltraDamageLabel2.SetActive(true);
        }
        else
        {
            _UltraDamageLabel.SetActive(false);
            _UltraDamageLabel2.SetActive(false);
        }
        
    }

    private IEnumerator CheckMinShield()
    {
        _isWarningActive = true;

        while (Managers.Player.Shield < 100 && Managers.Player.Shield > 0)
        {
            _DangerMinShield.SetActive(true);
            yield return new WaitForSeconds(1f);
            _DangerMinShield.SetActive(false);
            yield return new WaitForSeconds(1f);

        }

        _isWarningActive = false;

    }


    private void OnLevelComplete()
    {
        StartCoroutine(LevelCompleteCOR());
    }

    private IEnumerator LevelCompleteCOR()
    {
        _BackGroundHUD1.SetActive(false);
        _AimPlayer.SetActive(false);
        _mainCam.SetActive(false);
        _skyCam.SetActive(true);
        _levelCompleteLabel.SetActive(true);
        yield return new WaitForSeconds(4f);
        Managers.Mission.GoToNextLevel();
    }

    private void OnAllMoneyCollected()
    {
        StartCoroutine(AllCoinsCollected());
    }

    private IEnumerator AllCoinsCollected()
    {
        _allCoinsCollected.SetActive(true);
        yield return new WaitForSeconds(5f);
        _allCoinsCollected.SetActive(false);

        _UltraDamageLabel.SetActive(true);
    }

    private void OnAmmoUpdated()
    {
        if (Managers.Player.isAmmoItem)
        {
            StartCoroutine(AmmoItemUI());
        }
        string messageAmmo = $"{Managers.Player.CurAmmo} / {Managers.Player.MaxAmmo}";
        _ammoText.text = messageAmmo;
    }

    private IEnumerator AmmoItemUI()
    {
        _plusAmmo.SetActive(true);
        yield return new WaitForSeconds(2f);
        Managers.Player.isAmmoItem = false;
        _plusAmmo.SetActive(false);
    }

    private void OnShieldUpdated()
    {
        if (Managers.Player.isShieldItem)
        {
            StartCoroutine(ShieldItemUI());
        }
        string messageShield = $"{Managers.Player.Shield} / {Managers.Player.MaxShield}";
        _shieldText.text = messageShield;
    }

    private IEnumerator ShieldItemUI()
    {
        _plusShield.SetActive(true);
        yield return new WaitForSeconds(3f);
        Managers.Player.isShieldItem = false;
        _plusShield.SetActive(false);
    }

    private void OnMoneyUpdated()
    {
        string moneyMessage = $"{Managers.Player.ECoin} / {Managers.Player.MaxECoin} $";
        _moneyText.text = moneyMessage;
    }

    private void OnLevelFailed()
    {
        StartCoroutine(OnLevelFailedCor());
    }

    private IEnumerator OnLevelFailedCor()
    {
        Managers.Audio.StopMusic();
        yield return new WaitForSeconds(3f);
        Managers.Audio.levelFailedSource.Play();
        _AimPlayer.SetActive(false);
        _BackGroundHUD1.SetActive(false);
        _mainCam.SetActive(false);

        _skyCam.SetActive(true);
        _levelFailedLabel.SetActive(true);
        yield return new WaitForSeconds(5f);
        
        Managers.Player.Respawn(); // просто задаёт щит и т.д.
        Managers.Mission.RestartCurrent();
        
    }

    private void OnGameComplete()
    {
        _AimPlayer.SetActive(false);
        _GameComplete.SetActive(true);

        // возвращаем на главный экран.
    }
}