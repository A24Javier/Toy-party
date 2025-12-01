using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class IAMove : MonoBehaviour
{

    float speed = 1.5f;
    float distanciaMin = 0.5f;
    
    public Transform target;
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    [SerializeField] Transform target3;
    [SerializeField] Transform target4;
    
    Rigidbody rb;

    int frutacount = 0;

    [SerializeField] ControllerFruta mifruta;

    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        NewObjetiu(target1);
    }

    public void NewObjetiu(Transform Newtarget)
    {
        if (Newtarget != null)
        {
            this.target = Newtarget;
        }
    }

    private void FixedUpdate()
    {
        if (mifruta.frutaTime) {
            float distancia = Vector3.Distance(transform.position, target.position);

            if (distancia > distanciaMin)
            {
                Vector3 direccion = (target.position - transform.position).normalized;
                Vector3 velocidadDeseada = new Vector3(direccion.x, 0f, direccion.z) * speed;

                rb.velocity = new Vector3(velocidadDeseada.x, rb.velocity.y, velocidadDeseada.z);
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                if (target == target1)
                {
                    NewObjetiu(target2);
                }
                else if (target == target2)
                {
                    NewObjetiu(target3);
                }
                else if (target == target3)
                {
                    NewObjetiu(target4);
                }
                else if (target == target4)
                {
                    NewObjetiu(target1);
                }
            }

            textMeshProUGUI.text = frutacount.ToString();

        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mifruta.frutaTime) {
            if (collision.gameObject.CompareTag("Fruta1"))
            {
                frutacount--;
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Fruta2"))
            {
                frutacount++;
                Destroy(collision.gameObject);
            }
        }
        
    }
}
