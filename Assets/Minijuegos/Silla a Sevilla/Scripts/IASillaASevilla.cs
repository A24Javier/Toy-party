using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASillaASevilla : MonoBehaviour
{
    // funicon para el moviemnto de ia, es para que la ia en el estado de juego pueda coger una de las sillas.
    float timeTaming;
    bool fristllamada = false;
    PlayerControllerSillaaSevilla MiPlayer;

    private void Start()
    {
        MiPlayer = GetComponent<PlayerControllerSillaaSevilla>();
    }
    public void MoveIA()
    {
        if (!fristllamada)
        {
            timeTaming = Random.Range(0.0f, 3.0f);
            fristllamada = true;
        }
        
        if (timeTaming > 0)
            timeTaming -= Time.deltaTime;
        else
        {
            fristllamada = false;
            MiPlayer.playerAction();
        }

    }
}
