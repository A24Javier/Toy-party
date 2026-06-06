using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMinijuegosController : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Idle ()
    {
        animator.SetBool("Jump", false);
        animator.SetBool("Run", false);
    }

    public void Saltar()
    {
        animator.SetBool("Jump",true);
    }

    public void Correr()
    {
        animator.SetBool("Run", true);
    }

    public void EstadoAnimo(int puesto)
    {
        animator.SetInteger("Position", puesto);
    }
}
