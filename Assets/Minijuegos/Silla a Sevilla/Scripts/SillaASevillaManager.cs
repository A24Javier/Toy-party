using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SillaASevillaManager : MonoBehaviour
{
    //variable de la maquina de estado
    enum Statjoc {StartRound, Play, InfoPlay ,ChooseChair, Eleminated}
    Statjoc MyStat;

    //variables serilizefiled
    [SerializeField] TextMeshProUGUI MyTextronda;
    [SerializeField] GameObject[] HudObject;
    [SerializeField] ChairController[] MisSillas;
    [SerializeField] Image[] ImgHud;
    
    //varibales del juego
    string Textronda;
    int ronda;

    //varibles de tiempo
    float timeRonda, timeplay, timeinfoplaying, timeChoose, timeeleminated;

    List<PlayerControllerSillaaSevilla> MisJugadores = new List<PlayerControllerSillaaSevilla>();
    List<GameObject> ObjectoJugador = new List<GameObject>();
    Sprite[] MisSprite;

    List<PlayerSillaaSevilla> MiRegstroEleminados = new List<PlayerSillaaSevilla>();

    private void Start()
    {
        //inicmaos la varible de la maquina de estado
        MyStat = Statjoc.StartRound;
        //instaciamos la varible string
        Textronda = "Ronda ";
        //instaciamos la variable de la ronda
        ronda = 1;
        //inicmaos los tiempos de cada varible de tiempo
        timeRonda = 3.0f;
        timeplay = 15.0f;
        timeinfoplaying = 2.0f;
        timeChoose = 8.0f;
        timeeleminated = 1.0f;
        //desactivamos los objectos del hud al inicar el juego
        for (int  i = 0; i < HudObject.Length; i++)
            HudObject[i].SetActive(false);
    }

    public void RegistraJugador(PlayerControllerSillaaSevilla player)
    {
        MisJugadores.Add(player);
        ObjectoJugador.Add(player.gameObject);
    }

    private void FixedUpdate()
    {
        switch (MyStat)
        {
            case Statjoc.StartRound:
                RunNewRonda();
                 break;
            case Statjoc.Play:
                RunPlay();
                break;
            case Statjoc.InfoPlay:
                InfoPlaying();
                break;
            case Statjoc.ChooseChair:
                RunInputs();
                break;
            case Statjoc.Eleminated:
                RunEleminated();
                break;
        }  
    }

    //funcion para que los jugadores vuelvan a sus posiciones originales.
    void ResetPosPlayers()
    {
        //ponemos a los persojnaes a su poscion original
        for (int i = 0; i < MisJugadores.Count; i++)
        {
            MisJugadores[i].PosPlayer();
        }
        //Liberamos las sillas
        for (int i = 0; i < MisSillas.Length; i++)
        {
            MisSillas[i].Liberar();
        }
    }

    void RunNewRonda()
    {
        HudObject[0].SetActive(true);
       
        switch (ronda)
        {
            case 1:
                MyTextronda.text = Textronda + ronda.ToString();
                break;
            case 2:
                MyTextronda.text = Textronda + ronda.ToString();
                //desactivamos la silla del medio
                MisSillas[0].gameObject.SetActive(false);
                ResetPosPlayers();
                break;
            case 3:
                MyTextronda.text = "Ultima Ronda";
                //activamos la silla del medio
                MisSillas[0].gameObject.SetActive(true);
                //desactivamos las otras sillas
                MisSillas[1].gameObject.SetActive(false);
                MisSillas[2].gameObject.SetActive(false);
                ResetPosPlayers();
                break;
        }
        
       
         //Time controller
        if (timeRonda > 0)
        {
            timeRonda -= Time.deltaTime;
        }
        else
        {
            //resetamos el tiempo
            timeRonda = 3.0f;
            //cambaimos el estado
            MyStat = Statjoc.Play;
        }
    }

    void RunPlay()
    {
        HudObject[0].SetActive(false);
        //activar musica

      


        //time Contorller
        if (timeplay > 0)
        {
            timeplay -= Time.deltaTime;
        }
        else
        {
            //resteamos el timepo
            timeplay = 15.0f;
            //cambaio el estado
            MyStat = Statjoc.InfoPlay;
        }
    }

    void InfoPlaying()
    {
        HudObject[0].SetActive(true);
        MyTextronda.text = "Escoge una silla";
        if (timeinfoplaying > 0)
        {
            timeinfoplaying -= Time.deltaTime;
        }
        else
        {
            timeinfoplaying = 2.0f;
            HudObject[0].SetActive(false);
            MyStat = Statjoc.ChooseChair;
        }
    }
    void RunInputs()
    {
        //desactivarmusica
        //ativar los inputs del juego
        for (int i = 0; i < MisJugadores.Count; i++)
            MisJugadores[i].MoveEnable();

        if (timeChoose > 0)
        {
            timeChoose -= Time.deltaTime;
        }
        else
        {
            timeChoose = 8.0f;
            MyStat = Statjoc.Eleminated;
        }
    }

    void RemovePlayer(int indice)
    {
        GameObject player = ObjectoJugador[indice];

        PlayerSillaaSevilla MiPlayer = new PlayerSillaaSevilla(MisJugadores[indice].id,ronda);
        MiRegstroEleminados.Add(MiPlayer);
       
        ObjectoJugador.RemoveAt(indice);
        MisJugadores.RemoveAt(indice);

        Destroy(player);
    }

    void RunEleminated()
    {
        //bloquemos las acciones en caso necesario que no se haya bloquedo todo
        for (int i = 0; i < MisJugadores.Count; i++)
            MisJugadores[i].MoveDisabled();


        //comprbamos que jugador no esta sentado en niguna silla, y se elemina el jugador
        for (int i = 0; i < MisJugadores.Count; i++)
        {
            if (!MisJugadores[i].Sit)
            {
                switch (MisJugadores[i].GetId())
                {
                    case 1:
                        HudObject[1].SetActive(true);
                        break;
                    case 2:
                        HudObject[2].SetActive(true);
                        break;
                    case 3:
                        HudObject[3].SetActive(true);
                        break;
                    case 4:
                        HudObject[4].SetActive(true);
                        break;
                }
                RemovePlayer(i);
            }
        }


        //time controller
        if (timeeleminated > 0)
        {
            timeeleminated -= Time.deltaTime;
        }
        else
        {
            if (ronda < 3)
            {
                //comprobamos que si quedan más de un jugador en pie
                if (MisJugadores.Count > 1)
                { 
                    //sumamos la varibale de ronda, ya que vamos a la sigunte ronda
                    ronda++;
                    timeeleminated = 1.0f;
                    MyStat = Statjoc.StartRound;
                }
                //en caso de que no quede más de uno
                else
                {
                    //comprobamos al menos que quede uno en pie 
                    if (MisJugadores.Count == 1)
                    {
                        //comprobamos que la condicon de jugador de pie, para ponerle la primera posicion
                        //if (MisJugadores[0].id == Caracter.id)
                            //Caratecet.pos = 1;
                    }
                    //en caso contrario, dependera si es la rona 1 o 2
                    else
                    {
                        //Si no queda nadie, lo que se hara los ultimos en eleminarse ponerle la primera posicion.
                        for (int i = 0; i < MiRegstroEleminados.Count; i++)
                        {
                            switch (MiRegstroEleminados[i].VerPos())
                            {
                                //si es la primera ronda, pondremos a todos los jugadores como ganadores
                                case 1:
                                    //carater.pos = 
                                    break;
                                //si es la segunda ronda quein haya sido elimindo primero 
                                case 2:
                                    //comprabremos quien ha sido eleminado desde la ronda 1 y quien de la segunda ronda 
                                    if (MiRegstroEleminados[i].VerPos() == 1)
                                    {
                                        //los de la primera ronda seran ascendio ha segunda posicion
                                    }
                                    else
                                    {
                                        //los que han quedado en la segunda se pasaran a primero
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            if (ronda == 3)
            {
                //ver quien queda de piea que sera el ganador
                if (MisJugadores.Count == 1)
                {
                    //comprobamos que la condicon de jugador de pie, para ponerle la primera posicion
                    //if (MisJugadores[0].id == Caracter.id)
                    //Caratecet.pos = 1;
                }
            }
        } // fin del time contoller
    }
}
