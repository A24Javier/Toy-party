using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairController : MonoBehaviour
{
    public PlayerControllerSillaaSevilla MyPlayer;

    public Transform puntoParaSentarse;

    public bool EstaLibre() => MyPlayer == null;

    public void Ocupar(PlayerControllerSillaaSevilla jugador)
    {
        MyPlayer = jugador;
        MyPlayer.Sit = true;
    }

    public void Liberar()
    {
        MyPlayer = null;
    }
}
