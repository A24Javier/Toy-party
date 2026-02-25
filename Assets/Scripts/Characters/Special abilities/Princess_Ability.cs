using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Princess_Ability.asset", menuName = ("Abilities/Create Princess_Ability"))]
public class Princess_Ability : AbilityFunction
{
    [SerializeField] private GameObject _prefabTrap;
    [SerializeField] private int _totalTraps;

    public override void UseAbility()
    {
        Board board = FindAnyObjectByType<Board>();

        // Obtenemos las casillas aleatorias donde se instanciarán las trampas
        Box[] aleatoryBoxes = new Box[_totalTraps];

        for(int i = 0; i < _totalTraps; i++)
        {
            Box aleatoryBox = null;
            int attemps = 0;

            do
            {
                aleatoryBox = board.GetRandomBox();
                attemps++;
            } while ((aleatoryBox.type != BoxType.Coin || aleatoryBox.type != BoxType.Normal) || attemps < 100);

            aleatoryBoxes[i] = aleatoryBox;
        }

        // Plantamos las trampas en las casillas
        for(int i = 0; i < aleatoryBoxes.Length; i++)
        {
            // Plantamos la trampa en la casilla
            GameObject trap = Instantiate(_prefabTrap, aleatoryBoxes[i].transform.position, Quaternion.identity);

            // Hacemos saber a la casilla que tiene una trampa

        }
    }
}
