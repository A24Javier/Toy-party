using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> items;

    public int totalObjLoaded;
    public int maxObjects;

    public Inventory()
    {
        items = new List<Item>();

        maxObjects = 3;
    }

    public virtual int GetTotalObjLoaded()
    {
        return items.Count;
    }

    public virtual int GetMaxObjects()
    {
        return maxObjects;
    }

    public virtual bool IsFull()
    {
        return items.Count >= maxObjects;
    }

    public virtual Item GetItem(int index)
    {
        return items[index];
    }

    public virtual void AddItem(Item _item)
    {
        if(items.Count < maxObjects)
        {
            items.Add(_item);
            //UIManager.instance.AddItem(newItem);
            _item.inventoryAssociated = this;
            _item.itemFunction.item = _item;
        }
        else
        {
            Debug.Log("No puedes llevar más objetos");
        }

        Debug.Log($"Total items: {items.Count}");
    }

    public virtual void DeleteItem(Item itemDelete)
    {
        items.Remove(itemDelete);
    }
}
