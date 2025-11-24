using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Dice> dices;
    public List<Item> items;

    public int totalObjLoaded;
    public int maxObjects;

    public Inventory()
    {
        dices = new List<Dice>();
        items = new List<Item>();

        totalObjLoaded = 0;
        maxObjects = 3;
    }

    public virtual Item GetItem(int index)
    {
        return items[index];
    }

    public virtual void AddItem(Item newItem)
    {
        if((items.Count + dices.Count) < maxObjects)
        {
            items.Add(newItem);
            UIManager.instance.AddItem(newItem);
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

    public virtual Dice GetDice(int index)
    {
        return dices[index];
    }

    public virtual void AddDice(Dice newDice)
    {
        if((items.Count + dices.Count) < maxObjects)
        {
            dices.Add(newDice);
            totalObjLoaded++;
        }
        else
        {
            Debug.Log("No puedes llevar más objetos");
        }
    }

    public virtual void DeleteDice(Dice diceDelete)
    {
        dices.Remove(diceDelete);
        totalObjLoaded--;
    }
}
