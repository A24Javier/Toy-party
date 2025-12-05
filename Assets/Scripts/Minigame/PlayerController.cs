using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public Transform cameraTransform;  

    private Rigidbody rb;
    private PlayerInput input;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInput();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector2 move = input.PlayerMov.Move.ReadValue<Vector2>();

        anim.SetBool("IsRunnin", move.sqrMagnitude > 0.01f);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = camForward * move.y + camRight * move.x;

        Vector3 velocity = direction * speed + new Vector3(0, rb.velocity.y, 0);
        rb.velocity = velocity;
    }

    private void Rotate()
    {
        Vector2 move = input.PlayerMov.Move.ReadValue<Vector2>();

        if (move.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = -cameraTransform.forward;
            Vector3 camRight = -cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 direction = camForward * move.y + camRight * move.x;

            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void Victory()
    {
        anim.SetBool("IsVictory", true);
    }
}
