using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ManagerHUDChutechutegol : MonoBehaviour
{
    enum ChuteGol { ChuteDelantero, ChutePortero, Porteria, Cambio}

    ChuteGol chuteGol;

    [SerializeField] TextMeshProUGUI[] Punts;
    [SerializeField] TextMeshProUGUI TextTime;
    [SerializeField] Transform[] PlayersPos;
    [SerializeField] Camera[] MisCamaras;
    [SerializeField] GameObject[] TimesPlayers;
    [SerializeField] TextMeshProUGUI[] PlayerTime;
    

    [SerializeField] Transform PosPortero;

    [SerializeField] Transform PosPelota;

    //variable del tiempo de juego o turno
    float timeDurationForDelantero,
          timeGame;

    List<PlayersChuteGol> players = new List<PlayersChuteGol>();
    PelotaController MyPelota;

    Vector3[] Posiciones;
    Vector3 Posicion;
    Vector3 pelota;
    bool inputEnable;

    string[] Comparation = new string[2];

    private void Start()
    {
        timeDurationForDelantero = 3;
        timeGame = 45;
        int random;

        for (int i = 0; i < Punts.Length; i++)
            Punts[i].text = 0.ToString();

        //instaciamos
        //variable para el portero
        random = Random.Range(0,3);
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetID() == random)
            {
                players[i].SetPortero();
            }
        }
        //salavamos las posicones originales de los personajes para poder rotarl segun el turno del jugador.
        Posiciones = new Vector3[PlayersPos.Length];
        for (int i = 0; i < PlayersPos.Length; i++)
        {
            Posiciones[i] = PlayersPos[i].position;
        }

        //salvamos la poscion original del portero para que vuelva a su puesto de origen depues de intentar proteger la porteria
        Posicion = PosPortero.position;
        //y lo mismo coin l a pelota, le devolvemos su posicion para que vuelva a su posicion para el sigunte jugador
        pelota = PosPelota.position;
        //asiganmos los turnos a los personajes
        Inicio();
        //segun los turnos habra que modifica en caso de que el portero no sea el player y sea otro jugador.
        ChangeTransfrom();
        chuteGol = ChuteGol.ChuteDelantero;
    }

    public Vector3 PoscionPelota() { return pelota; }
    public void MiPelotaList(PelotaController Ball)
    {
        MyPelota = Ball;
    }
    void Inicio()
    {
        List<int> list = new List<int>();
        int CantPlayers = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetPortero())
            {
                list.Add(CantPlayers);
                CantPlayers++;
            }
            else
            {
                //a la hora de tnener que hacer cambios para que el portero no modifique su Transform
                players[i].SetTurn(4);
            }
        }


        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int RandomIndex = Random.Range(i, list.Count);
            list[i] = list[RandomIndex];
            list[RandomIndex] = temp;
        }
        int indice = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetPortero())
            {
                players[i].SetTurn(list[indice]);
                indice++;
            }
        }
    }

    private void Update()
    {
        TimeController();
    }

    void CambioStat()
    {
        switch (chuteGol)
        {
            case ChuteGol.ChuteDelantero:
                chuteGol = ChuteGol.ChutePortero;
                ActivarIA();
                break;
            case ChuteGol.ChutePortero:
                chuteGol = ChuteGol.Porteria;
                break;
            case ChuteGol.Porteria:
                chuteGol = ChuteGol.Cambio;
                break;
            case ChuteGol.Cambio:
                chuteGol = ChuteGol.ChuteDelantero;
                break;
        }
    }

    void ActivarIA()
    {
        for (int i = 0; i < players.Count;i++) { 
            if (chuteGol == ChuteGol.ChuteDelantero)
            {
                if (players[i].getTurn() == 0)
                {
                    players[i].IAACTIVE();
                }
            }
            else if (chuteGol == ChuteGol.ChutePortero)
            {
                if (players[i].GetPortero())
                {
                    players[i].IAACTIVE();
                }
            }
        
        }
    }

    private void FixedUpdate()
    {
        if (timeGame > 1)
        {
            switch (chuteGol)
            {
                case ChuteGol.ChuteDelantero:
                    ChuteDelantero();
                    break;
                case ChuteGol.ChutePortero:
                    Portero();
                    break;
                case ChuteGol.Porteria:
                    ThePorteria();
                    break;
                case ChuteGol.Cambio:
                    Cambio();
                    break;
            }
        }
    }

    void ChuteDelantero()
    {
        MisCamaras[0].depth = -1;
        MisCamaras[1].depth = -2;
        TimesPlayers[1].SetActive(false);
        TimesPlayers[0].SetActive(true);
        for (int  i = 0; i < players.Count; i++)
        {
            if (players[i].getTurn() == 0)
            {
                players[i].MiInputEnable();
                inputEnable = true;
            }
        }

        if (inputEnable)
        {
            if (timeDurationForDelantero > 1)
            {
                timeDurationForDelantero -= Time.deltaTime;
                UpdateInterfaz(timeDurationForDelantero, PlayerTime[0]);
            }
            else
            {
                timeDurationForDelantero = 3;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].getTurn() == 0)
                    {
                        players[i].MiInputDisable();
                        inputEnable = false;
                        CambioStat();
                    }
                }
            }
        }

    }

    void Portero()
    {
        MisCamaras[1].depth = -1;
        MisCamaras[0].depth = -2;
        TimesPlayers[1].SetActive(true);
        TimesPlayers[0].SetActive(false);
        for (int i = 0; i < players.Count;i++)
        {
            if (players[i].GetPortero())
            {
                players[i].MiInputEnable();
                inputEnable = true;
            }
        }
        if (inputEnable)
        {
            if (timeDurationForDelantero > 1)
            {
                timeDurationForDelantero -= Time.deltaTime;
                UpdateInterfaz(timeDurationForDelantero, PlayerTime[1]);
            }
            else
            {
                timeDurationForDelantero = 3;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].GetPortero())
                    {
                        players[i].MiInputDisable();
                        inputEnable = false;
                        CambioStat();
                    }
                }
            }
        }
        
    }

    void ThePorteria()
    {
        MisCamaras[0].depth = -1;
        MisCamaras[1].depth = -2;
        TimesPlayers[1].SetActive(false);
        TimesPlayers[0].SetActive(false);
        ComprbarAcciones();
        //variables par activar el movinto de la pelota y el portero
        //Actualizmoas los puntos en si
        PuntsUI();
        CambioStat();  
    }

    //funcion para comprbar si el delantero mete gol o el portero para el tiro a porteria 
    void ComprbarAcciones()
    {
        //creamos las variables
        string actionPortero = "";
        string actionDelantero = "";
        //hacemos un bucle for para comproabr la lista de jugadores y slavamos la accion que tenga el delntero y el portero
        for (int i = 0; i <players.Count; i++)
        {
            if (players[i].GetPortero())
                actionPortero = players[i].GetNomAction();
            else
            {
                if (players[i].getTurn() == 0)
                {
                    actionDelantero = players[i].GetNomAction();
                }
            }
        }

        //ahora acmeos la comprobamcion de la informacion salvada en las variables
        //si son iguales, eso siginifca punto para el portero
        if (string.Equals(actionDelantero, actionPortero, System.StringComparison.OrdinalIgnoreCase))
        {
            //en caso que el delnatero sea dos jugadore y niguno le ha dado a niguna accion, no regalar puntos
            if (!(actionDelantero == "") && !(actionPortero == ""))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].GetPortero())
                    {
                        UpdatePunts(players[i]);
                    }
                }
            }
        }
        //si no son iguale punto para el delnatero
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].getTurn() == 0)
                {
                    UpdatePunts(players[i]);
                }
            }
        }
    }

    //funcion para el stado de cmabio, que restera los valores y cmabio de turnos de los jugaodres(Delanteros)
    void Cambio()
    {
        //funcion hacer cambio
        for (int i = 0; i < players.Count;i++)
        {
            if (!players[i].GetPortero())
            {
                int totalEstados = 3;
                players[i].SetTurn((players[i].getTurn() + 1) % totalEstados);
            }
            players[i].SetNom();
        }
        PuntsUI();
        ChangeTransfrom();
        MyPelota.transform.position = pelota;
        CambioStat();
        ActivarIA();
    }
    //aqu� es para la gestion de las posciones de los jugadores y el balon, son para resterar las posicones a su posino de origen.
    void ChangeTransfrom()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int turn = players[i].getTurn();

            // Si el turno coincide con un �ndice v�lido en la lista de posiciones (0, 1, 2)
            if (turn >= 0 && turn < Posiciones.Length)
            {
                players[i].setTransform(Posiciones[turn]);
            }
            else if (turn == 4)
            {
                // Caso especial para la posici�n fija (el portero)
                players[i].setTransform(Posicion);
            }
        }
    }
    //esto es para actulizar el hud sobre los puntos segun los turnos que tenga cada jugador
    void PuntsUI()
    {
        for (int i = 0; i < players.Count;i++)
        {
            switch (players[i].getTurn())
            {
                case 0:
                    Punts[1].text = players[i].GetPunts().ToString();
                    break;
                case 1:
                    Punts[2].text = players[i].GetPunts().ToString();
                    break;
                case 2:
                    Punts[3].text = players[i].GetPunts().ToString();
                    break;
                case 4:
                    Punts[0].text = players[i].GetPunts().ToString();
                    break;
            }
        }
    }

    //funicon para a�adir jugaodres a la lista de los jugadores, para controlar a los jugadores desde directmente un lista
    public void RegistroPlayer(PlayersChuteGol MiJugador)
    {
        players.Add(MiJugador);
    }
    
    //esta funocomn es para actulizar el tiempo que se muttre en el hud del minijuego
    void UpdateInterfaz(float time, TextMeshProUGUI MyTexto)
    {
        // Formateamos para mostrar Minutos:Segundos
        float minutos = Mathf.FloorToInt(time / 60);
        float segundos = Mathf.FloorToInt(time % 60);
        MyTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    //actulizamos los puntos de los jugadores 
    void UpdatePunts(PlayersChuteGol player)
    {
        player.SetPunts();
    }

    //funion para controlar el timepo del minijuego chute chute gol.
    void TimeController()
    {
        if (timeGame > 1)
        {
            timeGame -= Time.deltaTime;
            UpdateInterfaz(timeGame,TextTime);
        }
        else
        {
            //condicion de victorias
            AssignarPosicones();
            //abrir el hud de romcpnsas

        }
    }

    void AssignarPosicones()
    {
        //salavmos la putacion en una lsita alterna
        int[] score = new int[4];
        for (int i = 0; i < players.Count; i++)
        {
            score[i] = players[i].Score;
        }
        //ordenmso el array del score
        BubbleSort(score);
        if (score[0] == score[1])
        {
            //hay empate entre los dos priemro jugadores
        }
        else
        {

        }
    }

    void BubbleSort(int[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                // Si el n�mero de la izquierda es mayor que el de la derecha...
                if (array[j] > array[j + 1])
                {
                    // Intercambio de valores (Swap)
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
}