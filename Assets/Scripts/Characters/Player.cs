using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class Player : Character
{
    private Board board;
    private Vector3 upToBox = new Vector3 (0f, 0.07f, 0f);
    public bool isSelectingPath = false;
    private Animator animator;
    Box newBox = null;
    public bool smooth = true;
    public float velocidadDeRotacion = 5f;
    [SerializeField] private Item randomTP;

    //jump
    public float powerJump = 1f;
    public float timeJump = 1f;

    // Sounds
    private AudioSource audioSource;
    [SerializeField] private AudioClip stepSfx;

    // Debug
    private bool isDebug = false;
    [SerializeField] private DebugEvent[] movesWithoutFinish;

    private void OnEnable()
    {
        foreach(DebugEvent dgEvent in movesWithoutFinish)
        {
            DebugFunctions.instance.AddEvent(dgEvent);
        }
        
    }

    private void OnDisable()
    {
        foreach (DebugEvent dgEvent in movesWithoutFinish)
        {
            DebugFunctions.instance.RemoveEvent(dgEvent);
        }
    }

    void Start()
    {
        inventory.AddItem(randomTP);
        animator = GetComponent<Animator>();
        board = GameObject.FindObjectOfType<Board>();
        actualBox = board.GetCasilla(0);
        runningParticles = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = stepSfx;
        //transform.position = actualBox1.GetThisBoxTransf().position + upToBox;
    }

    private void Update()
    {
        Look();
    }
    public override void Move(int steps)
    {
        int totalSteps = steps + extraStep;
        totalSteps = Mathf.Max(totalSteps, 0); // TotalSteps nunca será un numero negativon 
        Debug.Log($"TotalSteps es: {totalSteps}");

        StartCoroutine(MoveCharacterBoard(totalSteps));
        extraStep = 0;
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
            audioSource.Play();
            runningParticles.Play();
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
                runningParticles.Stop();
                animator.SetBool("isJumping", true);
                powerJump = actualBox.powerJump;
                timeJump = actualBox.timeJump;
                transform.DOJump(destination, powerJump, 1, timeJump);
                while (Vector3.Distance(transform.position, destination) > 0.05f)
                {
                    yield return null;
                }
                animator.SetBool("isJumping", false);
            }

            transform.position = destination;
            actualBox = newBox;

            // Guardamos la rotación de cámara de esta casilla
            if (actualBox != null)
            {
                savedCameraRotationY = actualBox.camRotationY;

                // GIRAMOS LA CÁMARA AL MOMENTO
                GameController.instance.UpdateCameraRotation(savedCameraRotationY);
            }
            else
            {
                savedCameraRotationY = 0f;
            }


            if (actualBox.PossiblesBoxesCount() >= 2)
            {
                // Pausamos el movimiento y dejamos que el jugador elija
                runningParticles.Stop();
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

        runningParticles.Stop();
        // Activamos efectos de la última casilla
        animator.SetBool("isRunning", false);
        if (!isDebug) { newBox.ActivateEffect(this); }
        
    }


    public IEnumerator PathSelected(Vector3 destination1, Box box)
    {
        UIManager.instance.DeactivatePathDecision();
        audioSource.Play();
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
            powerJump = actualBox.powerJump;
            timeJump = actualBox.timeJump;
            transform.DOJump(destination1, powerJump, 1, timeJump);
            while (Vector3.Distance(transform.position, destination1) > 0.05f)
            {
                yield return null;
            }
            animator.SetBool("isJumping", false);
        }

        actualBox = box;
        if (actualBox != null)
        {
            savedCameraRotationY = actualBox.camRotationY;
            GameController.instance.UpdateCameraRotation(savedCameraRotationY);
        }
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

    #region Debug Functions
    public void MoveWithoutFinalizeTurn(int steps)
    {
        isDebug = true;
        Move(steps);
    }
    #endregion

}
