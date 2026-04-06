using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    [SerializeField] private string atras;

    public void IrAtras()
    {
        SceneManager.LoadScene(atras);
    }
}
