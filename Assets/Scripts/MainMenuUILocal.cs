using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUILocal : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playText;
    [SerializeField] TextMeshProUGUI _settingsText;

    private void Update()
    {
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        _playText.text = Managers.Localization.GetLocalizedText("Start Game:");
        _settingsText.text = Managers.Localization.GetLocalizedText("Settings:");
    }

}
