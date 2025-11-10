using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class Player : Character
{
    private Board board;
    private Vector3 upToBox = new Vector3 (0f, 0.07f, 0f);
    private Box actualBox1;
    public bool isSelectingPath = false;
    private Animator animator;

    // Inventario (por hacer) (tal vez merezca hacer polimorfismo para el inventario)
    private Dice specialDice;

    void Start()
    {
        animator = GetComponent<Animator>();
        board = GameObject.FindObjectOfType<Board>();
        actualBox1 = board.GetCasilla(0);
        transform.position = actualBox1.GetThisBoxTransf().position + upToBox;
    }

    public override void Move(int steps)
    {
        StartCoroutine(MoveCharacterBoard(steps));
    }

    protected override IEnumerator MoveCharacterBoard(int steps)
    {
        Box newBox = null;
        animator.SetBool("isRunning", true);

        for (int i = 0; i < steps; i++)
        {
            newBox = actualBox1.GetNewBox(0);

            Vector3 destination = newBox.GetThisBoxTransf().position + upToBox;

            while (Vector3.Distance(transform.position, destination) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = destination;
            actualBox1 = newBox;

            if (actualBox1.PossiblesBoxesCount() >= 2)
            {
                Debug.Log("Esta en encrucijada");
                animator.SetBool("isRunning", false);
                isSelectingPath = true;

                UIManager.instance.SetActualPlayer(this);
                UIManager.instance.CreateSelectionPath(newBox);

                // Es una encrucijada
                // Desactivar movimiento

                while (isSelectingPath)
                {
                    yield return null;
                }
                UIManager.instance.DeactivatePathDecision();
                newBox = actualBox1;

                // Activar movimiento
                animator.SetBool("isRunning", true);
            }

            //yield return new WaitForSeconds(0.2f);
        }

        // Nos aseguramos de que "isRunning" se desactiva (ya que sin esto a veces no lo hace)
        while (animator.GetBool("isRunning"))
        {
            animator.SetBool("isRunning", false);
            yield return null;
        }
        
        newBox.ActivateEffect(this);
    }

    public IEnumerator PathSelected(Vector3 destination1, Box box)
    {
        destination1 += upToBox;

        while (Vector3.Distance(transform.position, destination1) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination1, speed * Time.deltaTime);
            yield return null;
        }
        actualBox1 = box;
        isSelectingPath = false;
    }

    public override IEnumerator DoAnim(string animationKey)
    {
        // Hacemos que haga la animación
        animator.SetBool(animationKey, true);
        yield return null;

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        // Nos aseguramos de que se esta ejecutando esta animación
        while (!animatorState.IsName(animationKey))
        {
            animatorState = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Esperamos a que la animación finalice
        while (animatorState.normalizedTime < 1f)
        {
            // Vamos actualizando el AnimatorState
            animatorState = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(animatorState.normalizedTime);
            yield return null;
        }

        // Hacemos que su bool vuelva a ser false
        animator.SetBool(animationKey, false);
    }
}
