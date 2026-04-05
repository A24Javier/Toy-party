using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectCharController : MonoBehaviour
{
    [SerializeField] private TMP_Text _charNameTMP;
    [SerializeField] private TMP_Text _charDescrTMP;

    [SerializeField] private TMP_Text _abilityNameTMP;
    [SerializeField] private TMP_Text _abilityDescrTMP;
    [SerializeField] private Image _abilityImage;

    [SerializeField] private CharacterSetting[] _characters;

    private int _charIndex = 0;

    void Start()
    {
        SetTexts();
    }

    public void SelectChar(int charIndex)
    {
        _charIndex = charIndex;
        SetTexts();
    }

    private void SetTexts()
    {
        _charNameTMP.text = _characters[_charIndex].CharacterName;
        _charDescrTMP.text = _characters[_charIndex].CharDescription;

        _abilityNameTMP.text = _characters[_charIndex].characterAbility.AbilityName;
        _abilityDescrTMP.text = _characters[_charIndex].characterAbility.Description;
        _abilityImage.sprite = _characters[_charIndex].characterAbility.AbilitySprite;
    }
}
