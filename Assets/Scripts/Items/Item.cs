using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = ("Item/Create Item"))]
public class Item : ScriptableObject
{
    public Sprite itemSpr;

    public LocalizedString ItemLocalName;
    
    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    public int itemPrice;
    public ItemFunction itemFunction;
    public Inventory inventoryAssociated;

    private bool _initialized = false;

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        ItemLocalName.StringChanged -= UpdateName;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        _initialized = false;
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        ItemLocalName.StringChanged += UpdateName;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        ItemLocalName.RefreshString();
    }

    private void OnLocaleChanged(Locale locale)
    {
        ItemLocalName.RefreshString();
    }

    private void UpdateName(string value)
    {
        _itemName = value;
    }
}
