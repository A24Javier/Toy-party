using UnityEngine;

public class BalaController : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ShootAction();
    }

    private void ShootAction()
    {
        if (rb == null)
            return;

        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}