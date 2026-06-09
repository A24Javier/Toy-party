using MongoDB.Bson.Serialization.Serializers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerHUDChutechutegol : MonoBehaviour
{
    enum ChuteGol { ChuteDelantero, ChutePortero, Porteria, Cambio }

    ChuteGol chuteGol;

    [Header("HUD")]
    [SerializeField] TextMeshProUGUI[] Punts;
    [SerializeField] TextMeshProUGUI TextTime;
    [SerializeField] GameObject[] TimesPlayers;
    [SerializeField] TextMeshProUGUI[] PlayerTime;
    [SerializeField] Image[] PerfilPlayer;

    [Header("Posiciones")]
    [SerializeField] Transform[] PlayersPos;
    [SerializeField] Transform PosPortero;
    [SerializeField] Transform PosPelota;

    [Header("Cámaras")]
    [SerializeField] Camera[] MisCamaras;

    [Header("Timeline")]
    [SerializeField] public ConrtoladorTimeLineChuteChuteGol MiDirector;

    [Header("Audio")]
    [SerializeField] AudioSource MiAudio;

    [Header("Escenas")]
    [SerializeField] string nombreEscenaRecompensas = "NivelRecompensasMinijuegos";

    [Header("Tiempo")]
    [SerializeField] float tiempoTurno = 3f;
    [SerializeField] float tiempoTotalMinijuego = 45f;
    [SerializeField] float tiempoAntesDeEmpezar = 1.5f;

    float timeDurationForDelantero;
    float timeGame;
    float goTime;

    public List<PlayersChuteGol> players = new List<PlayersChuteGol>();

    PelotaController MyPelota;

    Vector3[] Posiciones;
    Vector3 PosicionPorteroInicial;
    Vector3 PosicionPelotaInicial;

    bool inputEnable;
    public bool PorteroParo = false;

    bool PuntosON;
    bool CourrtineActive;
    bool incio;
    bool minijuegoFinalizado;

    [SerializeField] ScalaPersonajes Mod;

    private string ultimaActionDelantero = "";
    private string ultimaActionPortero = "";

    private void Awake()
    {
        Mod.InicializarMinijuego(5);
    }

    private void Start()
    {
        timeDurationForDelantero = tiempoTurno;
        timeGame = tiempoTotalMinijuego;
        goTime = tiempoAntesDeEmpezar;

        incio = true;
        inputEnable = false;
        PuntosON = false;
        minijuegoFinalizado = false;

        for (int i = 0; i < Punts.Length; i++)
            Punts[i].text = "0";

        for (int i = 0; i < TimesPlayers.Length; i++)
            TimesPlayers[i].SetActive(false);

        if (players.Count == 0)
        {
            Debug.LogError("No hay jugadores registrados en ManagerHUDChutechutegol.");
            return;
        }

        ElegirPorteroInicial();

        if (MiDirector != null)
            MiDirector.AssignarPortero();

        Posiciones = new Vector3[PlayersPos.Length];

        for (int i = 0; i < PlayersPos.Length; i++)
            Posiciones[i] = PlayersPos[i].position;

        if (PosPortero != null)
            PosicionPorteroInicial = PosPortero.position;

        if (PosPelota != null)
            PosicionPelotaInicial = PosPelota.position;

        Inicio();
        ChangeTransfrom();

        ActulizarImagePerfil();
        
        chuteGol = ChuteGol.ChuteDelantero;
    }

    void ElegirPorteroInicial()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);

        for (int i = 0; i < players.Count; i++)
        {
            if (i == randomIndex)
                players[i].SetPortero();
        }
    }

    public Vector3 PoscionPelota()
    {
        return PosicionPelotaInicial;
    }

    public void MiPelotaList(PelotaController Ball)
    {
        MyPelota = Ball;
    }

    void Inicio()
    {
        List<int> turnosDisponibles = new List<int>();

        int cantidadDelanteros = 0;

        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetPortero())
            {
                turnosDisponibles.Add(cantidadDelanteros);
                cantidadDelanteros++;
            }
            else
            {
                players[i].SetTurn(4);
            }
        }

        for (int i = 0; i < turnosDisponibles.Count; i++)
        {
            int temp = turnosDisponibles[i];
            int randomIndex = Random.Range(i, turnosDisponibles.Count);
            turnosDisponibles[i] = turnosDisponibles[randomIndex];
            turnosDisponibles[randomIndex] = temp;
        }

        int indice = 0;

        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetPortero())
            {
                players[i].SetTurn(turnosDisponibles[indice]);
                indice++;
            }
        }
    }

    private void Update()
    {
        if (minijuegoFinalizado)
            return;

        if (goTime > 0)
        {
            goTime -= Time.deltaTime;

            for (int i = 0; i < TimesPlayers.Length; i++)
                TimesPlayers[i].SetActive(false);

            return;
        }

        if (TimesPlayers.Length > 2)
            TimesPlayers[2].SetActive(true);

        TimeController();
    }

    public void CambioStat()
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
        for (int i = 0; i < players.Count; i++)
        {
            if (chuteGol == ChuteGol.ChuteDelantero)
            {
                if (players[i].getTurn() == 0)
                    players[i].MiInputEnable();
            }
            else if (chuteGol == ChuteGol.ChutePortero)
            {
                if (players[i].GetPortero())
                    players[i].MiInputEnable();
            }
        }
    }

    private void FixedUpdate()
    {
        if (minijuegoFinalizado)
            return;

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
        SetCameraDepth(0);

        if (TimesPlayers.Length > 1)
            TimesPlayers[1].SetActive(false);

        if (TimesPlayers.Length > 0)
            TimesPlayers[0].SetActive(true);

        for (int i = 0; i < players.Count; i++)
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

                if (PlayerTime.Length > 0)
                    UpdateInterfaz(timeDurationForDelantero, PlayerTime[0]);
            }
            else
            {
                timeDurationForDelantero = tiempoTurno;

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].getTurn() == 0)
                    {
                        players[i].MiInputDisable();
                        inputEnable = false;

                        if (MiDirector != null)
                            MiDirector.MoverDelantero();
                    }
                }
            }
        }
    }

    void Portero()
    {
        SetCameraDepth(1);

        if (TimesPlayers.Length > 1)
            TimesPlayers[1].SetActive(true);

        if (TimesPlayers.Length > 0)
            TimesPlayers[0].SetActive(false);

        for (int i = 0; i < players.Count; i++)
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

                if (PlayerTime.Length > 1)
                    UpdateInterfaz(timeDurationForDelantero, PlayerTime[1]);
            }
            else
            {
                timeDurationForDelantero = tiempoTurno;

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
        SetCameraDepth(0);

        if (TimesPlayers.Length > 1)
            TimesPlayers[1].SetActive(false);

        if (TimesPlayers.Length > 0)
            TimesPlayers[0].SetActive(false);

        ComprobarAcciones();
    }

    void ComprobarAcciones()
    {
        string actionPortero = "";
        string actionDelantero = "";

        // 1. Recolectamos las acciones actuales de los jugadores
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            if (players[i].GetPortero())
                actionPortero = players[i].GetNomAction();
            else if (players[i].getTurn() == 0)
                actionDelantero = players[i].GetNomAction();
        }

        // 2. Si AMBOS han elegido, disparamos el Timeline de forma segura
        if (!string.IsNullOrEmpty(actionDelantero) && !string.IsNullOrEmpty(actionPortero))
        {
            // Almacenamos las acciones para procesarlas al final de la animación
            ultimaActionDelantero = actionDelantero;
            ultimaActionPortero = actionPortero;

            // Calculamos internamente si el portero detuvo el balón
            PorteroParo = string.Equals(actionDelantero, actionPortero, System.StringComparison.OrdinalIgnoreCase);

            if (MiDirector != null)
            {
                // Pasamos las acciones al Director para que arme el estado (Gol o Parada)
                MiDirector.SetActions(actionDelantero, actionPortero);

                // Iniciamos la animación (Timeline)
                MiDirector.StartAction();
            }

            // [IMPORTANTE]: Aquí debes limpiar el string de tus jugadores para evitar bucles repetidos
            // Ejemplo: 
            foreach(var p in players) { p.SetNom(""); }
        }
    }

    public void FinalizarAnimacionYDarPuntos()
    {

        if (MiDirector != null)
        {
            MiDirector.PausarAnimationPorteroPelota();
        }

        if (PuntosON)
        {
            AsignarPuntos(ultimaActionDelantero, ultimaActionPortero);
        }

        PuntsUI();
        
        CambioStat();
    }

    void AsignarPuntos(string actionDelantero, string actionPortero)
    {
        if (PorteroParo)
        {
            // Si hay parada, sumamos puntos solo al portero
            foreach (var p in players)
            {
                if (p != null && p.GetPortero())
                    UpdatePunts(p);
            }
        }
        else
        {
            // Si hay gol, sumamos puntos al delantero (turno 0)
            foreach (var p in players)
            {
                if (p != null && p.getTurn() == 0)
                    UpdatePunts(p);
            }
        }

        if (MiDirector != null)
        {
            MiDirector.ReniciarTimeLines(); // Pone todos los Timelines al punto 0 y vacía variables
        }

        // Reseteamos los controladores de estado para la siguiente ronda
        PuntosON = false;
        PorteroParo = false;
    }

    void Cambio()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetPortero())
            {
                int totalEstados = 3;
                players[i].SetTurn((players[i].getTurn() + 1) % totalEstados);
            }

            players[i].SetNom("");
            players[i].NewAction();
        }

        PuntsUI();
        ActulizarImagePerfil();
        if (MiDirector != null)
            MiDirector.ReniciarTimeLines();

        ChangeTransfrom();

        if (MyPelota != null)
            MyPelota.transform.position = PosicionPelotaInicial;

        ActivarIA();
        CambioStat();
    }

    void ChangeTransfrom()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int turn = players[i].getTurn();

            if (turn >= 0 && turn < Posiciones.Length)
            {
                players[i].setTransform(Posiciones[turn]);
            }
            else if (turn == 4)
            {
                if (incio)
                {
                    players[i].setTransform(PosicionPorteroInicial);
                    incio = false;
                }
            }
        }

        if (MiDirector != null)
            MiDirector.AssignarDelantero();

        PuntosON = true;
        CourrtineActive = true;
    }

    void PuntsUI()
    {
        for (int i = 0; i < players.Count; i++)
        {
            switch (players[i].getTurn())
            {
                case 0:
                    if (Punts.Length > 1) Punts[1].text = players[i].GetPunts().ToString();
                    break;

                case 1:
                    if (Punts.Length > 2) Punts[2].text = players[i].GetPunts().ToString();
                    break;

                case 2:
                    if (Punts.Length > 3) Punts[3].text = players[i].GetPunts().ToString();
                    break;

                case 4:
                    if (Punts.Length > 0) Punts[0].text = players[i].GetPunts().ToString();
                    break;
            }
        }
    }

    void ActulizarImagePerfil()
    {
        for (int i = 0; i < players.Count; i++)
        {
            switch (players[i].getTurn())
            {
                case 0:
                    PerfilPlayer[1].sprite = players[i].GetSprite();
                    rotacionModelo(players[i], 0);
                    break;
                case 1:
                    PerfilPlayer[2].sprite = players[i].GetSprite();
                    rotacionModelo(players[i], 1);
                    break;
                case 2:
                    PerfilPlayer[3].sprite = players[i].GetSprite();
                    rotacionModelo(players[i], 2);
                    break;
                case 4:
                    PerfilPlayer[0].sprite = players[i].GetSprite();
                    rotacionModelo(players[i], 4);
                    break;
            }
        }
        
    }

    void rotacionModelo(PlayersChuteGol MyPlayer, int turno)
    {
        float rotacionY = 0f;

        // Aquí manejas la lógica de los turnos de forma general para todos los personajes
        if (turno == 0)
        {
            rotacionY = 0f; // Mirar al frente en el turno 0
        }
        else if (turno == 1 || turno == 2)
        {
            rotacionY = 90f; // Mirar a la derecha en los turnos 1 y 2
        }
        else if (turno == 4)
        {
            rotacionY = 180f; // Mirar a la izquierda en el turno 4
        }

        // SI UN PERSONAJE TIENE UNA EXCEPCIÓN (Por ejemplo, el personaje 0 se comporta al revés)
        if (MyPlayer.idPersonaje == 3)
        {
            // Puedes alterar la rotación aquí solo para él si lo necesitas
            rotacionY += 90f; 
        }

        // Llamamos al gestor que creamos en el paso anterior para aplicar la rotación por ID
        MyPlayer.MiModelo.RotarPersonajeEnTiempoDeJuego(MyPlayer.idPersonaje, rotacionY);
    }

    public void RegistroPlayer(PlayersChuteGol MiJugador)
    {
        if (!players.Contains(MiJugador))
            players.Add(MiJugador);
    }

    void UpdateInterfaz(float time, TextMeshProUGUI MyTexto)
    {
        if (MyTexto == null) return;

        float minutos = Mathf.FloorToInt(time / 60);
        float segundos = Mathf.FloorToInt(time % 60);

        MyTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void UpdatePunts(PlayersChuteGol player)
    {
        player.SetPunts();
    }

    void TimeController()
    {
        if (minijuegoFinalizado)
            return;

        if (timeGame > 1)
        {
            timeGame -= Time.deltaTime;
            UpdateInterfaz(timeGame, TextTime);
        }
        else
        {
            minijuegoFinalizado = true;

            if (MiAudio != null)
                MiAudio.Stop();

            AssignarPosicones();
            FinalizarMinijuego();
        }
    }

    void AssignarPosicones()
    {
        DatosMinijuego.ResetDatos();
        DatosMinijuego.cantidadJugadores = players.Count;

        List<PlayersChuteGol> ranking = players
            .OrderByDescending(p => p.GetPunts())
            .ThenBy(p => p.GetID())
            .ToList();

        for (int i = 0; i < ranking.Count; i++)
        {
            PlayersChuteGol jugador = ranking[i];

            int idJugador = jugador.GetID();

            if (idJugador < 1 || idJugador > 4)
            {
                Debug.LogWarning("Jugador con ID fuera de rango: " + idJugador);
                continue;
            }

            int index = idJugador - 1;

            DatosMinijuego.ids[index] = idJugador;
            DatosMinijuego.puntos[index] = jugador.GetPunts();
            DatosMinijuego.posiciones[index] = i + 1;
            DatosMinijuego.estrellas[index] = 0;
            DatosMinijuego.monedas[index] = 0;

            Debug.Log($"Jugador {idJugador} queda en posición {i + 1} con {jugador.GetPunts()} puntos.");
        }
    }

    void FinalizarMinijuego()
    {
        Scene escenaActual = gameObject.scene;

        DatosMinijuego.escenaRecompensas = nombreEscenaRecompensas;

        SceneManager.LoadSceneAsync(nombreEscenaRecompensas, LoadSceneMode.Additive).completed += (op) =>
        {
            ManagerFinMinijuego managerNueva = Object.FindObjectOfType<ManagerFinMinijuego>();

            if (managerNueva != null)
            {
                managerNueva.minigame = ManagerFinMinijuego.TipoMiniGame.OtherMinigames;
            }
            else
            {
                Debug.LogWarning("No se encontró ManagerFinMinijuego en la escena de recompensas.");
            }

            if (escenaActual.IsValid() && escenaActual.isLoaded)
            {
                SceneManager.UnloadSceneAsync(escenaActual);
            }
        };
    }

    void SetCameraDepth(int camaraPrincipal)
    {
        if (MisCamaras == null || MisCamaras.Length < 2)
            return;

        if (camaraPrincipal == 0)
        {
            MisCamaras[0].depth = -1;
            MisCamaras[1].depth = -2;
        }
        else
        {
            MisCamaras[1].depth = -1;
            MisCamaras[0].depth = -2;
        }
    }
}