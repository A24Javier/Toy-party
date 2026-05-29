using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = ("Item/Create Item"))]
public class Item : ScriptableObject
{
    public Sprite itemSpr;

    public LocalizedString ItemLocalName;
    public LocalizedString ItemLocalDescription;
    
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemDescription;
    public string ItemName => _itemName;
    public string ItemDescription => _itemDescription;

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
        ItemLocalDescription.StringChanged -= UpdateDescription;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        _initialized = false;
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        ItemLocalName.StringChanged += UpdateName;
        ItemLocalDescription.StringChanged += UpdateDescription;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        ItemLocalName.RefreshString();
        ItemLocalDescription.RefreshString();
    }

    private void OnLocaleChanged(Locale locale)
    {
        ItemLocalName.RefreshString();
        ItemLocalDescription.RefreshString();
    }

    private void UpdateName(string value)
    {
        _itemName = value;
    }

    private void UpdateDescription(string value)
    {
        _itemDescription = value;
    }
}
