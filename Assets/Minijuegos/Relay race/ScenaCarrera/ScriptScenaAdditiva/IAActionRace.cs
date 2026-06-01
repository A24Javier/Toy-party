using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAActionRace : MonoBehaviour
{
    public float distanciaDeteccion = 2f; // Los 2 metros que buscas
    public LayerMask capaObstaculos; // Selecciona la capa de tus obstáculos

    PlayerRunController MiRunner;

    void Start()
    {
        MiRunner = GetComponent<PlayerRunController>();
    }

    void Update()
    {
        MiRunner.Run();

        // Lanzar Raycast para detectar obstáculos
        DetectarObstaculo();
    }

    void DetectarObstaculo()
    {
        RaycastHit hit;
        // El rayo sale del centro hacia adelante
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaDeteccion, capaObstaculos))
        {
            if (MiRunner.GetInfoJugador().GetSuelo())
            {
                MiRunner.Jump();
            }
        }
    }

    // Dibujar el rayo en el editor para depuración
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * distanciaDeteccion);
    }
}
