using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/LocalizationData")]

public class LocalizationData : ScriptableObject
{
    public string language;

    public LocalizationEntry[] entries; // ������ ���� ���������

    public Dictionary<string, string> GetDictionary()
    {
        Dictionary<string, string> dictionary = new();

        if (entries == null || entries.Length == 0)
        {
            Debug.LogError($"������ � LocalizationData: � ����������� '{language}' ��� �������!");
            return dictionary;
        }

        foreach (var entry in entries)
        {
            dictionary[entry.key] = entry.value;
        }
        Debug.Log($"����������� '{language}' ������� ���������. ���������� �����: {dictionary.Count}");
        return dictionary;
    }

}