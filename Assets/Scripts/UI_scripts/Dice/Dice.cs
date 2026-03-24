using UnityEngine;

[CreateAssetMenu(menuName = ("Dice/Create Dice"))]
public class Dice : ScriptableObject
{
    [Header("Dice rendering")]
    public GameObject DiceObject;

    [Header("Dice values")]
    public float ChangeNumSpeed;
    public int MinNumber;
    public int MaxNumber;

    [Header("Dice rotation")]
    public float RotSpeedX;
    public float RotSpeedY;

    [Header("Dice function")]
    public ItemFunction ItemFunction;
}
