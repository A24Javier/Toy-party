using UnityEngine;

[CreateAssetMenu(fileName = "Peon_Ability.asset", menuName = ("Abilities/Create Peon_Ability"))]
public class Peon_Ability : AbilityFunction
{
    public override void UseAbility()
    {
        bool payForAbility = true;

#if (UNITY_EDITOR || DEVELOPER_BUILD)
        payForAbility = false;
#endif
        Character peon = GameController.instance.GetCharacterOfTurn();

        if (payForAbility && peon.GetCoins() >= Ability.AbilityPrice || !payForAbility)
        {
            if (payForAbility)
            {
                UIManager.instance.UpdateTextCoins(peon, -Ability.AbilityPrice);
            }

            TheTowerSelectPath.Instance.StartSelectPathTower();
        }
    }
}
