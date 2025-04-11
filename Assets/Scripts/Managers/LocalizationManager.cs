using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour, IGameManager
{
    [SerializeField] private LocalizationData[] languages;
    [SerializeField] private AudioClip _clipClickButton;

    private Dictionary<string, string> _localizedText; // текущий язык
    private string _currentLanguage = "en"; // по умолчанию 

    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        Debug.Log("Localization manager starting...");

        status = ManagerStatus.Started;
    }

    private void Awake()
    {
        LoadLanguage(_currentLanguage); // загружаем по умолчанию
    }

    public void SetLanguage(string language)
    {
        Managers.Audio.PlaySound(_clipClickButton);
        LoadLanguage(language);
    }


    // новый язык
    public void LoadLanguage(string language)
    {
        Debug.Log($"Попытка загрузки языка: {language} (LoadLanguage)");
        if (languages == null || languages.Length == 0)
        {
            Debug.LogError("Ошибка: В массиве languages нет данных! Проверьте настройки в Unity.");
            return;
        }

        Debug.Log($"Попытка загрузки языка: {language}");

        // Логирование всех доступных языков
        Debug.Log("Доступные языки:");
        foreach (LocalizationData data in languages)
        {
            Debug.Log($"- {data.language}");
        }

        foreach (LocalizationData data in languages)
        {
            Debug.Log($"Проверяем локализацию: {data.language}");

            if (data.language == language)
            {
                _localizedText = data.GetDictionary();
                _currentLanguage = language;
                UpdateLocalizedTexts();
                Debug.Log($"Язык успешно загружен: {language}");

                return;
            }
        }
        Debug.LogWarning($"Локализация для языка '{language}' не найдена!");
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
            Debug.LogError("Localization dictionary is null! Проверьте, загружена ли локализация.");
            return key;
        }

        if (_localizedText.ContainsKey(key))
        {
            return _localizedText[key];

        }
       
        Debug.LogWarning($"Ключ '{key}' не найден в локализации!");
        return key; // Если ключ не найден, возвращаем сам ключ
    }
}
