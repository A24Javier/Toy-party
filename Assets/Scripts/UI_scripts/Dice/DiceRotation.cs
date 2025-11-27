using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRotation : MonoBehaviour
{

    private void Update()
    {
        RotarCam();
    }
    void RotarCam()
    {
        GameObject cameraObject = GameObject.Find("Main Camera");

        if (cameraObject == null)
        {
            Debug.LogError("No se ha encontrado la cámara con el nombre 'Main Camera'.");
            return;
        }

        Vector3 cameraPosition = cameraObject.transform.position;

        Vector3 objectPosition = transform.position;

        Vector3 direction = cameraPosition - objectPosition;
        direction.y = 0;  

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y+180, 0f);
    }
}
