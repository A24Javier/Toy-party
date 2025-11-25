using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items;

    public int totalObjLoaded;
    public int maxObjects;

    public Inventory()
    {
        items = new List<Item>();

        totalObjLoaded = 0;
        maxObjects = 3;
    }

    public virtual int GetTotalObjLoaded()
    {
        return totalObjLoaded;
    }

    public virtual int GetMaxObjects()
    {
        return maxObjects;
    }

    public virtual Item GetItem(int index)
    {
        return items[index];
    }

    public virtual void AddItem(Item newItem)
    {
        if(items.Count < maxObjects)
        {
            items.Add(newItem);
            //UIManager.instance.AddItem(newItem);
            newItem.inventoryAssociated = this;
            newItem.itemFunction.item = newItem;
            totalObjLoaded++;
        }
        else
        {
            Debug.Log("No puedes llevar más objetos");
        }
    }

    public virtual void DeleteItem(Item itemDelete)
    {
        items.Remove(itemDelete);
        totalObjLoaded--;
    }
}
