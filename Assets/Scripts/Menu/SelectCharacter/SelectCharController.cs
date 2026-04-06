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

    [SerializeField] private GameObject[] _charactersPreview;
    [SerializeField] private Transform _fatherPreview;
    [SerializeField] private Transform _fatherHide;

    void Start()
    {
        ShowChar();
        SetTexts();
    }

    public void SelectChar(int charIndex)
    {
        if (_charIndex == charIndex)
            return;

        _charIndex = charIndex;

        ShowChar();
        SetTexts();
    }

    private void ShowChar()
    {
        if(_fatherPreview.childCount > 0)
        {
            GameObject characterGO = _fatherPreview.GetChild(0).gameObject;
            characterGO.transform.parent = _fatherHide;
            characterGO.transform.localPosition = Vector3.zero;
        }

        Transform charTransf = _charactersPreview[_charIndex].transform;
        charTransf.parent = _fatherPreview;
        charTransf.localPosition = Vector3.zero;
        charTransf.localRotation = Quaternion.Euler(Vector3.zero);

    }

    private void SetTexts()
    {
        _charNameTMP.text = _characters[_charIndex].CharacterName;
        _charDescrTMP.text = _characters[_charIndex].CharDescription;

        _abilityNameTMP.text = _characters[_charIndex].characterAbility.AbilityName;
        _abilityDescrTMP.text = _characters[_charIndex].characterAbility.Description;
        _abilityImage.sprite = _characters[_charIndex].characterAbility.AbilitySprite;
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("PlayerSelected", _charIndex);
    }
}
