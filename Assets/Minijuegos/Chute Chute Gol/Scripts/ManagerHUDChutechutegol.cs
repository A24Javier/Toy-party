using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerHUDChutechutegol : MonoBehaviour
{
    enum ChuteGol { ChuteDelantero, ChutePortero, Porteria, Cambio }

    ChuteGol chuteGol;

    [Header("HUD")]
    [SerializeField] TextMeshProUGUI[] Punts;
    [SerializeField] TextMeshProUGUI TextTime;
    [SerializeField] GameObject[] TimesPlayers;
    [SerializeField] TextMeshProUGUI[] PlayerTime;

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

    private void Start()
    {
        timeDurationForDelantero = tiempoTurno;
        timeGame = tiempoTotalMinijuego;
        goTime = tiempoAntesDeEmpezar;

        incio = true;
        inputEnable = false;
        PuntosON = false;
        CourrtineActive = false;
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

        if (CourrtineActive)
        {
            CourrtineActive = false;
            StartCoroutine(EsperarResolucion());
        }
    }

    IEnumerator EsperarResolucion()
    {
        if (MiDirector != null)
        {
            yield return new WaitUntil(() => MiDirector.continueScript);
            MiDirector.PausarAnimationPorteroPelota();
        }

        PuntsUI();
        CambioStat();
    }

    void ComprobarAcciones()
    {
        string actionPortero = "";
        string actionDelantero = "";

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            if (players[i].GetPortero())
                actionPortero = players[i].GetNomAction();
            else if (players[i].getTurn() == 0)
                actionDelantero = players[i].GetNomAction();
        }

        if (!string.IsNullOrEmpty(actionDelantero) && !string.IsNullOrEmpty(actionPortero))
        {
            if (MiDirector != null)
                MiDirector.SetActions(actionDelantero, actionPortero);
        }

        Debug.Log(actionDelantero + " - " + actionPortero);

        PorteroParo = string.Equals(actionDelantero, actionPortero, System.StringComparison.OrdinalIgnoreCase);

        if (MiDirector != null)
            MiDirector.StartAction();

        if (PuntosON)
            AsignarPuntos(actionDelantero, actionPortero);
    }

    void AsignarPuntos(string actionDelantero, string actionPortero)
    {
        if (PorteroParo)
        {
            if (!string.IsNullOrEmpty(actionDelantero) && !string.IsNullOrEmpty(actionPortero))
            {
                foreach (var p in players)
                {
                    if (p.GetPortero())
                        UpdatePunts(p);
                }
            }
        }
        else
        {
            foreach (var p in players)
            {
                if (p.getTurn() == 0)
                    UpdatePunts(p);
            }
        }

        PuntosON = false;
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

            players[i].SetNom();
            players[i].NewAction();
        }

        PuntsUI();

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