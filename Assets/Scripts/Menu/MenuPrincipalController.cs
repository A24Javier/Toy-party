using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Nombres de escenas")]
    [SerializeField] private string creditos;
    [SerializeField] private string minijuego;
    [SerializeField] private string jugar;
    [SerializeField] private string atras;

    public void IrACreditos()
    {
        SceneManager.LoadScene(creditos);
    }

    public void IrAJugar()
    {
        SceneManager.LoadScene(jugar);
    }

    public void IrAMinijuego()
    {
        SceneManager.LoadScene(minijuego);
    }

    public void IrAtras()
    {
        SceneManager.LoadScene(atras);
    }
}