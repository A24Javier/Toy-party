using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;

    public Box[] boxes;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Box GetCasilla(int index)
    {
        if (index < 0 || index >= boxes.Length) return null;
        return boxes[index];
    }

    public Box GetRandomBox()
    {
        int rand = Random.Range(0, boxes.Length);
        return boxes[rand];
    }
}
