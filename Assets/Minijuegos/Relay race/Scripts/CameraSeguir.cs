using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSeguir : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform target; // El jugador a seguir
    public Vector3 offset = new Vector3(0, 5, -10); // Distancia relativa entre cámara y jugador

    [Header("Suavizado (Menor número = Más rápido)")]
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero; // Requerido por SmoothDamp (mide la velocidad interna de la cámara)

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculamos la posición ideal donde debería estar la cámara
        Vector3 targetPosition = target.position + offset;

        // 2. Movemos la cámara suavemente hacia esa posición adaptándose a la velocidad del objetivo
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
