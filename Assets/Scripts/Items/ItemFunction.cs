using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFunction.asset", menuName = ("Item functions/Create Item function"))]
public abstract class ItemFunction : ScriptableObject
{
    public Item item;
    public abstract void UseItem();
}
