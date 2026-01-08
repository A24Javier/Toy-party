using UnityEngine;

[CreateAssetMenu(fileName = "SumMovementFunction.asset", menuName = ("Item functions/Create SumMovementFunction"))]
public class SumMovementFunction : ItemFunction
{
    public int sumMovement;

    public override void UseItem()
    {
        Character character = GameController.instance.GetCharacterOfTurn();
        character.SetExtraStep(sumMovement);

        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
