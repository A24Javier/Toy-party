using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    [Header("Nombres de escenas")]
    [SerializeField] private string atras;


    public void IrAtras()
    {
        SceneManager.LoadScene(atras);
    }
}