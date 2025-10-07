using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform objetivo;

    public bool smooth = true;

    public float velocidadDeRotacion = 5f;

    private void LateUpdate()
    {
        if (objetivo == null)
            return;

        Vector3 direction = new Vector3(objetivo.position.x - transform.position.x, 0f, objetivo.position.z - transform.position.z);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            if (smooth)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * velocidadDeRotacion);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }
}