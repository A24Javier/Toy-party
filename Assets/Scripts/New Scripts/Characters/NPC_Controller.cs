using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : Character
{
    private Board board;
    private Box actualBox;
    private Animator animator;

    // Inventario (por hacer) (tal vez merezca hacer polimorfismo para el inventario)
    private Dice specialDice;

    //private Vector3 upToBox = new Vector3(0, 0.4f, 0);
    private const float TIME_WAIT_PATH = 2.5f;
    private const float MIN_TIME_ROLL = 1f;
    private const float MAX_TIME_ROLL = 2.6f;

    void Start()
    {
        animator = GetComponent<Animator>();
        board = FindObjectOfType<Board>();
        actualBox = board.GetCasilla(0);
    }

    void Update()
    {
        
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

    protected override IEnumerator MoveCharacterBoard(int steps)
    {
        animator.SetBool("isRunning", true);
        for (int i = 0; i < steps; i++)
        {
            Box newBox = actualBox.GetNewBox(0);

            Vector3 destination = newBox.GetThisBoxTransf().position; //+ upToBox;

            while (Vector3.Distance(transform.position, destination) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = destination;
            actualBox = newBox;

            if(actualBox.PossiblesBoxesCount() >= 2) // Activar sistema encrucijada, pero random
            {
                animator.SetBool("isRunning", false);
                bool[] isPathToStar = actualBox.GetIsPathToStar();
                int pathesStar = 0;

                for(int j = 0; j < isPathToStar.Length; j++) // Averiguamos cuantos caminos pueden llevar a la estrella
                {
                    if (isPathToStar[j]) {pathesStar++;}
                }

                // 90% de ir por el camino de la estrella, 10% que no
                int randStarPath = Random.Range(0, 101); // Saca un numero entre 0 y 100

                if (randStarPath <= 90) // Ir camino estrella
                {
                    int randPathStarSelected = Random.Range(0, pathesStar); // Elije uno de los posibles caminos que puede llevar a la estrella
                    int pathPass = 0;

                    yield return new WaitForSeconds(TIME_WAIT_PATH); // Hace como que esta eligiendo

                    for (int j = 0; j < actualBox.PossiblesBoxesCount(); j++)
                    {
                        animator.SetBool("isRunning", true);
                        if (isPathToStar[j])
                        {
                            if (pathPass == randPathStarSelected)
                            {
                                while (Vector3.Distance(transform.position, actualBox.GetBoxTransf(j).position) > 0.05f)
                                {
                                    transform.position = Vector3.MoveTowards(transform.position, actualBox.GetBoxTransf(j).position, speed * Time.deltaTime);
                                    yield return null;
                                }
                                transform.position = actualBox.GetBoxTransf(j).position;
                            }
                            else { pathPass++; }
                        }
                    }
                }
            }
            //yield return new WaitForSeconds(0.05f);

        }

        // Nos aseguramos de que "isRunning" se desactiva (ya que sin esto a veces no lo hace)
        while (animator.GetBool("isRunning"))
        {
            animator.SetBool("isRunning", false);
            yield return null;
        }
        actualBox.ActivateEffect(this);
        
    }

    public IEnumerator ProcessBuyStar(int price)
    {
        // Si el NPC tiene las monedas necesarias para comprar la estrella la comprará
        bool buyStar = coins >= price;
        yield return new WaitForSeconds(5f);
        if(buyStar) { UIManager.instance.BuyStar(); }
        else { UIManager.instance.NotBuyStar(); }
        
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
