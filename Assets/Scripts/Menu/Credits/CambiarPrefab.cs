using UnityEngine;

public class CambiarPrefab : MonoBehaviour
{
    [Header("Prefab UI")]
    [SerializeField] private GameObject prefabAInstanciarRight;
    [SerializeField] private GameObject prefabAInstanciarLeft;

    [Header("Nombre del punto de instanciación")]
    [SerializeField] private string nombrePuntoDeInstanciacion = "SpawnPoint";

    [Header("Objeto que tiene el Animator")]
    [SerializeField] private GameObject objetoAnimado;

    private Transform puntoDeInstanciacion;
    private Canvas canvasPadre;

    private void Awake()
    {
        GameObject objetoSpawn = GameObject.Find(nombrePuntoDeInstanciacion);
        if (objetoSpawn != null)
        {
            puntoDeInstanciacion = objetoSpawn.transform;
        }

        canvasPadre = FindFirstObjectByType<Canvas>();

        if (objetoAnimado == null)
        {
            return;
        }

        Animator animator = objetoAnimado.GetComponent<Animator>();
        if (animator == null)
        {
            return;
        }

        animator.SetBool("Entrar?", true);
        animator.SetBool("Salir?", false);
    }

    public void EjecutarAccionRight()
    {
        if (prefabAInstanciarRight == null)
        {
            Debug.LogWarning("No hay prefab asignado");
            return;
        }

        if (puntoDeInstanciacion == null)
        {
            Debug.LogWarning("No se ha encontrado el punto de instanciación");
            return;
        }

        if (canvasPadre == null)
        {
            Debug.LogWarning("No se ha encontrado ningún Canvas en la escena");
            return;
        }

        GameObject nuevoObjeto = Instantiate(prefabAInstanciarRight, canvasPadre.transform);

        RectTransform rectNuevo = nuevoObjeto.GetComponent<RectTransform>();
        RectTransform rectSpawn = puntoDeInstanciacion.GetComponent<RectTransform>();

        if (rectNuevo != null && rectSpawn != null)
        {
            rectNuevo.anchoredPosition = rectSpawn.anchoredPosition;
            rectNuevo.localScale = Vector3.one;
            rectNuevo.localRotation = Quaternion.identity;
        }
        else
        {
            nuevoObjeto.transform.position = puntoDeInstanciacion.position;
        }

        if (objetoAnimado == null)
        {
            Debug.LogWarning("No hay objeto animado asignado");
            return;
        }

        Animator animator = objetoAnimado.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("El objeto animado no tiene Animator");
            return;
        }

        animator.SetBool("Entrar?", false);
        animator.SetBool("Salir?", true);

        Destroy(gameObject, 0.20f);
    }

    public void EjecutarAccionLeft()
    {
        if (prefabAInstanciarLeft == null)
        {
            Debug.LogWarning("No hay prefab asignado");
            return;
        }

        if (puntoDeInstanciacion == null)
        {
            Debug.LogWarning("No se ha encontrado el punto de instanciación");
            return;
        }

        if (canvasPadre == null)
        {
            Debug.LogWarning("No se ha encontrado ningún Canvas en la escena");
            return;
        }

        GameObject nuevoObjeto = Instantiate(prefabAInstanciarLeft, canvasPadre.transform);

        RectTransform rectNuevo = nuevoObjeto.GetComponent<RectTransform>();
        RectTransform rectSpawn = puntoDeInstanciacion.GetComponent<RectTransform>();

        if (rectNuevo != null && rectSpawn != null)
        {
            rectNuevo.anchoredPosition = rectSpawn.anchoredPosition;
            rectNuevo.localScale = Vector3.one;
            rectNuevo.localRotation = Quaternion.identity;
        }
        else
        {
            nuevoObjeto.transform.position = puntoDeInstanciacion.position;
        }

        if (objetoAnimado == null)
        {
            Debug.LogWarning("No hay objeto animado asignado");
            return;
        }

        Animator animator = objetoAnimado.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("El objeto animado no tiene Animator");
            return;
        }

        animator.SetBool("Entrar?", false);
        animator.SetBool("Salir?", true);

        Destroy(gameObject, 0.20f);
    }


}