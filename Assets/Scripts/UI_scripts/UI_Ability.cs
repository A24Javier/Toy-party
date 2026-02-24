using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour
{
    [SerializeField] private Image _imageBackground;
    [SerializeField] private Image _abilityImage;
    [SerializeField] private Button _abilityButton;

    public static UI_Ability Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetAbilityButton(Ability ability)
    {
        _abilityImage.sprite = ability.AbilitySprite;
        _imageBackground.color = ability.BackgroundColor;

        _abilityButton.onClick.RemoveAllListeners();
        _abilityButton.onClick.AddListener(delegate { ability.AbilityFunction.UseAbility(); });
    }
}
