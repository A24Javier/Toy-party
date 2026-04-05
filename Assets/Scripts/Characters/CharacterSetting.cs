using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "CharacterSetting.asset", menuName = ("Data/Create Character setting"))]
public class CharacterSetting : ScriptableObject
{
    public GameObject characterPrefab;
    public Ability characterAbility;

    [SerializeField] private LocalizedString CharNameTraduction;
    [SerializeField] private LocalizedString CharDescrTraduction;

    [SerializeField] private string _characterName;
    [SerializeField] private string _characterDescription;

    public string CharacterName => _characterName;
    public string CharDescription => _characterDescription;

    public Sprite characterSprite;

    private bool _initialized = false;

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        CharNameTraduction.StringChanged += UpdateName;
        CharDescrTraduction.StringChanged += UpdateDescription;

        CharNameTraduction.RefreshString();
        CharDescrTraduction.RefreshString();
    }

    private void UpdateName(string value)
    {
        _characterName = value;
    }

    private void UpdateDescription(string value)
    {
        _characterDescription = value;
    }
}
