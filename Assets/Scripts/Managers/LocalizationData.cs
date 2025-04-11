using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/LocalizationData")]

public class LocalizationData : ScriptableObject
{
    public string language;

    public LocalizationEntry[] entries; // массив всех переводов

    public Dictionary<string, string> GetDictionary()
    {
        Dictionary<string, string> dictionary = new();

        if (entries == null || entries.Length == 0)
        {
            Debug.LogError($"Ошибка в LocalizationData: В локализации '{language}' нет записей!");
            return dictionary;
        }

        foreach (var entry in entries)
        {
            dictionary[entry.key] = entry.value;
        }
        Debug.Log($"Локализация '{language}' успешно загружена. Количество строк: {dictionary.Count}");
        return dictionary;
    }

}