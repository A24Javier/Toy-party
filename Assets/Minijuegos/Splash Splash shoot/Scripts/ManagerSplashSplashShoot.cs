using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerSplashSplashShoot : MonoBehaviour
{
    [Header("Tiempo")]
    public float timeGame = 45f;
    [SerializeField] private TextMeshProUGUI TimeHud;

    [Header("Jugadores")]
    private List<PlayerControllerSplashSplashShoot> Players = new List<PlayerControllerSplashSplashShoot>();
    private List<PlayerControllerSplashSplashShoot> TodosLosPlayers = new List<PlayerControllerSplashSplashShoot>();

    [Header("Gotas")]
    private List<Transform> GotasActivas = new List<Transform>();
    [SerializeField] private GameObject GotaPrime;
    [SerializeField] private float timeRespawnMax = 5f;
    private float timeRespawn = 5f;

    [Header("UI Munición")]
    public Sprite[] Mun;

    [Header("Audio")]
    [SerializeField] private AudioSource MusicGame;

    private Vector3[] PosicionObjeto =
    {
        new Vector3(-26f, 12.6f, 20f),
        new Vector3(-26f, 12.6f, -6f),
        new Vector3(42.4f, 12.6f, -6f),
        new Vector3(42.4f, 12.6f, 20f)
    };

    public bool GameTerminated;

    private bool finalizando = false;
    private int siguientePosicionEliminado = 4;

    private void Start()
    {
        GameTerminated = false;
        finalizando = false;
        siguientePosicionEliminado = 4;
    }

    private void Update()
    {
        TimeController();
    }

    public void RegistrarGota(Transform gota)
    {
        if (gota != null && !GotasActivas.Contains(gota))
            GotasActivas.Add(gota);
    }

    public void EliminarGota(Transform gota)
    {
        if (gota != null && GotasActivas.Contains(gota))
            GotasActivas.Remove(gota);
    }

    private void InvokeGota()
    {
        if (GotaPrime == null)
            return;

        if (timeRespawn > 0)
        {
            timeRespawn -= Time.deltaTime;
        }
        else
        {
            timeRespawn = timeRespawnMax;
            Vector3 randomPos = PosicionRandom();
            GameObject nuevaGota = Instantiate(GotaPrime, randomPos, Quaternion.identity);
            RegistrarGota(nuevaGota.transform);
        }
    }

    public Vector3 PosicionRandom()
    {
        return new Vector3(
            Random.Range(PosicionObjeto[0].x, PosicionObjeto[3].x),
            PosicionObjeto[0].y,
            Random.Range(PosicionObjeto[1].z, PosicionObjeto[3].z)
        );
    }

    public Transform ObjectivoIA(PlayerControllerSplashSplashShoot player, Transform ignorar = null)
    {
        if (player == null)
            return null;

        Vector3 posicionIA = player.transform.position;

        if (player.GetInfo().GetMunicion() <= 5)
        {
            return BuscarMasCercano(posicionIA, GotasActivas, ignorar);
        }

        Transform jugadorObjetivo = null;
        float distMin = Mathf.Infinity;

        foreach (PlayerControllerSplashSplashShoot p in Players)
        {
            if (p == null)
                continue;

            if (p == player || p.transform == ignorar)
                continue;

            float distancia = Vector3.Distance(p.transform.position, posicionIA);

            if (distancia < distMin)
            {
                distMin = distancia;
                jugadorObjetivo = p.transform;
            }
        }

        return jugadorObjetivo;
    }

    private Transform BuscarMasCercano(Vector3 origen, List<Transform> lista, Transform ignorar = null)
    {
        if (lista.Count == 0)
            return null;

        Transform masCercano = null;
        float minDist = Mathf.Infinity;

        for (int i = lista.Count - 1; i >= 0; i--)
        {
            if (lista[i] == null)
            {
                lista.RemoveAt(i);
                continue;
            }

            if (lista[i] == ignorar)
                continue;

            float dist = Vector3.Distance(origen, lista[i].position);

            if (dist < minDist)
            {
                minDist = dist;
                masCercano = lista[i];
            }
        }

        return masCercano;
    }

    public void AddPlayer(PlayerControllerSplashSplashShoot player)
    {
        if (player == null)
            return;

        if (!Players.Contains(player))
            Players.Add(player);

        if (!TodosLosPlayers.Contains(player))
            TodosLosPlayers.Add(player);
    }

    public void RemovePlayer(PlayerControllerSplashSplashShoot player)
    {
        if (player == null)
            return;

        Players.Remove(player);
    }

    public int GetPLayerListCount()
    {
        return Players.Count;
    }

    public void PlayerEliminado(PlayerControllerSplashSplashShoot player)
    {
        if (player == null || player.GetInfo() == null)
            return;

        if (!Players.Contains(player))
            return;

        player.GetInfo().SetEliminado(true);
        player.GetInfo().SetPosicionFinal(siguientePosicionEliminado);

        siguientePosicionEliminado--;

        Players.Remove(player);

        UltimoJugador();
    }

    private void TimeController()
    {
        if (GameTerminated || finalizando)
            return;

        if (timeGame > 1)
        {
            InvokeGota();
            timeGame -= Time.deltaTime;
            UpdateHudTime(timeGame);
            UltimoJugador();
        }
        else
        {
            timeGame = 0;
            GameTerminated = true;

            if (MusicGame != null)
                MusicGame.Stop();

            AsignarPosicionesPorTiempo();
            FinalizarMinijuego();
        }
    }

    private void UpdateHudTime(float time)
    {
        if (TimeHud == null)
            return;

        float minutos = Mathf.FloorToInt(time / 60);
        float segundos = Mathf.FloorToInt(time % 60);

        TimeHud.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void UltimoJugador()
    {
        if (GameTerminated || finalizando)
            return;

        if (Players.Count == 1)
        {
            PlayerControllerSplashSplashShoot ganador = Players[0];

            if (ganador != null && ganador.GetInfo() != null)
                ganador.GetInfo().SetPosicionFinal(1);

            GameTerminated = true;

            if (MusicGame != null)
                MusicGame.Stop();

            FinalizarMinijuego();
        }
    }

    private void AsignarPosicionesPorTiempo()
    {
        List<PlayerControllerSplashSplashShoot> vivos = new List<PlayerControllerSplashSplashShoot>();

        foreach (PlayerControllerSplashSplashShoot p in Players)
        {
            if (p != null && p.GetInfo() != null && !p.GetInfo().GetEliminado())
                vivos.Add(p);
        }

        vivos.Sort((a, b) =>
        {
            int lifeCompare = b.GetInfo().GetLife().CompareTo(a.GetInfo().GetLife());

            if (lifeCompare != 0)
                return lifeCompare;

            return b.GetInfo().GetMunicion().CompareTo(a.GetInfo().GetMunicion());
        });

        int posicion = 1;

        foreach (PlayerControllerSplashSplashShoot p in vivos)
        {
            if (p.GetInfo().GetPosicionFinal() == 0)
            {
                p.GetInfo().SetPosicionFinal(posicion);
                posicion++;
            }
        }

        foreach (PlayerControllerSplashSplashShoot p in TodosLosPlayers)
        {
            if (p == null || p.GetInfo() == null)
                continue;

            if (p.GetInfo().GetPosicionFinal() == 0)
            {
                p.GetInfo().SetPosicionFinal(posicion);
                posicion++;
            }
        }
    }

    private void FinalizarMinijuego()
    {
        if (finalizando)
            return;

        finalizando = true;
        GameTerminated = true;

        PrepararDatosRecompensas();

        if (MinigameController.instance != null)
        {
            MinigameController.instance.OpenRewardScene();
        }
        else
        {
            Debug.LogError("ManagerSplashSplashShoot: no existe MinigameController. No se puede abrir NivelRecompensasMinijuegos.");
        }
    }

    private void PrepararDatosRecompensas()
    {
        AsegurarArraysDatosMinijuego();

        List<PlayerControllerSplashSplashShoot> ordenados = new List<PlayerControllerSplashSplashShoot>();

        foreach (PlayerControllerSplashSplashShoot p in TodosLosPlayers)
        {
            if (p != null && p.GetInfo() != null)
                ordenados.Add(p);
        }

        ordenados.Sort((a, b) =>
        {
            int posA = a.GetInfo().GetPosicionFinal();
            int posB = b.GetInfo().GetPosicionFinal();

            if (posA == 0)
                posA = 99;

            if (posB == 0)
                posB = 99;

            return posA.CompareTo(posB);
        });

        for (int i = 0; i < ordenados.Count && i < 4; i++)
        {
            PlayerInfoSplash info = ordenados[i].GetInfo();

            int index = info.GetID() - 1;

            if (index < 0 || index >= 4)
                index = i;

            DatosMinijuego.posiciones[index] = info.GetPosicionFinal();
            DatosMinijuego.estrellas[index] = 0;
            DatosMinijuego.monedas[index] = 0;
        }

        Debug.Log("ManagerSplashSplashShoot: datos preparados para NivelRecompensasMinijuegos.");
    }

    private void AsegurarArraysDatosMinijuego()
    {
        if (DatosMinijuego.posiciones == null || DatosMinijuego.posiciones.Length < 4)
            DatosMinijuego.posiciones = new int[4];

        if (DatosMinijuego.estrellas == null || DatosMinijuego.estrellas.Length < 4)
            DatosMinijuego.estrellas = new int[4];

        if (DatosMinijuego.monedas == null || DatosMinijuego.monedas.Length < 4)
            DatosMinijuego.monedas = new int[4];
    }
}