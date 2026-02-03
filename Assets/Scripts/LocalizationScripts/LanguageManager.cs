using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro ; 
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    private bool active = false;
    public TMP_Dropdown languageDropdown;


    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocalKey", 0);
        languageDropdown.value = ID;
        ChangeLanguage(ID);
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
    }
    public void ChangeLanguage(int localeID)
    {
        if (active == true)
        
          return;
        
        StartCoroutine(SetLocale(localeID));
        
    }

    IEnumerator SetLocale(int _localeID)
    {
        active= true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocalKey", _localeID);
        active= false;
    }
  
}
