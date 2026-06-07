using System.Collections;
using UnityEngine;

public class CourritneAction : MonoBehaviour
{
    private PlayerControllerSplashSplashShoot player;

    public float intervalo = 0.15f;

    private Renderer[] todosLosRenderers;
    private bool corrutinaEnMarcha = false;

    private void Start()
    {
        player = GetComponent<PlayerControllerSplashSplashShoot>();
        todosLosRenderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (player == null) return;

        // Si por alguna razón el array está vacío (ej. el modelo tardó en instanciarse), volvemos a buscar
        if (todosLosRenderers == null || todosLosRenderers.Length == 0)
        {
            todosLosRenderers = GetComponentsInChildren<Renderer>();
            return;
        }

        // Disparamos la corrutina si el jugador es imbatible
        if (player.GetImbatible() && !corrutinaEnMarcha)
        {
            StartCoroutine(EfectoParpadeo());
        }
    }

    private IEnumerator EfectoParpadeo()
    {
        corrutinaEnMarcha = true;

        // Estado inicial para alternar (true/false)
        bool visibilidadActual = false;

        while (player != null && player.GetImbatible())
        {
            // Cambiamos el estado para el siguiente frame del parpadeo
            visibilidadActual = !visibilidadActual;

            // ¡CLAVE!: Apagamos o encendemos TODOS los renderers de los sub-hijos a la vez
            for (int i = 0; i < todosLosRenderers.Length; i++)
            {
                if (todosLosRenderers[i] != null)
                {
                    todosLosRenderers[i].enabled = visibilidadActual;
                }
            }

            yield return new WaitForSeconds(intervalo);
        }

        // Al terminar el estado imbatible, nos aseguramos de encender TODOS de nuevo
        EncenderTodosLosRenderers();

        corrutinaEnMarcha = false;
    }

    private void EncenderTodosLosRenderers()
    {
        if (todosLosRenderers == null) return;

        for (int i = 0; i < todosLosRenderers.Length; i++)
        {
            if (todosLosRenderers[i] != null)
            {
                todosLosRenderers[i].enabled = true;
            }
        }
    }
}