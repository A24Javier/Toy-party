using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform jugador;
    public float smooth = 5f;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    private Quaternion currentRotation; 

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 newPos = new Vector3(
            jugador.position.x + offsetX,
            jugador.position.y + offsetY,
            jugador.position.z + offsetZ
        );

        transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);

        transform.rotation = currentRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cam_C"))
        {
            Box box = other.GetComponent<Box>();
            if (box != null)
            {
                currentRotation *= Quaternion.Euler(0f, box.cRotation, 0f); 
            }
          
        }
        Debug.Log("aaaaaaa");
    }

    public void SetTarget(Transform newJugador)
    {
        jugador = newJugador;
    }
}
