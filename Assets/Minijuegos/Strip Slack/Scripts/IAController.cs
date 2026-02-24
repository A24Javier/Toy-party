using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour
{
    //Variables para la fuerza
    public float forceIA = 0f;
    float aumentoBase = 0.5f;
    float multiplicadorForceIA = 1.5f;

    float tiempoIANullForce = 1.0f;

    float forceIAMax = 95.0f;
    private void Update()
    {
        AugmentForceIA();
    }
    void AugmentForceIA()
    {
        if (forceIA <= forceIAMax)
        {
            float tiempoIAForce = Time.time - tiempoIANullForce;

            float bonoForce = Mathf.Clamp(1.0f / tiempoIAForce, 1f, 10f);

            forceIA += aumentoBase * (bonoForce * multiplicadorForceIA);

            tiempoIANullForce = Time.time;
        }
        if (forceIA > 0)
        {
            forceIA -= Time.deltaTime * 10.0f;
        }
    }
}
