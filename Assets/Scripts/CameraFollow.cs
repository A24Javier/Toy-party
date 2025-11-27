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

        Vector3 targetPos =
            jugador.position
            + (jugador.right * offsetX)
            + (Vector3.up * offsetY)
            + (jugador.forward * offsetZ);

        Vector3 directionBackward = Quaternion.Euler(0f, cRotation, 0f) * Vector3.forward;
        targetPos -= directionBackward * 0.4f;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smooth * Time.deltaTime
        );

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
