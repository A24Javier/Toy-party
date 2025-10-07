using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private List<Box> possiblesBoxes;
    [SerializeField] private UnityEvent doBoxAction;

    public void ActiveEffect(Player player)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log("Player cayo en casilla monedas");
                UIManager.instance.SetActualPlayer(player);
                StartCoroutine(UIManager.instance.UpdateTextCoins(rewardCoins));
                break;
            case BoxType.Trap:
                Debug.Log("Player cayo en casilla trampa");
                break;
            case BoxType.Path:
                break;
            case BoxType.Star:
                Debug.Log("Player cayo en casilla estrella");
                break;
        }

        doBoxAction.Invoke();
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
