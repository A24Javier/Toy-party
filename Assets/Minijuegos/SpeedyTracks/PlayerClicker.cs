using System.Collections;
using UnityEngine;

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
}