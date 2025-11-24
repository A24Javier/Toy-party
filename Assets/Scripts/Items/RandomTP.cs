using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTP : ItemFunction
{
    public override void UseItem()
    {
        Transform newPos = Board.instance.GetRandomBox().transform;

        // Hacer que pegue una explosión de particulas o algo

        GameController.instance.GetPlayerOfTurn().transform.position = newPos.position;
        GameController.instance.FinishTurn();
    }
}
