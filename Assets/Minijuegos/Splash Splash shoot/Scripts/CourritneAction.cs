using System.Collections;
using UnityEngine;

public class CourritneAction : MonoBehaviour
{
    private PlayerControllerSplashSplashShoot player;

    public float intervalo = 0.15f;

    private Renderer objetoRenderer;
    private bool corrutinaEnMarcha = false;

    private void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        player = GetComponent<PlayerControllerSplashSplashShoot>();
    }

    private void Update()
    {
        if (player == null || objetoRenderer == null)
            return;

        if (player.GetImbatible() && !corrutinaEnMarcha)
        {
            StartCoroutine(EfectoParpadeo());
        }

        if (!player.GetImbatible())
        {
            objetoRenderer.enabled = true;
        }
    }

    private IEnumerator EfectoParpadeo()
    {
        corrutinaEnMarcha = true;

        while (player != null && player.GetImbatible())
        {
            objetoRenderer.enabled = !objetoRenderer.enabled;
            yield return new WaitForSeconds(intervalo);
        }

        if (objetoRenderer != null)
            objetoRenderer.enabled = true;

        corrutinaEnMarcha = false;
    }
}