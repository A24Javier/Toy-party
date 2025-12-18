using System.Collections;
using UnityEngine;

public class PuertaEvento : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Abriendo()
    {
        animator.SetBool("abriendo", true);
        animator.SetBool("cerrando", false);

        StartCoroutine(CerrarDespuesDeTiempo());
    }

    private IEnumerator CerrarDespuesDeTiempo()
    {
        yield return new WaitForSeconds(4f);
        Cerrando();
    }

    public void Cerrando()
    {
        animator.SetBool("abriendo", false);
        animator.SetBool("cerrando", true);
    }
}
