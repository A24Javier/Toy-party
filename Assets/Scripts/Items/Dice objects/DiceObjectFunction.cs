using UnityEngine;

[CreateAssetMenu(fileName = "DiceObjectFunction.asset", menuName = ("Item functions/Create DiceObject"))]
public class DiceObjectFunction : ItemFunction
{
    public Dice Dice;

    public override void UseItem()
    {
        // Cambiamos el dado que se usara al tirar el dado
        GameController.instance.DiceToUse = Dice;

        // Cambiamos el sprite del dado en la UI
        UIManager.instance.ChangeDiceSprite(item.itemSpr);

        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
