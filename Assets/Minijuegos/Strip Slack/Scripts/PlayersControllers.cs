using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersControllers : MonoBehaviour
{
    //Variables para la fuerza
    public float forcePlayer = 0f;
    float aumentoBase = 0.5f;
    float multiplicadorPorRapidez = 1.5f;

    float tiempoUltimoClick = 1.0f;

    float velocidadDescenso = 10.0f;

    const float maxForce = 100.0f;
    void Update()
    {
        // 1. La fuerza baja constantemente cada frame, sin importar los clicks
        if (forcePlayer > 0)
        {
            forcePlayer -= Time.deltaTime * velocidadDescenso;

            // Evitamos que la fuerza sea negativa
            forcePlayer = Mathf.Max(forcePlayer, 0);
        }
    }

    public void AugmentForce()
    {
        if (forcePlayer <= maxForce)
        {
            // 1. Calculamos cu�nto tiempo pas� desde el �ltimo click
            float tiempoDesdeUltimo = Time.time - tiempoUltimoClick;

            // 2. Calculamos el bono de velocidad (si el tiempo es muy corto, el bono es alto)
            // Usamos 1 / tiempo para que a menor tiempo, mayor sea el resultado
            float bonoRapidez = Mathf.Clamp(1.0f / tiempoDesdeUltimo, 1f, 10f);

            // 3. Aumentamos la variable
            forcePlayer += aumentoBase * (bonoRapidez * multiplicadorPorRapidez);

            // Actualizamos la marca de tiempo
            tiempoUltimoClick = Time.time;
        }
    }
}
