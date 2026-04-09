using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerSillaaSevilla : MonoBehaviour
{
    int id;
    int PosEliminated;

    public PlayerSillaaSevilla (int id, int pos)
    {
        this.id = id;
        this.PosEliminated = pos;
    }

    public  int VerId()
    {
        return id;
    }

    public int VerPos()
    {
        return PosEliminated;
    }

    public void Setid(int id)
    {
        this.id = id;
    }
    public void SetPos(int pos)
    {
        this.PosEliminated = pos;
    }
}
