using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFunction.asset", menuName = ("Item functions/Create Item function"))]
public abstract class ItemFunction : ScriptableObject
{
    public Item item;
    public RuntimeAnimatorController headAnimator;
    public abstract void UseItem();

    public virtual void ItemHeadFunction()
    {
        GameObject itemHead = new GameObject("Item Head");
        itemHead.AddComponent<SpriteRenderer>().sprite = item.itemSpr;
        itemHead.AddComponent<Animator>();

        GameObject itemHeadGO = Instantiate(itemHead);

        itemHeadGO.transform.parent = GameController.instance.GetCharacterOfTurn().transform;
        itemHeadGO.transform.localPosition = Vector3.zero;
        itemHeadGO.transform.LookAt(Camera.main.transform);

        itemHeadGO.GetComponent<Animator>().runtimeAnimatorController = headAnimator;
        Destroy(itemHeadGO, 1.2f);
    }
}
