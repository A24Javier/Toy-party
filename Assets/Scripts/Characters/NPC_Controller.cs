using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : Character
{
    private Board board;
    private Animator animator;

    //private Vector3 upToBox = new Vector3(0, 0.4f, 0);
    private const float TIME_WAIT_PATH = 2.5f;
    private const float MIN_TIME_ROLL = 1f;
    private const float MAX_TIME_ROLL = 2.6f;

    private Box newBox = null;  // Casilla a la que está mirando
    public bool smooth = true;
    public float velocidadDeRotacion = 5f;

    public CameraFollow cameraFollow;

    public float powerJump = 1f;
    public float timeJump = 1f;

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        animator = GetComponent<Animator>();
        board = FindObjectOfType<Board>();
        actualBox = board.GetCasilla(0);
        runningParticles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        Look();
    }


    public IEnumerator doRolling(DiceScript dice)
    {
        float randomTimeRoll = Random.Range(MIN_TIME_ROLL, MAX_TIME_ROLL);
        yield return new WaitForSeconds(randomTimeRoll);
        dice.NPC_Roll();
    }
    public override void Move(int steps)
    {
        StartCoroutine(MoveCharacterBoard(steps));
    }

    private void Look()
    {
        if (newBox == null)
            return;

        Vector3 direction = new Vector3(
            newBox.transform.position.x - transform.position.x,
            0f,
            newBox.transform.position.z - transform.position.z
        );

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
            runningParticles.Play();
            newBox = actualBox.GetNewBox(0);

            Vector3 destination = newBox.GetThisBoxTransf().position; //+ upToBox;
            string animToThis = newBox.GetAnimToThis();

            if (animToThis == "NoAnim")
            {
                while (Vector3.Distance(transform.position, destination) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                    yield return null;
                }
            }
            else if (animToThis == "Jump")
            {
                runningParticles.Stop();
                animator.SetBool("isJumping", true);
                powerJump = actualBox.powerJump;
                timeJump = actualBox.timeJump;
                transform.DOJump(destination, powerJump, 1, timeJump);

                while(Vector3.Distance(transform.position, destination) > 0.05f)
                {
                    yield return null;
                }
                animator.SetBool("isJumping", false);
            }

            transform.position = destination;
            actualBox = newBox;

            cameraFollow.cRotation = actualBox.camRotationY;
            this.savedCameraRotationY = actualBox.camRotationY;

            if (actualBox.PossiblesBoxesCount() >= 2) // Activar sistema encrucijada, pero random
            {
                runningParticles.Stop();
                animator.SetBool("isRunning", false);
                
                int randPath = Random.Range(0, actualBox.PossiblesBoxesCount());

                yield return new WaitForSeconds(TIME_WAIT_PATH); // Hace como que esta eligiendo

                animator.SetBool("isRunning", true);
                newBox = actualBox.GetBoxTransf(randPath).GetComponent<Box>();
                destination = newBox.GetThisBoxTransf().position;
                animToThis = newBox.GetAnimToThis();

                if (animToThis == "NoAnim")
                {
                    runningParticles.Stop();
                    while (Vector3.Distance(transform.position, destination) > 0.05f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                        yield return null;
                    }
                }
                else if (animToThis == "Jump")
                {
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

                transform.position = actualBox.GetBoxTransf(randPath).position;
            }
            //yield return new WaitForSeconds(0.05f);

        }

        runningParticles.Stop();
        // Nos aseguramos de que "isRunning" se desactiva (ya que sin esto a veces no lo hace)
        animator.SetBool("isRunning", false);
        yield return null;
        actualBox.ActivateEffect(this);
        UIManager.instance.DeactivatePathDecision();

    }

    public IEnumerator ProcessBuyStar(int price)
    {
        // Si el NPC tiene las monedas necesarias para comprar la estrella la comprará
        bool buyStar = coins >= price;
        Debug.Log("Decidiendo si comprar la estrella...");
        yield return new WaitForSeconds(3.5f);
        if(buyStar) { UIManager.instance.BuyStar(); }
        else { UIManager.instance.NotBuyStar(); }
        
    }

    public override IEnumerator DoAnim(string animationKey, string animationName)
    {
        // Hacemos que haga la animación
        animator.SetBool(animationKey, true);
        yield return null;

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Antes de comprobacion animationName");


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
    }
}
