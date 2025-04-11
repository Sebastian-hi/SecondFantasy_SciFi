using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

public class UIMenuInLevel : MonoBehaviour
{
    [SerializeField] private PlayerShoot _playerShoot;
    [SerializeField] private GameObject _menuLevel;
    [SerializeField] private AudioClip _clipClickButton;
    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;
    [SerializeField] private GameObject _giveHintAboutMenu;

    private Slider _musicVolumeSlider;
    private Slider _soundVolumeSlider;

    private void Start()                                        // RestartCurrent уже есть в Mission
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _menuLevel.SetActive(false);

        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.value = Managers.Audio.MusicVolume;
            _musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);

            _soundVolumeSlider.value = Managers.Audio.SoundVolume;
            _soundVolumeSlider.onValueChanged.AddListener(ChangeSoundVolume);
        }

        StartCoroutine(GiveHintMenu());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        { 
            Managers.Audio.PlaySound(_clipClickButton);
            bool isActive = !_menuLevel.activeSelf;
            _menuLevel.SetActive(isActive);

            Managers.Player.InMenu = isActive;

            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = isActive ? 0f : _playerShoot.NormalSpeedXCam;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = isActive ? 0f : 4f;

            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isActive;
        }
    }

    private IEnumerator GiveHintMenu()
    {
        _giveHintAboutMenu.SetActive(true);
        yield return new WaitForSeconds(5f);
        _giveHintAboutMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Managers.Audio.StopMusic();
        Managers.Audio.PlaySound(_clipClickButton);
        Debug.Log("Выходим в главное меню");
        SceneManager.LoadScene("StartMenu");
    } 

    public void ChangeMusicVolume(float volume)
    {
        Managers.Audio.MusicVolume = volume;
    }

    public void ChangeSoundVolume(float value)
    {
        Managers.Audio.SoundVolume = value;
    }
}