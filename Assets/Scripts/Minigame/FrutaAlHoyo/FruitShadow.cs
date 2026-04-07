using UnityEngine;

public class LandingIndicator : MonoBehaviour
{
    public GameObject markerPrefab;
    public LayerMask groundLayer;

    private Transform marker;

    void Start()
    {
        // Crear el marcador al inicio
        if (markerPrefab != null)
        {
            marker = Instantiate(markerPrefab).transform;
        }
    }

    void Update()
    {
        if (marker == null) return;

        RaycastHit hit;

        // Lanzamos un raycast hacia abajo desde arriba del objeto
        Vector3 start = transform.position + Vector3.up * 5f;

        if (Physics.Raycast(start, Vector3.down, out hit, 1000f, groundLayer))
        {
            marker.position = hit.point;            // punto donde caerá
            marker.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
