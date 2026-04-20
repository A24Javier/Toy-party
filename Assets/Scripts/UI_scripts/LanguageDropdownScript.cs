using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageDropdownScript : MonoBehaviour
{
    private Locale[] _locales;
    private string[] _languagesNames;
    private TMP_Dropdown _dropdown;

    void Start()
    {
        // Averiguamos los nombres de los locales que tiene el proyecto
        _locales = LocalizationSettings.AvailableLocales.Locales.ToArray();
        _languagesNames = new string[_locales.Length];

        for(int i = 0; i < _locales.Length; i++)
        {
            _languagesNames[i] = _locales[i].LocaleName;
        }

        // Obtenemos el componente del GO
        _dropdown = GetComponent<TMP_Dropdown>();

        // Seteamos el nombre de las opciones en el dropdown
        _dropdown.AddOptions(_languagesNames.ToList());

        // Hacemos que al cambiar el dropdown active esta funcion
        _dropdown.onValueChanged.AddListener(LanguageChange);

        // Averiguamos que idioma tiene activado ahora y lo reflejamos
        // en el dropdown
        for(int i = 0; i < _locales.Length; i++)
        {
            if (_locales[i] == LocalizationSettings.SelectedLocale)
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void LanguageChange(int value)
    {
        // Cambiar idioma
        LocalizationSettings.SelectedLocale = _locales[value];
    }
}
