using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager2vs2 : MonoBehaviour
{
    public enum Juego2vs2 { RelayRace, SpeedyTrack }
    public Juego2vs2 MiJuego;

    //añadimos los jugadores
    List<PlayerController> playerControllers = new List<PlayerController>();
    //dos los lista equip1
    List<PlayerController> Equipo1 = new List<PlayerController>();
    //equipo2
    List<PlayerController> Equipo2 = new List<PlayerController>();

    bool JuegoTerminado = false;

    [SerializeField] GameObject[] Musica; 

    public void AñadirJuagdores(PlayerController Myplayer)
    {
        playerControllers.Add(Myplayer);
    }

    public void RemoveJugadores(PlayerController MyPlayer)
    {
        playerControllers.Add(MyPlayer);
    }

    private void Start()
    {
        for (int i = 0; i < Musica.Length; i++)
            Musica[i].SetActive(false);
        EscogerEquipo();
        JuegoTerminado = false;
        switch(MiJuego)
        {
            case Juego2vs2.RelayRace:
                OpenRelayRace();
                break;
            case Juego2vs2.SpeedyTrack:
                OpenSpeedyTrack();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (JuegoTerminado)
        {
            OpenLVRecompensas();
        }
    }

    //funciones del EscogerEquipo
    void EscogerEquipo()
   {
        int random;
        for (int i = 0; i < playerControllers.Count; i++)
        {
            random = Random.Range(0, 3);
            if (playerControllers[i].GetPlayerInfo().GetID() == random)
            {
                if (Equipo1.Count >= 0 && Equipo1.Count < 2)
                {
                    Equipo1.Add(playerControllers[i]);
                    RemoveJugadores(playerControllers[i]);
                }
                else if (Equipo2.Count >= 0 && Equipo2.Count < 2)
                {
                    Equipo2.Add(playerControllers[i]);
                    RemoveJugadores(playerControllers[i]);
                }
            }
        }
   }

    void OpenRelayRace()
    {
        Musica[0].SetActive(true);
    }

    //aquí se abre el juego speedy track y se pasa la informacion
    void OpenSpeedyTrack()
    {
        Musica[1].SetActive(true);
    }

    void OpenLVRecompensas()
    {

    }

    public void SeTerminoElJuego() => JuegoTerminado = true;

}
