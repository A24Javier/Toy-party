using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public enum BoxType
    {
        Normal,
        Coin,
        Trap,
        Path,
        Star
    }

    [SerializeField] private BoxType type;
    [SerializeField] private int rewardCoins = 3;
    [SerializeField] private int starPrize = 5;
    [SerializeField] private List<Box> possiblesBoxes;

    public void ActiveEffect(Player player)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log(player.GetPlayerName() + " cayo en casilla monedas");
                StartCoroutine(UIManager.instance.UpdateTextCoins(rewardCoins, player));
                break;
            case BoxType.Trap:
                Debug.Log(player.GetPlayerName() + " cayo en casilla trampa");
                break;
            case BoxType.Path:
                break;
            case BoxType.Star:
                Debug.Log(player.GetPlayerName() + " cayo en casilla estrella");
                break;
        }
    }

    public Transform GetNewBoxTransf()
    {
        return possiblesBoxes[0].transform;
    }

    public int PossiblesBoxesCount()
    {
        return possiblesBoxes.Count;
    }

    public Box GetNewBox(int i) { return possiblesBoxes[i]; }


    public Transform GetThisBoxTransf() { return this.transform; }

    public Transform GetBoxTransf(int box)
    {
        return possiblesBoxes[box].transform;
    }
}
