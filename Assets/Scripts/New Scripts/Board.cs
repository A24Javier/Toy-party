using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Box[] boxes;

    public Box GetCasilla(int index)
    {
        if (index < 0 || index >= boxes.Length) return null;
        return boxes[index];
    }
}
