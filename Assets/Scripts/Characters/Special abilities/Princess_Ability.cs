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
        Board board = FindAnyObjectByType<Board>();

        // Obtenemos las casillas aleatorias donde se instanciarán las trampas
        HashSet<Box> aleatoryBoxes = new HashSet<Box>();

        for(int i = 0; i < _totalTraps; i++)
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
                

            //aleatoryBoxes[i] = aleatoryBox;
        }

        // Plantamos las trampas en las casillas
        /*for(int i = 0; i < aleatoryBoxes.Length; i++)
        {
            // Plantamos la trampa en la casilla
            GameObject trap = Instantiate(_prefabTrap, aleatoryBoxes[i].transform.position + (Vector3.up * 0.1f), Quaternion.identity);
         
            // Hacemos saber a la casilla que tiene una trampa
            aleatoryBoxes[i].SetTrap(trap);
        }*/

        PrincessTrapCoroutine.Instance.StartTrapCoroutine(aleatoryBoxes);
    }
}
