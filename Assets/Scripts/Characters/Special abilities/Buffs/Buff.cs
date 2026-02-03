using UnityEngine;

[CreateAssetMenu(menuName = ("Buff/Create Buff"))]
public class Buff : ScriptableObject
{
    public string buffName;

    public int extraCoinsBoxes;
    public int lessLoseInBoxes;

    public int positivePercentageBuff; // Queda por implementar en todas las deciciones

    public int extraPriceStore;
}
