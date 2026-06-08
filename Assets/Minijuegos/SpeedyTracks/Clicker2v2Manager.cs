using TMPro;
using UnityEngine;

public class Clicker2v2Manager : MonoBehaviour
{
    [Header("Equipos")]
    public Team equipoA;
    public Team equipoB;

    [Header("Config")]
    public int clicksParaGanar = 20;

    [Header("Jugadores equipo A")]
    public PlayerClicker jugadorA1;
    public PlayerClicker jugadorA2;

    [Header("Jugadores equipo B")]
    public PlayerClicker jugadorB1;
    public PlayerClicker jugadorB2;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoEquipoA;
    [SerializeField] private TextMeshProUGUI textoEquipoB;
    [SerializeField] private TextMeshProUGUI textoTurnos;
    [SerializeField] private TextMeshProUGUI textoGanador;

    private PlayerClicker turnoA;
    private PlayerClicker turnoB;

    public bool GameTerminated { get; private set; }

    [Header("Modelos")]
    [SerializeField] ScalaPersonajes Mod;

    private void Awake()
    {
        Mod.InicializarMinijuego(6);
    }

    private void Start()
    {
        GameTerminated = false;

        if (equipoA != null)
            equipoA.clicksTotales = 0;

        if (equipoB != null)
            equipoB.clicksTotales = 0;

        turnoA = jugadorA1;
        turnoB = jugadorB1;

        ConfigurarJugadores();
        ActualizarUI();
    }

    private void ConfigurarJugadores()
    {
        ConfigurarJugador(jugadorA1, TeamId.A, 1);
        ConfigurarJugador(jugadorA2, TeamId.A, 2);
        ConfigurarJugador(jugadorB1, TeamId.B, 3);
        ConfigurarJugador(jugadorB2, TeamId.B, 4);
    }

    private void ConfigurarJugador(PlayerClicker jugador, TeamId equipo, int id)
    {
        if (jugador == null)
            return;

        jugador.manager = this;
        jugador.equipoId = equipo;
        jugador.idJugador = id;
    }

    public void RegistrarClick(PlayerClicker jugador)
    {
        if (GameTerminated)
            return;

        if (jugador == null)
            return;

        if (jugador.equipoId == TeamId.A)
        {
            if (jugador != turnoA)
                return;

            equipoA.clicksTotales++;

            Debug.Log($"{jugador.nombreJugador} clicó → {equipoA.nombre}: {equipoA.clicksTotales}");

            turnoA = turnoA == jugadorA1 ? jugadorA2 : jugadorA1;
        }
        else
        {
            if (jugador != turnoB)
                return;

            equipoB.clicksTotales++;

            Debug.Log($"{jugador.nombreJugador} clicó → {equipoB.nombre}: {equipoB.clicksTotales}");

            turnoB = turnoB == jugadorB1 ? jugadorB2 : jugadorB1;
        }

        ActualizarUI();
        ComprobarVictoria();
    }

    private void ComprobarVictoria()
    {
        if (equipoA.clicksTotales >= clicksParaGanar)
        {
            FinalizarMinijuego(TeamId.A);
        }
        else if (equipoB.clicksTotales >= clicksParaGanar)
        {
            FinalizarMinijuego(TeamId.B);
        }
    }

    private void FinalizarMinijuego(TeamId equipoGanador)
    {
        if (GameTerminated)
            return;

        GameTerminated = true;

        DetenerJugadores();

        if (textoGanador != null)
        {
            textoGanador.text = equipoGanador == TeamId.A
                ? "Gana equipo " + equipoA.nombre
                : "Gana equipo " + equipoB.nombre;
        }

        GuardarResultados(equipoGanador);

        Debug.Log("Clicker 2v2 terminado. Ganador: " + equipoGanador);

        if (MinigameController.instance == null)
        {
            Debug.LogError("Clicker2v2Manager: no existe MinigameController.");
            return;
        }

        MinigameController.instance.OpenRewardScene();
    }

    private void GuardarResultados(TeamId equipoGanador)
    {
        DatosMinijuego.ResetDatos();
        DatosMinijuego.cantidadJugadores = 4;

        if (equipoGanador == TeamId.A)
        {
            GuardarJugador(jugadorA1, 1, equipoA.clicksTotales);
            GuardarJugador(jugadorA2, 1, equipoA.clicksTotales);

            GuardarJugador(jugadorB1, 2, equipoB.clicksTotales);
            GuardarJugador(jugadorB2, 2, equipoB.clicksTotales);
        }
        else
        {
            GuardarJugador(jugadorB1, 1, equipoB.clicksTotales);
            GuardarJugador(jugadorB2, 1, equipoB.clicksTotales);

            GuardarJugador(jugadorA1, 2, equipoA.clicksTotales);
            GuardarJugador(jugadorA2, 2, equipoA.clicksTotales);
        }
    }

    private void GuardarJugador(PlayerClicker jugador, int posicion, int puntos)
    {
        if (jugador == null)
            return;

        int index = jugador.idJugador - 1;

        if (index < 0 || index >= DatosMinijuego.posiciones.Length)
            return;

        DatosMinijuego.ids[index] = jugador.idJugador;
        DatosMinijuego.posiciones[index] = posicion;
        DatosMinijuego.puntos[index] = puntos;
        DatosMinijuego.monedas[index] = 0;
        DatosMinijuego.estrellas[index] = 0;
    }

    private void DetenerJugadores()
    {
        if (jugadorA1 != null) jugadorA1.StopPlayer();
        if (jugadorA2 != null) jugadorA2.StopPlayer();
        if (jugadorB1 != null) jugadorB1.StopPlayer();
        if (jugadorB2 != null) jugadorB2.StopPlayer();
    }

    private void ActualizarUI()
    {
        if (textoEquipoA != null)
        {
            textoEquipoA.text = $"{equipoA.nombre}: {equipoA.clicksTotales}/{clicksParaGanar}";
        }

        if (textoEquipoB != null)
        {
            textoEquipoB.text = $"{equipoB.nombre}: {equipoB.clicksTotales}/{clicksParaGanar}";
        }

        if (textoTurnos != null)
        {
            string nombreTurnoA = turnoA != null ? turnoA.nombreJugador : "-";
            string nombreTurnoB = turnoB != null ? turnoB.nombreJugador : "-";

            textoTurnos.text = $"Turno A: {nombreTurnoA}\nTurno B: {nombreTurnoB}";
        }
    }
}