using UnityEngine;

[CreateAssetMenu(fileName = "StealCoins.asset", menuName = ("Item functions/Create StealCoins"))]
public class StealCoins : ItemFunction
{
    public int coinsToSteal = 3;

    public override void UseItem()
    {
        ItemHeadFunction();

        UIManager.instance.ConfigureSelectPlayer(GameController.instance.GetCharacterOfTurn(), "StealCoins", coinsToSteal);

        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
