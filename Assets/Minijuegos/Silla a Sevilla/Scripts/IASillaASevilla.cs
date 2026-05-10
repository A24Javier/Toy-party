using UnityEngine;

public class IASillaASevilla : MonoBehaviour
{
    float timeTaming;
    bool primeraLlamada = false;

    PlayerControllerSillaaSevilla MiPlayer;

    private void Awake()
    {
        MiPlayer = GetComponent<PlayerControllerSillaaSevilla>();
    }

    public void MoveIA()
    {
        if (MiPlayer == null)
            return;

        if (!primeraLlamada)
        {
            timeTaming = Random.Range(0.0f, 3.0f);
            primeraLlamada = true;
        }

        if (timeTaming > 0)
        {
            timeTaming -= Time.deltaTime;
        }
        else
        {
            primeraLlamada = false;
            MiPlayer.playerAction();
        }
    }
}