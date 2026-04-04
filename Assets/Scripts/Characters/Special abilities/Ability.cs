using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = ("Abilities/Create Ability"))]
public class Ability : ScriptableObject
{
    public LocalizedString AbilityLocalName;
    [SerializeField] private string _abilityName;
    public string AbilityName => _abilityName;

    public LocalizedString AbilityDescrLocal;
    [SerializeField] private string _abilityDescription;
    public string Description => _abilityDescription;

    public Sprite AbilitySprite;
    public Color BackgroundColor;

    public bool PayForUse;
    public int AbilityPrice;

    public Buff[] PassiveBuffs;

    public AbilityFunction AbilityFunction;

    private bool _initialized = false;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        AbilityLocalName.StringChanged += UpdateName;
        AbilityDescrLocal.StringChanged += UpdateDescription;

        AbilityLocalName.RefreshString();
        AbilityDescrLocal.RefreshString();
    }

    private void UpdateName(string value)
    {
        _abilityName = value;
    }

    private void UpdateDescription(string value)
    {
        _abilityDescription = value;
    }
}
