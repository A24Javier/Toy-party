using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = ("Item/Create Item"))]
public class Item : ScriptableObject
{
    public Sprite itemSpr;
    public LocalizedString ItemLocalName;
    public string itemName => ItemLocalName.GetLocalizedString();
    public int itemPrice;
    public ItemFunction itemFunction;
    public Inventory inventoryAssociated;
}
