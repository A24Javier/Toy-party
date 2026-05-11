using UnityEngine;

public class Muro : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("gotaPro"))
        {
            Destroy(collision.gameObject);
        }
    }
}