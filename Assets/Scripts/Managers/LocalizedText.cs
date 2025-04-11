using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;

    private Text _textComponent;
    private TextMeshProUGUI _textMeshProComponent; // ��� TextMeshPro

    private void OnEnable()
    {
        StartCoroutine(WaitForLocalization());
    }

    private IEnumerator WaitForLocalization()
    {
        while (Managers.Localization == null)
        {
            yield return null; // ��� 1 ����, ���� LocalizationManager �� ����������������
        }

        _textComponent = GetComponent<Text>();      // ����������� UI Text
        _textMeshProComponent = GetComponent<TextMeshProUGUI>();  // TextMeshPro
        UpdateText();
    }


    public void UpdateText()
    {
        if (_textMeshProComponent == null)
        {
            _textMeshProComponent = GetComponent<TextMeshProUGUI>();
        }

        if (_textComponent != null)
        {
            _textComponent.text = Managers.Localization.GetLocalizedText(key);
        }
        else if (_textMeshProComponent != null)
        {
            string newText = Managers.Localization.GetLocalizedText(key);
            _textMeshProComponent.text = Managers.Localization.GetLocalizedText(key);
        }
        else
        {
            Debug.LogWarning("�� ������ ��������� Text ��� TextMeshProUGUI �� ������� " + gameObject.name);
        }
    }
}
