using UnityEngine;

[CreateAssetMenu(menuName = ("Item/Create Item"))]
public class Item : ScriptableObject
{
    public Sprite itemSpr;
    public GameObject itemGO;
    public ItemFunction itemFunction;
}
