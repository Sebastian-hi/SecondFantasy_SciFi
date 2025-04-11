using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour, IGameManager
{
    [SerializeField] private LocalizationData[] languages;
    [SerializeField] private AudioClip _clipClickButton;

    private Dictionary<string, string> _localizedText; // ������� ����
    private string _currentLanguage = "en"; // �� ��������� 

    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        Debug.Log("Localization manager starting...");

        status = ManagerStatus.Started;
    }

    private void Awake()
    {
        LoadLanguage(_currentLanguage); // ��������� �� ���������
    }

    public void SetLanguage(string language)
    {
        Managers.Audio.PlaySound(_clipClickButton);
        LoadLanguage(language);
    }


    // ����� ����
    public void LoadLanguage(string language)
    {
        Debug.Log($"������� �������� �����: {language} (LoadLanguage)");
        if (languages == null || languages.Length == 0)
        {
            Debug.LogError("������: � ������� languages ��� ������! ��������� ��������� � Unity.");
            return;
        }

        Debug.Log($"������� �������� �����: {language}");

        // ����������� ���� ��������� ������
        Debug.Log("��������� �����:");
        foreach (LocalizationData data in languages)
        {
            Debug.Log($"- {data.language}");
        }

        foreach (LocalizationData data in languages)
        {
            Debug.Log($"��������� �����������: {data.language}");

            if (data.language == language)
            {
                _localizedText = data.GetDictionary();
                _currentLanguage = language;
                UpdateLocalizedTexts();
                Debug.Log($"���� ������� ��������: {language}");

                return;
            }
        }
        Debug.LogWarning($"����������� ��� ����� '{language}' �� �������!");
    }

    private void UpdateLocalizedTexts()
    {
        LocalizedText[] texts = FindObjectsOfType<LocalizedText>();

        foreach (LocalizedText text in texts)
        {
            text.UpdateText();
        }
    }

    public string GetLocalizedText(string key)
    {
        if (_localizedText == null)
        {
            Debug.LogError("Localization dictionary is null! ���������, ��������� �� �����������.");
            return key;
        }

        if (_localizedText.ContainsKey(key))
        {
            return _localizedText[key];

        }
       
        Debug.LogWarning($"���� '{key}' �� ������ � �����������!");
        return key; // ���� ���� �� ������, ���������� ��� ����
    }
}
