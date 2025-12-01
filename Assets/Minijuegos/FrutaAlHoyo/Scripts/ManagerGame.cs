using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGame : MonoBehaviour
{
    public static ManagerGame Instance;

    int[] FrutaCogida;

    public void PuntsCesta1(int value)
    {
        FrutaCogida[0] = value;
    }
    public void PuntsCesta2(int value)
    {
        FrutaCogida[1] = value;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Winner()
    {
        if (FrutaCogida[0] > FrutaCogida[1])
        {
            
        }
        else if (FrutaCogida[0] < FrutaCogida[1])
        {

        }
        else if (FrutaCogida[0] == FrutaCogida[1])
        {

        }
    }
}
