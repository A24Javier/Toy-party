using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Dice/Create Dice"))]
public class Dice : ScriptableObject
{
    [Header("Dice rendering")]
    public Mesh diceMesh;
    public Material diceMat;

    [Header("Dice values")]
    public float changeNumSpeed;
    public int minNumber;
    public int maxNumber;

    [Header("Dice function")]
    public ItemFunction itemFunction;
}
