using UnityEngine;

[CreateAssetMenu(fileName = "MsPage_Ability.asset", menuName = ("Abilities/Create MsPage_Ability"))]
public class MsPage_Ability : AbilityFunction
{
    public override void UseAbility()
    {
        float rand = Random.Range(0, 101);

        if(rand >= 0 && rand <= 47.5f) // Dibuja y gana 4 monedas
        {

        }
        else if(rand > 47.5f && rand <= 95) // Dibuja una bomba que hace retroceder a un rival
        {

        }
        else // Dibuja portal que te lleva a una casilla a tu elección
        {

        }
    }
}
