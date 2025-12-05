using UnityEngine;

public class FruitMarkerFollow : MonoBehaviour
{
    public GameObject prefabMarcador;
    public LayerMask layerSuelo;

    private Transform marcador;
    private float posYsuelo;

    private void OnEnable()
    {
        CrearMarcador();
    }

    private void OnDisable()
    {
        if (marcador != null)
            Destroy(marcador.gameObject);
    }

    void CrearMarcador()
    {
        if (prefabMarcador == null)
        {
            Debug.LogError(" No hay prefab de marcador asignado");
            return;
        }
        RaycastHit hit;

        Vector3 origenArriba = transform.position + Vector3.up * 50f;
        Vector3 origenAbajo = transform.position - Vector3.up * 50f;

        bool downHit = Physics.Raycast(origenArriba, Vector3.down, 100f, layerSuelo, QueryTriggerInteraction.Ignore);
        bool upHit = Physics.Raycast(origenAbajo, Vector3.up, 100f, layerSuelo, QueryTriggerInteraction.Ignore);

        Vector3 posSuelo;

        if (Physics.Raycast(origenArriba, Vector3.down, out hit, 100f, layerSuelo))
        {
            posSuelo = hit.point;
        }
        else if (Physics.Raycast(origenAbajo, Vector3.up, out hit, 100f, layerSuelo))
        {
            posSuelo = hit.point;
        }
        else
        {
            Debug.LogWarning(" No se encontró suelo alrededor de " + name);
            return;
        }

        posYsuelo = posSuelo.y + 0.02f;

        GameObject m = Instantiate(prefabMarcador,
        new Vector3(transform.position.x, posYsuelo, transform.position.z),
        Quaternion.identity
        );

        marcador = m.transform;
    }

    private void Update()
    {
        if (marcador == null) return;

        marcador.position = new Vector3(
            transform.position.x,
            posYsuelo,
            transform.position.z
        );
    }
}
