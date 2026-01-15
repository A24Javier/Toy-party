using UnityEngine;

[CreateAssetMenu(fileName = "TP_OtherPlayer.asset", menuName = ("Item functions/Create TP_OtherPlayer"))]
public class TP_OtherPlayer : ItemFunction
{
    public override void UseItem()
    {
        UIManager.instance.ConfigureSelectPlayer(GameController.instance.GetCharacterOfTurn(), "TP_OtherPlayer", 0);

        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
