using UnityEngine;

[CreateAssetMenu(fileName = "AbsMovementFunction.asset", menuName = ("Item functions/Create AbsMovementFunction"))]
public class AbsPlayerMovement : ItemFunction
{
    public int movementAbs;

    public override void UseItem()
    {
        UIManager.instance.ConfigureSelectPlayer(GameController.instance.GetCharacterOfTurn(), "AbstractMovement", movementAbs);

        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
