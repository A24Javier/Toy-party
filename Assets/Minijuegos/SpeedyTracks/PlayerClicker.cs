using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerClicker : MonoBehaviour
{
    [Header("Datos")]
    public string nombreJugador;
    public int idJugador = 1;
    public bool esNPC;

    [Header("Input")]
    public KeyCode key = KeyCode.Space;

    [Header("Equipo")]
    public TeamId equipoId;
    public Clicker2v2Manager manager;

    [Header("NPC")]
    public float npcMinTiempo = 0.5f;
    public float npcMaxTiempo = 2f;

    private bool puedeJugar = true;

    [Header("Modelo")]
    [SerializeField] ModelController MiModelo;
    public int idPerosnaje;
    public int player;
    Image img;
    private void Start()
    {
        if (esNPC)
        {
            StartCoroutine(NPCRutina());
        }
    }

    private void Update()
    {
        if (!puedeJugar)
            return;

        if (manager == null)
            return;

        if (manager.GameTerminated)
            return;

        if (!esNPC && Input.GetKeyDown(key))
        {
            manager.RegistrarClick(this);
        }
    }

    private IEnumerator NPCRutina()
    {
        while (puedeJugar)
        {
            yield return new WaitForSeconds(Random.Range(npcMinTiempo, npcMaxTiempo));

            if (manager == null)
                continue;

            if (manager.GameTerminated)
                yield break;

            manager.RegistrarClick(this);
        }
    }

    public void StopPlayer()
    {
        puedeJugar = false;
    }

    public void CargarDatosDesdeSnapshot(CharacterSnapshot snap)
    {
        if (snap == null)
            return;

        idJugador = snap.characterId + 1;
        esNPC = !snap.isPlayer;
        idPerosnaje = snap.characterSettingIndex;

        AplicarModeloVisual();
         
        Debug.Log(
            $"Clicker slot {player}: " +
            $"idJugador={idJugador}, " +
            $"skin={idPerosnaje}, " +
            $"esNPC={esNPC}"
        );
    }

    public void AplicarModeloVisual()
    {
        if (MiModelo != null)
        {
            MiModelo.AsignarModeloAJugador(idPerosnaje, player, img);
        }
    }
}