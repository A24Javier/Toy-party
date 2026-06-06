using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SillaASevillaManager : MonoBehaviour
{
    enum Statjoc { StartRound, Play, InfoPlay, ChooseChair, Eleminated }

    Statjoc MyStat;

    [Header("HUD")]
    [SerializeField] TextMeshProUGUI MyTextronda;
    [SerializeField] GameObject[] HudObject;
    [SerializeField] Image[] ImgHud;

    [Header("Sillas")]
    [SerializeField] ChairController[] MisSillas;

    [Header("Timeline")]
    [SerializeField] ManagerTimeLine timeLine;

    [Header("Escenas")]
    [SerializeField] string nombreEscenaRecompensas = "NivelRecompensasMinijuegos";

    string Textronda;
    int ronda;

    float timeRonda;
    float timeplay;
    float timeinfoplaying;
    float timeChoose;
    float timeeleminated;

    List<PlayerControllerSillaaSevilla> MisJugadores = new List<PlayerControllerSillaaSevilla>();
    List<GameObject> ObjectoJugador = new List<GameObject>();

    List<PlayerSillaaSevilla> MiRegstroEleminados = new List<PlayerSillaaSevilla>();

    bool minijuegoFinalizado = false;

    private void Start()
    {
        MyStat = Statjoc.StartRound;

        Textronda = "Ronda ";
        ronda = 1;

        timeRonda = 1.5f;
        timeplay = 15.0f;
        timeinfoplaying = 1.5f;
        timeChoose = 5.0f;
        timeeleminated = 1.0f;

        for (int i = 0; i < HudObject.Length; i++)
        {
            if (HudObject[i] != null)
                HudObject[i].SetActive(false);
        }

        if (timeLine != null)
        {
            timeLine.PausarAnimation();
            timeLine.DesactivarMusicSpecial();
        }
    }

    public void RegistraJugador(PlayerControllerSillaaSevilla player)
    {
        if (player == null)
            return;

        if (!MisJugadores.Contains(player))
            MisJugadores.Add(player);

        if (!ObjectoJugador.Contains(player.gameObject))
            ObjectoJugador.Add(player.gameObject);
    }

    public void DesregistrarJugador(PlayerControllerSillaaSevilla player)
    {
        if (player == null)
            return;

        MisJugadores.Remove(player);
        ObjectoJugador.Remove(player.gameObject);
    }

    private void FixedUpdate()
    {
        if (minijuegoFinalizado)
            return;

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

    void ResetPosPlayers()
    {
        for (int i = 0; i < MisJugadores.Count; i++)
        {
            if (MisJugadores[i] != null)
                MisJugadores[i].PosPlayer();
        }

        for (int i = 0; i < MisSillas.Length; i++)
        {
            if (MisSillas[i] != null)
                MisSillas[i].Liberar();
        }
    }

    void RunNewRonda()
    {
        ActivarHud(0, true);

        switch (ronda)
        {
            case 1:
                if (MyTextronda != null)
                    MyTextronda.text = Textronda + ronda;
                break;

            case 2:
                if (MyTextronda != null)
                    MyTextronda.text = Textronda + ronda;

                // Segunda ronda: quedan 3 jugadores, dejamos 2 sillas.
                if (MisSillas.Length > 0 && MisSillas[0] != null)
                    MisSillas[0].gameObject.SetActive(false);

                ResetPosPlayers();
                break;

            case 3:
                if (MyTextronda != null)
                    MyTextronda.text = "Última Ronda";

                // Final: quedan 2 jugadores, dejamos 1 silla.
                if (MisSillas.Length > 0 && MisSillas[0] != null)
                    MisSillas[0].gameObject.SetActive(true);

                if (MisSillas.Length > 1 && MisSillas[1] != null)
                    MisSillas[1].gameObject.SetActive(false);

                if (MisSillas.Length > 2 && MisSillas[2] != null)
                    MisSillas[2].gameObject.SetActive(false);

                ResetPosPlayers();
                break;
        }

        if (timeRonda > 0)
        {
            timeRonda -= Time.deltaTime;
        }
        else
        {
            timeRonda = 1.5f;

            if (timeLine != null)
                timeLine.RenaudarAnimation();

            MyStat = Statjoc.Play;
        }
    }

    void RunPlay()
    {
        ActivarHud(0, false);

        if (timeplay > 0)
        {
            timeplay -= Time.deltaTime;
        }
        else
        {
            timeplay = 15.0f;

            if (timeLine != null)
                timeLine.PausarAnimation();

            MyStat = Statjoc.InfoPlay;
        }
    }

    void InfoPlaying()
    {
        if (timeLine != null)
            timeLine.ActivarMusicSpecial();

        ActivarHud(0, true);

        if (MyTextronda != null)
            MyTextronda.text = "Escoge una silla";

        if (timeinfoplaying > 0)
        {
            timeinfoplaying -= Time.deltaTime;
        }
        else
        {
            timeinfoplaying = 1.5f;

            ActivarHud(0, false);

            MyStat = Statjoc.ChooseChair;
        }
    }

    void RunInputs()
    {
        for (int i = 0; i < MisJugadores.Count; i++)
        {
            if (MisJugadores[i] != null)
                MisJugadores[i].MoveEnable();
        }

        if (timeChoose > 0)
        {
            timeChoose -= Time.deltaTime;
        }
        else
        {
            timeChoose = 5.0f;

            if (timeLine != null)
                timeLine.DesactivarMusicSpecial();

            MyStat = Statjoc.Eleminated;
        }
    }

    void RunEleminated()
    {
        for (int i = 0; i < MisJugadores.Count; i++)
        {
            if (MisJugadores[i] != null)
                MisJugadores[i].MoveDisabled();
        }

        EliminarJugadoresSinSilla();

        if (timeeleminated > 0)
        {
            timeeleminated -= Time.deltaTime;
        }
        else
        {
            timeeleminated = 1.0f;

            if (MisJugadores.Count <= 1 || ronda >= 3)
            {
                RegistrarGanadorSiExiste();
                FinalizarMinijuego();
                return;
            }

            ronda++;
            MyStat = Statjoc.StartRound;
        }
    }

    void EliminarJugadoresSinSilla()
    {
        for (int i = MisJugadores.Count - 1; i >= 0; i--)
        {
            PlayerControllerSillaaSevilla jugador = MisJugadores[i];

            if (jugador == null)
            {
                MisJugadores.RemoveAt(i);
                ObjectoJugador.RemoveAt(i);
                continue;
            }

            if (!jugador.Sit)
            {
                int posicionFinal = ObtenerPosicionFinalSegunRonda(ronda);

                MostrarHudEliminado(jugador.GetId());

                RegistrarEliminado(jugador.GetId(), posicionFinal);

                GameObject obj = jugador.gameObject;

                MisJugadores.RemoveAt(i);
                ObjectoJugador.RemoveAt(i);

                Destroy(obj);
            }
        }
    }

    int ObtenerPosicionFinalSegunRonda(int rondaActual)
    {
        switch (rondaActual)
        {
            case 1:
                return 4;

            case 2:
                return 3;

            case 3:
                return 2;

            default:
                return 4;
        }
    }

    void RegistrarEliminado(int idJugador, int posicionFinal)
    {
        if (idJugador < 1 || idJugador > 4)
            return;

        MiRegstroEleminados.Add(new PlayerSillaaSevilla(idJugador, posicionFinal));

        Debug.Log($"Jugador {idJugador} eliminado. Posición final: {posicionFinal}");
    }

    void RegistrarGanadorSiExiste()
    {
        if (MisJugadores.Count == 1 && MisJugadores[0] != null)
        {
            int idGanador = MisJugadores[0].GetId();

            bool yaRegistrado = false;

            for (int i = 0; i < MiRegstroEleminados.Count; i++)
            {
                if (MiRegstroEleminados[i].VerId() == idGanador)
                {
                    yaRegistrado = true;
                    break;
                }
            }

            if (!yaRegistrado)
            {
                MiRegstroEleminados.Add(new PlayerSillaaSevilla(idGanador, 1));
                Debug.Log($"Jugador {idGanador} gana Silla a Sevilla.");
            }
        }
        else if (MisJugadores.Count == 0)
        {
            Debug.LogWarning("No queda ningún jugador vivo. Se finalizará usando solo los eliminados registrados.");
        }
    }

    void MostrarHudEliminado(int idJugador)
    {
        switch (idJugador)
        {
            case 1:
                ActivarHud(1, true);
                break;

            case 2:
                ActivarHud(2, true);
                break;

            case 3:
                ActivarHud(3, true);
                break;

            case 4:
                ActivarHud(4, true);
                break;
        }
    }

    void ActivarHud(int index, bool activo)
    {
        if (HudObject == null)
            return;

        if (index < 0 || index >= HudObject.Length)
            return;

        if (HudObject[index] != null)
            HudObject[index].SetActive(activo);
    }

    void GuardarResultadosEnDatosMinijuego()
    {
        DatosMinijuego.ResetDatos();
        DatosMinijuego.cantidadJugadores = 4;

        for (int i = 0; i < MiRegstroEleminados.Count; i++)
        {
            int idJugador = MiRegstroEleminados[i].VerId();
            int posicion = MiRegstroEleminados[i].VerPos();

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

    void FinalizarMinijuego()
    {
        if (minijuegoFinalizado)
            return;

        minijuegoFinalizado = true;

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
}