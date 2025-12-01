using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruta : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            Destroy(gameObject);
        }
    }
}
