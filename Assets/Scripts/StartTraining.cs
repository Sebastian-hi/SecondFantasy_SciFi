using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTraining : MonoBehaviour
{
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _settingsCamera;
    [SerializeField] private GameObject _trainingCamera1;
    [SerializeField] private GameObject _trainingCamera2;
    [SerializeField] private GameObject _trainingCamera3;
    [SerializeField] private AudioClip _clipClickButton;
    [Space]
    [SerializeField] private GameObject _background1;
    [SerializeField] private GameObject _background2;
    [SerializeField] private GameObject _background3Terminal;
    [SerializeField] private GameObject _background4Terminal;
    [SerializeField] private GameObject _background5Terminal;
    [SerializeField] private GameObject _background6End;
    //[SerializeField] private GameObject _trainingRoom1;
    //[SerializeField] private GameObject _trainingRoom2;


    private void Start()
    {
        _trainingCamera1.SetActive(false);
        _trainingCamera2.SetActive(false);
        _trainingCamera3.SetActive(false);

        _background1.SetActive(false);
        _background2.SetActive(false);
        _background3Terminal.SetActive(false);
        _background4Terminal.SetActive(false);
        _background5Terminal.SetActive(false);
        _background6End.SetActive(false);
    }

    public void GoTraining()
    {
        Managers.Audio.PlaySound(_clipClickButton);
        _settingsCamera.SetActive(false);
        _mainCamera.SetActive(false);

        _trainingCamera1.SetActive(true);
        _background1.SetActive(true);
        _background2.SetActive(true);
    }

    public void GoTrainingTerminal()
    {
        Managers.Audio.PlaySound(_clipClickButton);
        _trainingCamera1.SetActive(false);
        _background1.SetActive(false);
        _background2.SetActive(false);

        _trainingCamera2.SetActive(true);
        _background3Terminal.SetActive(true);
        _background4Terminal.SetActive(true);
        _background5Terminal.SetActive(true);
    }

    public void EndTraining()
    {
        Managers.Audio.PlaySound(_clipClickButton);
        _background3Terminal.SetActive(false);
        _background4Terminal.SetActive(false);
        _background5Terminal.SetActive(false);
        _trainingCamera2.SetActive(false);
        _trainingCamera3.SetActive(true);

        _background6End.SetActive(true);
    }

}
