using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class RaceController : MonoBehaviour
{
    [Header("Start race cooldown")]
    [SerializeField]
    [Range(1, 10)]
    private int _cooldownSeconds = 3;

    [SerializeField]
    private TMP_Text _textCooldown;

    [SerializeField]
    private LocalizedString _startLocalized;

    [Space(10)]

    [Header("Camera player show")]
    [SerializeField]
    private Vector3 _relayCamOffset;

    [SerializeField] private Camera _cameraEquipo1;
    [SerializeField] private Camera _cameraEquipo2;

    [SerializeField] private float _distanceBetweenTracks = 20f;

    [Header("Posiciones de Relevos por Equipo")]
    [SerializeField] private Transform[] _relayPositionsEquipo1;
    [SerializeField] private Transform[] _relayPositionsEquipo2;
    [SerializeField]
    [Range(0.25f, 8f)]
    private float _secondsWaitingInRelay = 1.5f;

    [SerializeField]
    [Range(0.1f, 30f)]
    private float _cameraSpeed = 10f;

    [Header("Other")]
    public static RaceController Instance;

    [SerializeField] ScalaPersonajes Mod;

    List<PlayerRunController> players = new List<PlayerRunController>();

    public List<PlayerRunController> Equipo1 = new List<PlayerRunController>();

    public List<PlayerRunController> Equipo2 = new List<PlayerRunController>();

    [Header("Escenas")]
    [SerializeField] string nombreEscenaRecompensas = "NivelRecompensasMinijuegos";

    public void AñadirJugador(PlayerRunController player)
    {
        players.Add(player);
        
    }

    void Awake()
    {
        _textCooldown.text = "";
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Mod.InicializarMinijuego(4);
    }

    private void Start()
    {
        int miembroEq1 = 1;
        int miembroEq2 = 1;
        // Recorremos la lista hacia atrás para que RemoveAt no rompa el índice
        for (int i = players.Count - 1; i >= 0; i--)
        {
            // Si ya llenamos ambos equipos (2+2=4), dejamos de buscar
            if (Equipo1.Count >= 2 && Equipo2.Count >= 2) break;

            // Lógica de asignación
            if (Equipo1.Count < 2)
            {
                players[i].GetInfoJugador().equipo = 1;
                players[i].GetInfoJugador().miembro = miembroEq1;
                Equipo1.Add(players[i]);
                miembroEq1++;
            }
            else if (Equipo2.Count < 2)
            {
                players[i].GetInfoJugador().equipo = 2;
                players[i].GetInfoJugador().miembro = miembroEq2;
                Equipo2.Add(players[i]);
                miembroEq2++;
            }
        }
        players.Clear();
        EmpezarCarrera(Equipo1, _cameraEquipo1);
        EmpezarCarrera(Equipo2, _cameraEquipo2);
    }

    void EmpezarCarrera(List<PlayerRunController> eq,Camera c)
    {
        //Main Camera que siga al Jugador con el Player 1
        for (int i = 0; i < eq.Count; i++)
        {
            if (eq[i] != null)
            {
                if (eq[i].GetInfoJugador().miembro == 1)
                {
                    eq[i].GetInfoJugador().CanMove = true;
                    SeguirPlayer(eq[i],c);
                }
            }
        }
    }

    void SeguirPlayer(PlayerRunController run, Camera c)
    {
        if (c != null)
        {
            // Buscamos el script en la Main Camera
            CameraSeguir scriptCamara = c.GetComponent<CameraSeguir>();

            if (scriptCamara != null)
            {
                // Le asignamos el transform del jugador para que empiece a seguirlo
                scriptCamara.target = run.transform;
            }
            else
            {
                Debug.LogError("¡Falta el script CameraFollow en la Main Camera!");
            }
        }
    }

    public void Relevo(int eq)
    {
        if (eq == 1)
            CorrerRelevo(Equipo1,_cameraEquipo1);
        else if (eq == 2)
            CorrerRelevo(Equipo2, _cameraEquipo2);
    }

    void CorrerRelevo(List<PlayerRunController> eq, Camera c)
    {
        PlayerRunController jugadorAnterior = null;
        PlayerRunController jugadorNuevo = null;


        for (int i = 0; i < eq.Count; i++)
        {
            if (eq[i] != null)
            { 
                if (eq[i].GetInfoJugador().miembro == 1)
                {
                    jugadorAnterior = eq[i];
                }
                else if (eq[i].GetInfoJugador().miembro == 2)
                {
                    jugadorNuevo = eq[i];
                }
            }
        }

        if (jugadorNuevo != null)
        {
            if (jugadorAnterior != null)
            {
                jugadorAnterior.GetInfoJugador().CanMove = false;
                Debug.Log("[Relevo] Player 1 detenido.");
            }

            jugadorNuevo.GetInfoJugador().CanMove = true;
            SeguirPlayer(jugadorNuevo,c);
            Debug.Log("[Relevo] Player 2 activado y cámara asignada.");
        }
        else
        {
            // SI SALE ESTE ERROR: El problema es que no encuentra al Player 2 en la lista
            Debug.LogError("[Relevo] ERROR: No se encontró ningún jugador con player == 2 en la lista.");
        }
    }

    void AssignarPoscion(List<PlayerRunController> eq)
    {
        if (eq[0].GetInfoJugador().equipo == 1)
        {
            for (int i = 0; i < Equipo1.Count; i++)
            {
                Equipo1[i].GetInfoJugador().pos = 1;
                players.Add(Equipo1[i]);
            }
            for (int i = 0; i < Equipo2.Count; i++)
            {
                Equipo2[i].GetInfoJugador().pos = 2;
                players.Add(Equipo2[i]);
            }
        }
        else
        {
            for (int i = 0;  i < Equipo2.Count; i++)
            {
                Equipo2[i].GetInfoJugador().pos = 1;
                players.Add (Equipo2[i]);
            }
            for (int i = 0; i < Equipo1.Count; i++)
            {
                Equipo1[i].GetInfoJugador().pos = 2;
                players.Add(Equipo1[i]);
            }
        }
    }

    public void FinalizarPartida(int eq)
    {
        if (eq == 1)
            AssignarPoscion(Equipo1);
        if (eq == 2)
            AssignarPoscion(Equipo2);

        GuardarResultadosEnDatosMinijuego();

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

    void GuardarResultadosEnDatosMinijuego()
    {
        DatosMinijuego.ResetDatos();
        DatosMinijuego.cantidadJugadores = 4;

        for (int i = 0; i < players.Count; i++)
        {
            int idJugador = players[i].GetInfoJugador().GetID();
            int posicion = players[i].GetInfoJugador().pos;

            int index = idJugador - 1;

            if (index < 0 || index >= DatosMinijuego.posiciones.Length)
                continue;

            DatosMinijuego.ids[index] = idJugador;
            DatosMinijuego.posiciones[index] = posicion;
            DatosMinijuego.puntos[index] = 5 - posicion;
            DatosMinijuego.monedas[index] = 0;
            DatosMinijuego.estrellas[index] = 0;

            Debug.Log($"DatosMinijuego: Player{idJugador} posición={posicion}");
        }
    }
}