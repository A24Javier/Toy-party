using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerFinMinijuego : MonoBehaviour
{
    public enum TipoMiniGame { unoVSuno, OtherMinigames }
    public TipoMiniGame minigame;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaRecompensas = "NivelRecompensasMinijuegos";

    [Header("General")]
    [SerializeField] private GameObject GlobalHUD;
    [SerializeField] private GameObject[] playersScena;
    [SerializeField] private GameObject[] huds;

    [Header("Minijuegos 1 vs 1")]
    [Header("Player1")]
    [SerializeField] private TextMeshProUGUI PosPlayer1;
    [SerializeField] private TextMeshProUGUI CanitdadEstrellaPlayer1;
    [SerializeField] private TextMeshProUGUI CantidadMonedasPlayer1;

    [Header("Player2")]
    [SerializeField] private TextMeshProUGUI PosPlayer2;
    [SerializeField] private TextMeshProUGUI CanitdadEstrellaPlayer2;
    [SerializeField] private TextMeshProUGUI CantidadMonedasPlayer2;

    [Header("Minijuegos de 4 jugadores")]
    [Header("Player1")]
    [SerializeField] private TextMeshProUGUI Player1Pos;
    [SerializeField] private TextMeshProUGUI Player1CantEstrellas;
    [SerializeField] private TextMeshProUGUI Player1CantMoney;

    [Header("Player2")]
    [SerializeField] private TextMeshProUGUI Player2Pos;
    [SerializeField] private TextMeshProUGUI Player2CantEstrellas;
    [SerializeField] private TextMeshProUGUI Player2CantMoney;

    [Header("Player3")]
    [SerializeField] private TextMeshProUGUI Player3Pos;
    [SerializeField] private TextMeshProUGUI Player3CantEstrellas;
    [SerializeField] private TextMeshProUGUI Player3CantMoney;

    [Header("Player4")]
    [SerializeField] private TextMeshProUGUI Player4Pos;
    [SerializeField] private TextMeshProUGUI Player4CantEstrellas;
    [SerializeField] private TextMeshProUGUI Player4CantMoney;

    [Header("Moneda")]
    [SerializeField] private GameObject Moneda;

    [Header("PositionMoneda")]
    [SerializeField] private Transform MonedaPosPlayer1;
    [SerializeField] private Transform MonedaPosPlayer2;
    [SerializeField] private Transform MonedaPosPlayer3;
    [SerializeField] private Transform MonedaPosPlayer4;

    [Header("Tiempo")]
    [SerializeField] private float tiempoAntesDeVolver = 3f;

    private int[] recompensas = { 5, 4, 2, 1 };

    private List<PlayerRecompensas> players = new List<PlayerRecompensas>();

    private bool recompensasCompletas = false;

    public void AñadirRec(PlayerRecompensas playerR)
    {
        if (playerR != null && !players.Contains(playerR))
        {
            players.Add(playerR);
        }
    }

    public void RemoveRec(PlayerRecompensas playerR)
    {
        if (playerR != null)
        {
            players.Remove(playerR);
        }
    }

    private void Start()
    {
        StartCoroutine(FlujoFinMinijuego());
    }

    private IEnumerator FlujoFinMinijuego()
    {
        PrepararEscena();

        yield return null;

        ReconstruirListaJugadores();
        CogerInfo();

        if (minigame == TipoMiniGame.unoVSuno)
        {
            OneStat();
        }
        else
        {
            FourStat();
        }

        CargarInfo();

        yield return StartCoroutine(RepartirRecompensas());

        recompensasCompletas = true;

        yield return new WaitForSeconds(tiempoAntesDeVolver);

        VolverAlJuegoBase();
    }

    private void PrepararEscena()
    {
        if (GlobalHUD != null)
        {
            GlobalHUD.SetActive(true);
        }

        for (int i = 0; i < huds.Length; i++)
        {
            if (huds[i] != null)
            {
                huds[i].SetActive(false);
            }
        }

        if (minigame == TipoMiniGame.unoVSuno)
        {
            if (playersScena.Length > 0 && playersScena[0] != null)
            {
                playersScena[0].SetActive(true);
            }

            if (playersScena.Length > 1 && playersScena[1] != null)
            {
                playersScena[1].SetActive(true);
            }

            if (playersScena.Length > 2 && playersScena[2] != null)
            {
                playersScena[2].SetActive(false);
            }

            if (playersScena.Length > 3 && playersScena[3] != null)
            {
                playersScena[3].SetActive(false);
            }

            if (huds.Length > 0 && huds[0] != null)
            {
                huds[0].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < playersScena.Length; i++)
            {
                if (playersScena[i] != null)
                {
                    playersScena[i].SetActive(true);
                }
            }

            if (huds.Length > 1 && huds[1] != null)
            {
                huds[1].SetActive(true);
            }
        }
    }

    private void ReconstruirListaJugadores()
    {
        players.Clear();

        for (int i = 0; i < playersScena.Length; i++)
        {
            if (playersScena[i] == null)
                continue;

            if (!playersScena[i].activeInHierarchy)
                continue;

            PlayerRecompensas playerRec = playersScena[i].GetComponent<PlayerRecompensas>();

            if (playerRec != null && !players.Contains(playerRec))
            {
                players.Add(playerRec);
            }
        }
    }

    private void CogerInfo()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int indexJugador = i;

            players[i].id = indexJugador + 1;
            players[i].playerPos = "Player" + (indexJugador + 1);

            if (indexJugador >= 0 && indexJugador < DatosMinijuego.posiciones.Length)
            {
                players[i].posicion = DatosMinijuego.posiciones[indexJugador];
                players[i].estrellas = DatosMinijuego.estrellas[indexJugador];
                players[i].moneda = DatosMinijuego.monedas[indexJugador];
            }
            else
            {
                players[i].posicion = indexJugador + 1;
                players[i].estrellas = 0;
                players[i].moneda = 0;
            }

            players[i].Recompensa = false;

            Debug.Log(
                $"Recompensas cargadas para {players[i].playerPos}: " +
                $"posición={players[i].posicion}, " +
                $"estrellas={players[i].estrellas}, " +
                $"monedas iniciales={players[i].moneda}"
            );
        }
    }

    private void OneStat()
    {
        Debug.Log("Mostrando recompensa de minijuego 1 vs 1");
    }

    private void FourStat()
    {
        Debug.Log("Mostrando recompensa de minijuego de 4 jugadores");
    }

    private void CargarInfo()
    {
        if (minigame == TipoMiniGame.unoVSuno)
        {
            if (players.Count < 2)
            {
                Debug.LogError("No hay suficientes jugadores registrados para un minijuego 1 vs 1.");
                return;
            }

            PosPlayer1.text = players[0].posicion.ToString();
            CanitdadEstrellaPlayer1.text = players[0].estrellas.ToString();
            CantidadMonedasPlayer1.text = players[0].moneda.ToString();

            PosPlayer2.text = players[1].posicion.ToString();
            CanitdadEstrellaPlayer2.text = players[1].estrellas.ToString();
            CantidadMonedasPlayer2.text = players[1].moneda.ToString();
        }
        else
        {
            if (players.Count < 4)
            {
                Debug.LogError("No hay suficientes jugadores registrados para un minijuego de 4 jugadores.");
                return;
            }

            Player1Pos.text = players[0].posicion.ToString();
            Player1CantEstrellas.text = players[0].estrellas.ToString();
            Player1CantMoney.text = players[0].moneda.ToString();

            Player2Pos.text = players[1].posicion.ToString();
            Player2CantEstrellas.text = players[1].estrellas.ToString();
            Player2CantMoney.text = players[1].moneda.ToString();

            Player3Pos.text = players[2].posicion.ToString();
            Player3CantEstrellas.text = players[2].estrellas.ToString();
            Player3CantMoney.text = players[2].moneda.ToString();

            Player4Pos.text = players[3].posicion.ToString();
            Player4CantEstrellas.text = players[3].estrellas.ToString();
            Player4CantMoney.text = players[3].moneda.ToString();
        }
    }

    private IEnumerator RepartirRecompensas()
    {
        for (int i = 0; i < players.Count; i++)
        {
            PlayerRecompensas jugador = players[i];

            if (jugador == null)
                continue;

            if (!jugador.Recompensa)
            {
                int monedasGanadas = ObtenerCantidadRecompensa(jugador.posicion);

                jugador.moneda += monedasGanadas;
                jugador.Recompensa = true;

                GuardarMonedasEnDatosMinijuego(jugador.id, jugador.moneda);
                AplicarMonedasAlTablero(jugador.id, monedasGanadas);

                Transform posMoneda = ObtenerPosicionMoneda(jugador.playerPos);

                if (posMoneda != null && Moneda != null)
                {
                    yield return StartCoroutine(InstanciarMonedasSecuencial(posMoneda, monedasGanadas));
                }

                Debug.Log($"{jugador.playerPos} recibe {monedasGanadas} monedas por quedar en posición {jugador.posicion}");
            }
        }

        CargarInfo();
    }

    private void GuardarMonedasEnDatosMinijuego(int idJugador, int monedasFinales)
    {
        int index = idJugador - 1;

        if (index < 0 || index >= DatosMinijuego.monedas.Length)
            return;

        DatosMinijuego.monedas[index] = monedasFinales;
    }

    private void AplicarMonedasAlTablero(int idJugador, int monedasGanadas)
    {
        if (GameController.instance == null)
        {
            Debug.LogWarning("ManagerFinMinijuego: no existe GameController. No se pudieron aplicar monedas al tablero.");
            return;
        }

        Character character = GameController.instance.GetCharacter(idJugador - 1);

        if (character == null)
        {
            Debug.LogWarning("ManagerFinMinijuego: no se encontró Character con índice " + (idJugador - 1));
            return;
        }

        character.SetCoins(character.GetCoins() + monedasGanadas);
    }

    private int ObtenerCantidadRecompensa(int posicion)
    {
        if (posicion < 1 || posicion > recompensas.Length)
            return 0;

        return recompensas[posicion - 1];
    }

    private Transform ObtenerPosicionMoneda(string playernom)
    {
        switch (playernom)
        {
            case "Player1": return MonedaPosPlayer1;
            case "Player2": return MonedaPosPlayer2;
            case "Player3": return MonedaPosPlayer3;
            case "Player4": return MonedaPosPlayer4;
            default: return null;
        }
    }

    private IEnumerator InstanciarMonedasSecuencial(Transform spawnPoint, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Instantiate(Moneda, spawnPoint.position, Moneda.transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Lluvia de monedas terminada");
    }

    private void VolverAlJuegoBase()
    {
        if (!recompensasCompletas)
            return;

        if (MinigameController.instance != null)
        {
            MinigameController.instance.EndRewardsAndReturnToBoard();
        }
        else
        {
            Debug.LogWarning("ManagerFinMinijuego: no existe MinigameController. Cerrando escena de recompensas manualmente.");
            CerrarEscenaAditiva();
        }
    }

    private void CerrarEscenaAditiva()
    {
        Scene escenaActual = gameObject.scene;

        if (escenaActual.IsValid() && escenaActual.isLoaded)
        {
            SceneManager.UnloadSceneAsync(escenaActual);
        }
        else
        {
            SceneManager.UnloadSceneAsync(nombreEscenaRecompensas);
        }
    }
}