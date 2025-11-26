using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform jugador;
    public float smooth = 5f;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    public float cRotation = 0f;

    void LateUpdate()
    {
        if (jugador == null) return;

        // 1. POSICIÓN BASE SEGÚN OFFSET
        Vector3 targetPos =
            jugador.position
            + (jugador.right * offsetX)
            + (Vector3.up * offsetY)
            + (jugador.forward * offsetZ);

        // 2. RETROCESO UNIVERSAL (-0.4) SEGÚN cRotation
        Vector3 directionBackward = Quaternion.Euler(0f, cRotation, 0f) * Vector3.forward;
        targetPos -= directionBackward * 0.4f;

        // 3. MOVIMIENTO SUAVE
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smooth * Time.deltaTime
        );

        // 4. ROTACIÓN SOLO EN EJE Y
        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 targetEuler = new Vector3(currentEuler.x, cRotation, currentEuler.z);

        Quaternion targetRot = Quaternion.Euler(targetEuler);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            smooth * Time.deltaTime
        );
    }





    public void SetTarget(Transform newJugador)
    {
        jugador = newJugador;
    }
}
