using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private List<Locale> locales;

    private int _currentIndex;

    private void Start()
    {
        SetIndexOfCurrentLocale();
    }

    public void ChangeLanguage()
    {
        _currentIndex = (_currentIndex + 1) % locales.Count;
        LocalizationSettings.Instance.SetSelectedLocale(locales[_currentIndex]);
    }

    private async void SetIndexOfCurrentLocale()
    {
        var operation = LocalizationSettings.Instance.GetSelectedLocaleAsync();
        var currentLocale = await operation.Task;
        _currentIndex = locales.IndexOf(currentLocale);
    }
}