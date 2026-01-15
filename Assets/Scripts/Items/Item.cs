using UnityEngine;

[CreateAssetMenu(menuName = ("Item/Create Item"))]
public class Item : ScriptableObject
{
    public Sprite itemSpr;
    public string itemName;
    public ItemFunction itemFunction;
    public Inventory inventoryAssociated;
}
