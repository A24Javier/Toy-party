using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerCesta : MonoBehaviour
{
    [SerializeField] InputActionAsset MappingContext;
    InputAction MoveCesta;

    Rigidbody rb;

    float speed = 2f;

    int frutacount = 0;

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    float[] maxposx = {-4.534f, 4.08f };

    float[] maxposz = { -10.85f, -3.9f};

    [SerializeField] ControllerFruta mifruta;

    private void Start()
    {
        MappingContext.Enable();
        MoveCesta = MappingContext.FindActionMap("Cesta").FindAction("Move");
        rb = GetComponent<Rigidbody>();
    }

    void Mover()
    {
        //Lemeoas el input action
        Vector2 Move = MoveCesta.ReadValue<Vector2>();

        //Movimento horintal
        Vector3 MoveHorz = new Vector3(Move.x, 0f, Move.y);

        //agarramos la velocidad y
        float Speedy = rb.velocity.y;

        //creamos una nueva velocidad
        Vector3 newSpeed = (MoveHorz * speed);

        //limiar al juagador para que salga del escenario y que el juego no se rompa por curioidad del jugador.
        if (transform.position.z >= maxposz[1] && newSpeed.z > 0)
        {
            // Acci�n inversa: anular solo el movimiento en Z
            newSpeed.z = 0;
        }
        // Si estamos en el l�mite inferior Z (maxposz[0]) Y el input (newSpeed.z) intenta ir M�S ALL� (negativo)...
        else if (transform.position.z <= maxposz[0] && newSpeed.z < 0)
        {
            // Acci�n inversa: anular solo el movimiento en Z
            newSpeed.z = 0;
        }

        // --- Comprobaci�n del l�mite x (controlado por la f�sica/gravedad) ---
       

        // como el limite que queremos es la posicion x como en el ejemplo anterior, pero con la x
        if (transform.position.x >= maxposx[1] && newSpeed.x > 0)
        {  
            newSpeed.x = 0;
        }
        
        else if (transform.position.x <= maxposx[0] && newSpeed.x < 0)
        {
            newSpeed.x = 0;
        }

        //por ultimo aplicamos la velocidad de movimento
        rb.velocity = new Vector3(newSpeed.x, Speedy, newSpeed.z);
    }

    private void FixedUpdate()
    {
        if (mifruta.frutaTime)
        {
            Mover();
            textMeshProUGUI.text = frutacount.ToString();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mifruta.frutaTime)
        {
            if (collision.gameObject.CompareTag("Fruta1"))
            {
                frutacount++;
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Fruta2"))
            {
                frutacount--;
                Destroy(collision.gameObject);
            }
        }
    }
}
