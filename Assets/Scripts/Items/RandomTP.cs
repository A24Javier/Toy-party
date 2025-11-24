using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomTP.asset", menuName = ("Item functions/Create RandomTP"))]
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
