using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IAAction : MonoBehaviour
{
    public Transform objetivo;
    public float distanciaDeteccion = 2.0f;
    public float distanciaDisparo = 15.0f;
    public float velocidadGiro = 2.0f;

    private bool efectuarDisparo;

    private NavMeshAgent agente;
    private PlayerControllerSplashSplashShoot player;
    private bool esIA;

    private Transform ultimoObjetivoIgnorado;
    private float tiempoIgnorar = 1.5f;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        player = GetComponent<PlayerControllerSplashSplashShoot>();

        efectuarDisparo = true;

        if (player == null || player.GetInfo() == null)
        {
            Debug.LogWarning("IAAction: falta PlayerControllerSplashSplashShoot o PlayerInfoSplash en " + gameObject.name);
            enabled = false;
            return;
        }

        esIA = player.GetInfo().GetIA();

        if (agente != null)
        {
            agente.avoidancePriority = Random.Range(0, 99);
            agente.stoppingDistance = Random.Range(1.8f, 2.5f);
            agente.acceleration = 8.0f;
            agente.enabled = false;
        }

        RaycastHit hitSuelo;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hitSuelo, 1.5f))
        {
            if (hitSuelo.collider.CompareTag("Floor"))
                ActivarIA();
        }
    }

    private void ActivarIA()
    {
        if (!esIA || agente == null || agente.enabled)
            return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        agente.enabled = true;

        if (player.GetRig() != null)
            player.GetRig().isKinematic = true;

        Debug.Log("<color=green>IA Activada en: </color>" + gameObject.name);
    }

    private void Update()
    {
        if (player == null || player.GetManager() == null)
            return;

        if (player.GetManager().GameTerminated || !esIA || agente == null || !agente.enabled)
            return;

        objetivo = player.GetManager().ObjectivoIA(player, ultimoObjetivoIgnorado);

        if (objetivo != null && agente.isOnNavMesh)
        {
            DetectarObstaculos();

            if (Vector3.Distance(agente.destination, objetivo.position) > 1.0f)
                agente.SetDestination(objetivo.position);

            float distanciaActual = Vector3.Distance(transform.position, objetivo.position);

            if (distanciaActual <= distanciaDisparo)
            {
                MirarObjetivo();

                if (efectuarDisparo)
                {
                    player.Disparo();
                    efectuarDisparo = false;
                    StartCoroutine(CooldownCambioObjetivo(objetivo));
                }
            }
        }
        else if (objetivo == null)
        {
            if (agente.hasPath)
                agente.ResetPath();
        }
    }

    private IEnumerator CooldownCambioObjetivo(Transform objetivoCazado)
    {
        ultimoObjetivoIgnorado = objetivoCazado;

        if (agente != null && agente.enabled && agente.isOnNavMesh)
            agente.ResetPath();

        yield return new WaitForSeconds(tiempoIgnorar);

        ultimoObjetivoIgnorado = null;
        efectuarDisparo = true;
    }

    private void MirarObjetivo()
    {
        if (objetivo == null)
            return;

        Vector3 direccion = (objetivo.position - transform.position).normalized;
        direccion.y = 0;

        if (direccion != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * velocidadGiro);
        }
    }

    private void DetectarObstaculos()
    {
        if (agente == null)
            return;

        RaycastHit hit;
        Vector3 origen = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(origen, transform.forward, out hit, distanciaDeteccion))
        {
            if (hit.transform != objetivo && !hit.collider.CompareTag("Floor"))
            {
                agente.speed = 1.2f;
                return;
            }
        }

        agente.speed = 3.5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ActivarIA();
        }

        if (agente != null && agente.enabled && collision.gameObject.CompareTag("PlayerSplash"))
        {
            if (agente.velocity.magnitude < 0.5f)
                CambiarObjetivoPorColision();
        }
    }

    private void CambiarObjetivoPorColision()
    {
        if (objetivo != null)
        {
            StartCoroutine(CooldownCambioObjetivo(objetivo));

            Vector3 direccionEscape = transform.right * Random.Range(-1f, 1f) + transform.forward * -1f;
            agente.Move(direccionEscape * 0.8f);
        }
    }
}