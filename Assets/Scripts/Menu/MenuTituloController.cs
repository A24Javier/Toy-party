using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTituloController : MonoBehaviour
{
    [Header("Nombres de escenas")]
    [SerializeField] private string credit;

    public void IrACredit()
    {
        SceneManager.LoadScene(credit);
    }
}