using UnityEngine;

public class NPCControllerSimple : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;
    public float changeDirectionTime = 1.5f;

    [Header("Área de Movimiento")]
    public float radius = 3f;

    private Vector3 moveDirection;
    private float timer = 0f;
    private Vector3 origin;

    private Animator anim;
    private bool isVictory = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        origin = transform.position;
        NewRandomDirection();
    }

    void Update()
    {
        if (isVictory) return;   

        timer += Time.deltaTime;

        if (timer >= changeDirectionTime)
        {
            NewRandomDirection();
            timer = 0f;
        }

        transform.position += moveDirection * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, origin) > radius)
        {
            Vector3 dirBack = (origin - transform.position).normalized;
            moveDirection = dirBack;
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 5f
            );
        }

        anim.SetBool("IsRunnin", moveDirection.sqrMagnitude > 0.01f);
    }

    void NewRandomDirection()
    {
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    public void Victory()
    {
        isVictory = true;
        anim.SetBool("IsRunnin", false);   // dejar de correr
        anim.SetBool("IsVictory", true);   // activar anim Victory
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? origin : transform.position, radius);
    }
}
