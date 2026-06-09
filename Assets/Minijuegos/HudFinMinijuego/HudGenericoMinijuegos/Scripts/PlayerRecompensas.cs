using UnityEngine;
using UnityEngine.UI;

public class PlayerRecompensas : MonoBehaviour
{
    [SerializeField] private ManagerFinMinijuego man;

    public bool recompensa = false;

    public bool Recompensa
    {
        get { return recompensa; }
        set { recompensa = value; }
    }

    public string playerPos;

    public int id = 0;
    public int posicion = 0;
    public int estrellas = 0;
    public int moneda = 0;

    [Header("Modelo")]
    [SerializeField] private ModelController MiModelo;
    public int IDPersonaje;
    public int player;
    [SerializeField] private Image img;

    private void OnEnable()
    {
        if (man != null)
        {
            man.AñadirRec(this);
        }
    }

    private void OnDisable()
    {
        if (man != null)
        {
            man.RemoveRec(this);
        }
    }

    public void AplicarModeloVisual()
    {
        if (MiModelo != null)
        {
            MiModelo.AsignarModeloAJugador(IDPersonaje, player, img);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Moneda"))
        {
            Destroy(collision.gameObject);
            moneda++;
        }
    }
}