using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAActionRace : MonoBehaviour
{
    public float distanciaDeteccion = 3f; // Te sugiero subirlo a 3f o 4f porque la IA ahora va más rápido
    public LayerMask capaObstaculos;

    [SerializeField] private float _alturaOjosRaycast = 0.5f; // Eleva el rayo del suelo para que no choque con la pista

    private PlayerRunController MiRunner;

    void Start()
    {
        MiRunner = GetComponent<PlayerRunController>();
    }

    // Usamos FixedUpdate porque trabajamos con el Rigidbody del corredor
    void FixedUpdate()
    {
        // ESCUDO 1: Si no hay corredor asignado, salimos
        if (MiRunner == null || MiRunner.GetInfoJugador() == null) return;

        // ESCUDO 2: Si este personaje NO está marcado como IA, o NO le toca moverse aún...
        // ¡Bloqueamos el script por completo para que no actúe como un fantasma!
        if (!MiRunner.GetInfoJugador().GetIA() || !MiRunner.GetInfoJugador().CanMove)
        {
            return;
        }
        else
        {
            // A partir de aquí va tu lógica de Raycast, detección de vallas o saltos automáticos
            DetectarObstaculo();
        }
    }

    void DetectarObstaculo()
    {
        RaycastHit hit;

        // Creamos un punto de origen un poco más arriba de los pies (el centro del personaje)
        Vector3 origenRayo = transform.position + (Vector3.up * _alturaOjosRaycast);

        // Lanzamos el Raycast seguro
        if (Physics.Raycast(origenRayo, transform.forward, out hit, distanciaDeteccion, capaObstaculos))
        {
            // Opcional: Asegurarnos de que lo que ve adelante es una valla
            if (hit.collider.CompareTag("Valla"))
            {
                if (MiRunner.GetInfoJugador().GetSuelo())
                {
                    MiRunner.Jump();
                    Debug.Log($"¡IA detectó {hit.collider.name} y saltó!");
                }
            }
        }
    }

    // Dibujar el rayo correctamente en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origenRayo = transform.position + (Vector3.up * _alturaOjosRaycast);
        Gizmos.DrawRay(origenRayo, transform.forward * distanciaDeteccion);
    }
}
