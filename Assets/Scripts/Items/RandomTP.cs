using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomTP.asset", menuName = ("Item functions/Create RandomTP"))]
public class RandomTP : ItemFunction
{
    public override void UseItem()
    {
        Box newBox = Board.instance.GetRandomBox();

        // Hacer que pegue una explosión de particulas o algo

        GameController.instance.GetPlayerOfTurn().transform.position = newBox.transform.position;
        GameController.instance.GetPlayerOfTurn().actualBox = newBox;
        GameController.instance.GetPlayerOfTurn().Move(0);

       // Quitar objeto del inventario del player
       item.inventoryAssociated.DeleteItem(item);
    }
}
