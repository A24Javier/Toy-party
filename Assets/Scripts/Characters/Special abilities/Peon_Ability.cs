using UnityEngine;

[CreateAssetMenu(fileName = "Peon_Ability.asset", menuName = ("Abilities/Create Peon_Ability"))]
public class Peon_Ability : AbilityFunction
{
    public override void UseAbility()
    {
        TheTowerSelectPath.Instance.StartSelectPathTower();
    }
}
