using UnityEngine;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Moneda"))
        {
            Destroy(collision.gameObject);
            moneda++;
        }
    }
}