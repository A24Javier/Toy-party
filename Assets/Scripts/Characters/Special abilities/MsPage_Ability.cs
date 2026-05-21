using UnityEngine;

[CreateAssetMenu(fileName = "MsPage_Ability.asset", menuName = ("Abilities/Create MsPage_Ability"))]
public class MsPage_Ability : AbilityFunction
{
    public override void UseAbility()
    {
        bool payForAbility = true;

#if (UNITY_EDITOR || DEVELOPER_BUILD)
        payForAbility = false;
#endif
        Character mspage = GameController.instance.GetCharacterOfTurn();

        if (payForAbility && mspage.GetCoins() >= Ability.AbilityPrice || !payForAbility)
        {
            mspage.usingAbility = true;

            if (payForAbility)
            {
                UIManager.instance.UpdateTextCoins(mspage, -Ability.AbilityPrice);
            }

            float rand = Random.Range(0, 101);

            MsPage_AbilityCoroutine.Instance?.StartCoroutineAbility(mspage);
            
            /*
            if (rand >= 0 && rand <= 47.5f) // Dibuja y gana 4 monedas
            {
                UIManager.instance.FunctionUpdateTextCoins(mspage, 4);
                mspage.usingAbility = false;
            }
            else if (rand > 47.5f && rand <= 95) // Dibuja una bomba que hace retroceder a un rival
            {
                UIManager.instance.ConfigureSelectPlayer(mspage, "Bomb", 3);
            }
            else // Se crea un portal y se usa al instante
            {
                UIManager.instance.ConfigureSelectPlayer(mspage, "TP_OtherPlayer");
            }
            */
        }
        
    }
}
