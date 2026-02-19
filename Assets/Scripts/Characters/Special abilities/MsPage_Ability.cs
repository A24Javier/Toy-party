using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MsPage_Ability.asset", menuName = ("Abilities/Create MsPage_Ability"))]
public class MsPage_Ability : AbilityFunction
{
    public override void UseAbility()
    {
        float rand = Random.Range(0, 101);
        Character character = GameController.instance.GetCharacterOfTurn();
        // Hacer animación de pintar

        if(rand >= 0 && rand <= 47.5f) // Dibuja y gana 4 monedas
        {
            UIManager.instance.FunctionUpdateTextCoins(character, 4);
        }
        else if(rand > 47.5f && rand <= 95) // Dibuja una bomba que hace retroceder a un rival
        {
            UIManager.instance.ConfigureSelectPlayer(character, "Bomb", 3);
        }
        else // Se crea un portal y se usa al instante
        {
            UIManager.instance.ConfigureSelectPlayer(character, "TP_OtherPlayer");
        }
    }
}
