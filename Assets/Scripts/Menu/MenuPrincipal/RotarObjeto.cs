using UnityEngine;

public class RotarObjeto : MonoBehaviour
{
    [SerializeField] private Vector3 velocidadRotacion = new Vector3(0, 50, 0);

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime, Space.Self);
    }
}