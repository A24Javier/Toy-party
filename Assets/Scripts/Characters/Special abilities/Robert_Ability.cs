using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "Robert_Ability.asset", menuName = ("Abilities/Create Robert_Ability"))]
public class Robert_Ability : AbilityFunction
{
    public int boxesToPass = 5;
    public float durationToBox = 0.2f;

    public override void UseAbility()
    {
        bool payForAbility = true;

#if (UNITY_EDITOR || DEVELOPER_BUILD)
        payForAbility = false;
#endif
        Character robert = GameController.instance.GetCharacterOfTurn();

        if(payForAbility && robert.GetCoins() >= Ability.AbilityPrice || !payForAbility)
        {
            if(payForAbility)
            {
                UIManager.instance.UpdateTextCoins(robert, -Ability.AbilityPrice);
            }

            //robert.GetComponent<Animator>().SetBool("Dash", true);

            Sequence sq = DOTween.Sequence();
            Box[] boxes = new Box[boxesToPass];

            boxes[0] = GetNewNoPathBox(robert.actualBox);

            for (int i = 1; i < boxesToPass; i++)
            {
                boxes[i] = GetNewNoPathBox(boxes[(i - 1)]);
                sq.Append(robert.transform.DOMove(boxes[i].transform.position, durationToBox));
            }

            sq.Play();

            robert.actualBox = boxes[(boxesToPass - 1)];

            sq.OnComplete(() =>
            {
                robert.actualBox.ActivateEffect(robert);
            });
        }
    }

    private Box GetNewNoPathBox(Box actualBox)
    {
        Box newBox = actualBox;
        int attemps = 0;

        do
        {
            newBox = newBox.GetNewBox(0);
        }while(newBox.type == BoxType.Path && attemps < 100);

        return newBox;
    }
}
