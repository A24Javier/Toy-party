using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Collections;

[CreateAssetMenu(fileName = "Princess_Ability.asset", menuName = ("Abilities/Create Princess_Ability"))]
public class Princess_Ability : AbilityFunction
{
    [SerializeField] private int _totalTraps;

    public override void UseAbility()
    {
        bool payForAbility = true;

#if (UNITY_EDITOR || DEVELOPER_BUILD)
        payForAbility = false;
#endif
        Character princess = GameController.instance.GetCharacterOfTurn();

        if (payForAbility && princess.GetCoins() >= Ability.AbilityPrice || !payForAbility)
        {
            if (payForAbility)
            {
                UIManager.instance.UpdateTextCoins(princess, -Ability.AbilityPrice);
            }

            Board board = FindAnyObjectByType<Board>();

            // Obtenemos las casillas aleatorias donde se instanciarán las trampas
            HashSet<Box> aleatoryBoxes = new HashSet<Box>();

            for (int i = 0; i < _totalTraps; i++)
            {
                Box aleatoryBox = null;
                int attemps = 0;
                do
                {
                    do
                    {
                        aleatoryBox = board.GetRandomBox();
                        attemps++;
                    } while (aleatoryBox.type != BoxType.Coin && aleatoryBox.type != BoxType.Normal && attemps < 100);
                } while (!aleatoryBoxes.Add(aleatoryBox));

            }

            PrincessTrapCoroutine.Instance.StartTrapCoroutine(aleatoryBoxes);
        }
        
    }
}
