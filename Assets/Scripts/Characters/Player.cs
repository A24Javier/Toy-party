using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;


public class Player : Character
{
    private Board board;
    private Vector3 upToBox = new Vector3 (0f, 0.07f, 0f);
    public bool isSelectingPath = false;
    private Animator animator;
    Box newBox = null;
    public bool smooth = true;
    public float velocidadDeRotacion = 5f;
    public CameraFollow cameraFollow;
    [SerializeField] private Item randomTP;

    void Start()
    {
        inventory.AddItem(randomTP);
        animator = GetComponent<Animator>();
        board = GameObject.FindObjectOfType<Board>();
        actualBox = board.GetCasilla(0);
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        //transform.position = actualBox1.GetThisBoxTransf().position + upToBox;
    }

    private void Update()
    {
        Look();
    }
    public override void Move(int steps)
    {
        StartCoroutine(MoveCharacterBoard(steps));
    }

    private void Look()
    {
        if (newBox == null)
            return;

        Vector3 direction = new Vector3(newBox.gameObject.transform.position.x - transform.position.x, 0f, newBox.gameObject.transform.position.z - transform.position.z);

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

    protected override IEnumerator MoveCharacterBoard(int steps)
    {
        animator.SetBool("isRunning", true);

        for (int i = 0; i < steps; i++)
        {
            // Asignamos directamente al campo de la clase para Look()
            newBox = actualBox.GetNewBox(0);

            string animToThis = newBox.GetAnimToThis();
            Vector3 destination = newBox.GetThisBoxTransf().position + upToBox;

            if (animToThis == "NoAnim")
            {
                while (Vector3.Distance(transform.position, destination) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else if(animToThis == "Jump")
            {
                animator.SetBool("isJumping", true);
                transform.DOJump(destination, 1f, 1, 1f);
                while (Vector3.Distance(transform.position, destination) > 0.05f)
                {
                    yield return null;
                }
                animator.SetBool("isJumping", false);
            }

            transform.position = destination;
            actualBox = newBox;

            cameraFollow.cRotation = actualBox.camRotationY;
            this.savedCameraRotationY = actualBox.camRotationY;


            if (actualBox.PossiblesBoxesCount() >= 2)
            {
                // Pausamos el movimiento y dejamos que el jugador elija
                animator.SetBool("isRunning", false);
                isSelectingPath = true;

                UIManager.instance.SetActualPlayer(this);
                UIManager.instance.CreateSelectionPath(actualBox);

                // Esperamos a que PathSelected() se encargue de actualizar newBox
                while (isSelectingPath)
                {
                    yield return null;
                }

                // Reanudamos movimiento
                animator.SetBool("isRunning", true);
            }
        }

        // Activamos efectos de la última casilla
        animator.SetBool("isRunning", false);

        newBox.ActivateEffect(this);
    }


    public IEnumerator PathSelected(Vector3 destination1, Box box)
    {
        UIManager.instance.DeactivatePathDecision();

        newBox = box;

        destination1 += upToBox;
        string animToThis = newBox.GetAnimToThis();

        if(animToThis == "NoAnim")
        {
            while (Vector3.Distance(transform.position, destination1) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination1, speed * Time.deltaTime);
                yield return null;
            }
        }
        else if(animToThis == "Jump")
        {
            animator.SetBool("isJumping", true);
            transform.DOJump(destination1, 1f, 1, 1f);
            while (Vector3.Distance(transform.position, destination1) > 0.05f)
            {
                yield return null;
            }
            animator.SetBool("isJumping", false);
        }

        actualBox = box;
        isSelectingPath = false;
    }


    public override IEnumerator DoAnim(string animationKey, string animationName)
    {
        // Hacemos que haga la animación
        animator.SetBool(animationKey, true);
        yield return null;

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        
        // Nos aseguramos de que se esta ejecutando esta animación
        while (!animatorState.IsName(animationName))
        {
            animatorState = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Esperamos a que la animación finalice
        while (animatorState.normalizedTime < 1f)
        {
            // Vamos actualizando el AnimatorState
            animatorState = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Hacemos que su bool vuelva a ser false
        animator.SetBool(animationKey, false);
        yield return null;
        Debug.Log($"La animationKey {animationKey} esta en {animator.GetBool(animationKey)}");
    }
}
