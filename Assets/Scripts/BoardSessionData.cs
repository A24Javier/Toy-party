using System;

[System.Serializable]
public class BoardSessionData
{
    public int actualTurn;
    public int actualRound;
    public int[] idOrder;
    public bool[] isPlayerOrder;
}