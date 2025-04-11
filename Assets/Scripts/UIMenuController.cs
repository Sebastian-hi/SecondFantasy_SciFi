using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _settingsCamera;
    [SerializeField] private AudioClip _clipClickButton;


    private void Start()
    {
        _mainCamera.SetActive(true);
        _settingsCamera.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Managers.Audio.PlaySound(_clipClickButton);
        _mainCamera.SetActive(true);
        _settingsCamera.SetActive(false);
    }

    public void GoToSettings()
    {
        Managers.Audio.PlaySound(_clipClickButton);
        _mainCamera.SetActive(false);
        _settingsCamera.SetActive(true);
    }

    public void ExitTheGame()
    {
        Debug.Log("Выходим из игры");
        Managers.Audio.PlaySound(_clipClickButton);
        Application.Quit();
    }
}