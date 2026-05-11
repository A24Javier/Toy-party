using UnityEngine;

public class ChairController : MonoBehaviour
{
    public PlayerControllerSillaaSevilla MyPlayer;
    public Transform puntoParaSentarse;

    public bool EstaLibre()
    {
        return MyPlayer == null;
    }

    public void Ocupar(PlayerControllerSillaaSevilla jugador)
    {
        if (jugador == null)
            return;

        MyPlayer = jugador;
        MyPlayer.Sit = true;
    }

    public void Liberar()
    {
        if (MyPlayer != null)
            MyPlayer.Sit = false;

        MyPlayer = null;
    }
}