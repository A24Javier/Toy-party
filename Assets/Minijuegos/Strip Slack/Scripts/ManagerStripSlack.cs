using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerStripSlack : MonoBehaviour
{
    [Header("Tiempo")]
    [SerializeField] private float timeJoc = 30f;
    [SerializeField] private TextMeshProUGUI TextTime;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaRecompensas = "NivelRecompensasMinijuegos";

    private List<PlayerControllerStripSlack> players = new List<PlayerControllerStripSlack>();

    private bool minijuegoFinalizado = false;

    [SerializeField] private ScalaPersonajes Mod;

    private void Awake()
    {
        if (Mod != null)
            Mod.InicializarMinijuego(1);
    }

    private IEnumerator Start()
    {
        yield return null;

        CargarDatosDesdePartySession();
    }

    public PlayerControllerStripSlack GetPlayer(int i)
    {
        return players[i];
    }

    public int GetCountPlayer()
    {
        return players.Count;
    }

    public void AddPlayer(PlayerControllerStripSlack player)
    {
        if (player != null && !players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void RemovePlayer(PlayerControllerStripSlack player)
    {
        if (player != null && players.Contains(player))
        {
            players.Remove(player);
        }
    }

    private void Update()
    {
        if (minijuegoFinalizado)
            return;

        TimeController();
    }

    private void ActualizarInterfaz()
    {
        if (TextTime == null)
            return;

        float minutos = Mathf.FloorToInt(timeJoc / 60);
        float segundos = Mathf.FloorToInt(timeJoc % 60);

        TextTime.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void TimeController()
    {
        if (timeJoc > 1)
        {
            timeJoc -= Time.deltaTime;
            ActualizarInterfaz();
        }
        else
        {
            minijuegoFinalizado = true;

            AsignarPosiciones();
            FinalizarMinijuego();
        }
    }

    public float GetTime()
    {
        return timeJoc;
    }

    private void CargarDatosDesdePartySession()
    {
        if (PartySession.instance == null || PartySession.instance.characters == null)
        {
            Debug.LogWarning("ManagerStripSlack: no existe PartySession o no hay characters.");
            AplicarFallbackInspector();
            return;
        }

        foreach (PlayerControllerStripSlack p in players)
        {
            if (p == null)
                continue;

            int slotIndex = p.player - 1;

            if (slotIndex < 0 || slotIndex >= PartySession.instance.characters.Length)
            {
                Debug.LogWarning($"ManagerStripSlack: player {p.player} fuera de rango.");
                continue;
            }

            CharacterSnapshot snap = PartySession.instance.characters[slotIndex];

            if (snap == null)
                continue;

            p.CargarDatosDesdeSnapshot(snap);
        }
    }

    private void AplicarFallbackInspector()
    {
        foreach (PlayerControllerStripSlack p in players)
        {
            if (p == null)
                continue;

            p.AplicarModeloVisual();
        }
    }

    private void AsignarPosiciones()
    {
        DatosMinijuego.ResetDatos();
        DatosMinijuego.cantidadJugadores = 2;
        DatosMinijuego.escenaRecompensas = nombreEscenaRecompensas;

        LimpiarResultadosDatosMinijuego();

        PlayerControllerStripSlack player1 = ObtenerPlayerPorSlot(1);
        PlayerControllerStripSlack player2 = ObtenerPlayerPorSlot(2);

        if (player1 == null || player2 == null)
        {
            Debug.LogError("Strip Slack necesita jugadores con player = 1 y player = 2.");
            return;
        }

        int idPlayer1 = ObtenerIDSeguro(player1, 1);
        int idPlayer2 = ObtenerIDSeguro(player2, 2);

        float fuerzaPlayer1 = player1.GetForce();
        float fuerzaPlayer2 = player2.GetForce();

        int posicionPlayer1;
        int posicionPlayer2;

        if (fuerzaPlayer1 > fuerzaPlayer2)
        {
            posicionPlayer1 = 1;
            posicionPlayer2 = 2;
        }
        else if (fuerzaPlayer2 > fuerzaPlayer1)
        {
            posicionPlayer1 = 2;
            posicionPlayer2 = 1;
        }
        else
        {
            posicionPlayer1 = 1;
            posicionPlayer2 = 1;
        }

        GuardarResultadoJugador(idPlayer1, posicionPlayer1, fuerzaPlayer1);
        GuardarResultadoJugador(idPlayer2, posicionPlayer2, fuerzaPlayer2);

        Debug.Log("Strip Slack finalizado:");
        Debug.Log($"Jugador {idPlayer1} fuerza={fuerzaPlayer1} posición={posicionPlayer1}");
        Debug.Log($"Jugador {idPlayer2} fuerza={fuerzaPlayer2} posición={posicionPlayer2}");
    }

    private PlayerControllerStripSlack ObtenerPlayerPorSlot(int slot)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null && players[i].player == slot)
                return players[i];
        }

        return null;
    }

    private void LimpiarResultadosDatosMinijuego()
    {
        for (int i = 0; i < 4; i++)
        {
            DatosMinijuego.ids[i] = 0;
            DatosMinijuego.posiciones[i] = 0;
            DatosMinijuego.puntos[i] = 0;
            DatosMinijuego.monedas[i] = 0;
            DatosMinijuego.estrellas[i] = 0;
        }
    }

    private int ObtenerIDSeguro(PlayerControllerStripSlack player, int idPorDefecto)
    {
        if (player == null)
            return idPorDefecto;

        if (player.id < 1 || player.id > 4)
            return idPorDefecto;

        return player.id;
    }

    private void GuardarResultadoJugador(int idJugador, int posicion, float fuerza)
    {
        int index = idJugador - 1;

        if (index < 0 || index >= DatosMinijuego.posiciones.Length)
            return;

        DatosMinijuego.ids[index] = idJugador;
        DatosMinijuego.posiciones[index] = posicion;
        DatosMinijuego.puntos[index] = Mathf.RoundToInt(fuerza);
        DatosMinijuego.monedas[index] = 0;
        DatosMinijuego.estrellas[index] = 0;
    }

    private void FinalizarMinijuego()
    {
        if (MinigameController.instance == null)
        {
            Debug.LogError("ManagerStripSlack: no existe MinigameController. No se puede abrir la escena de recompensas.");
            return;
        }

        MinigameController.instance.OpenRewardScene();
    }
}