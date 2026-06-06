using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
    [Header("Base de Datos de Personajes (ScriptableObjects)")]
    [SerializeField] private List<Personajes> listaPersonajes;

    [Header("Contenedores de los Jugadores en el Nivel")]
    // Estos son los objetos que ya están en la escena y tienen el SkinnedMeshRenderer base
    [SerializeField] private Transform[] contenedoresJugadores;

    public void AsignarModeloAJugador(int idPersonaje, int numeroJugador)
    {
        // 1. Validaciones iniciales de índices
        if (idPersonaje < 0 || idPersonaje >= listaPersonajes.Count) return;
        int indiceArray = numeroJugador - 1;
        if (indiceArray < 0 || indiceArray >= contenedoresJugadores.Length) return;

        Transform contenedorObjetivo = contenedoresJugadores[indiceArray];
        if (contenedorObjetivo == null) return;

        // 2. Obtener los datos del ScriptableObject
        Personajes datos = listaPersonajes[idPersonaje];

        // 3. CAMBIAR MESH Y MATERIAL (Buscamos en los hijos del contenedor)
        // Usamos GetComponentInChildren porque la malla suele estar en un objeto hijo del "Rig"
        SkinnedMeshRenderer skinnedRenderer = contenedorObjetivo.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedRenderer != null)
        {
            // Cambiamos la malla animada (sharedMesh)
            if (datos.Malla != null) skinnedRenderer.sharedMesh = datos.Malla;

            // Cambiamos el material
            if (datos.Mat != null) skinnedRenderer.material = datos.Mat;
        }
        else
        {
            Debug.LogError($"No se encontró un SkinnedMeshRenderer en el Jugador {numeroJugador}. ¡Asegúrate de que el prefab base lo tenga!");
        }

        // 4. CAMBIAR LA ESCALA DEL OBJETO
        // Cambiamos la escala local del objeto visual (el hijo que tiene el Renderer)
        if (skinnedRenderer != null)
        {
<<<<<<< HEAD
            Transform hijo = contenedorObjetivo.GetChild(i);

            // SI el hijo tiene la etiqueta del modelo, lo borramos
            if (hijo.CompareTag(etiquetaModelo))
            {
                Destroy(hijo.gameObject);
            }
            // Si no tiene la etiqueta (tu objeto del minijuego), el script lo ignora y lo deja vivo
        }

        // 4. INSTANCIAR EL NUEVO PREFAB
        GameObject nuevoModelo = Instantiate(datos.PrefabPersonaje, contenedorObjetivo);

        // Nos aseguramos de que el nuevo modelo clonado tenga la etiqueta correcta para el futuro
        nuevoModelo.tag = etiquetaModelo;

        Vector3 escalaFinal, posicionFinal, rotacionFinal;
        scalaPersonajesScript.ObtenerTransformaciones(idPersonaje, out escalaFinal, out posicionFinal, out rotacionFinal,numeroJugador);

        // Reseteamos posición y rotación local
        nuevoModelo.transform.localPosition = posicionFinal;
        nuevoModelo.transform.localRotation = Quaternion.Euler(rotacionFinal);
        nuevoModelo.transform.localScale = escalaFinal;

        if (scalaPersonajesScript.juegoActual == ScalaPersonajes.Minijuegos.HudRecompnesas)
            nuevoModelo.gameObject.AddComponent<CapsuleCollider>();

        // 5. CAMBIAR EL MATERIAL
        SkinnedMeshRenderer skinnedRenderer = nuevoModelo.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedRenderer != null && datos.Mat != null)
        {
            skinnedRenderer.material = datos.Mat;
        }
        else if (datos.Mat != null)
        {
            MeshRenderer meshRenderer = nuevoModelo.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.material = datos.Mat;
        }

        // 7. ASIGNAR EL ANIMATOR CONTROLLER
        Animator animatorComponent = nuevoModelo.GetComponent<Animator>();
        if (animatorComponent == null)
        {
            animatorComponent = nuevoModelo.GetComponentInChildren<Animator>();
=======
            // Aplicamos la escala del ScriptableObject directamente al objeto de la malla
            skinnedRenderer.gameObject.transform.localScale = datos.Scalar;
>>>>>>> fa94ef1524621a01afd66d372f373ec996ba99a1
        }

        // 5. ASIGNAR EL ANIMATOR CONTROLLER (El cerebro de las animaciones)
        Animator animatorComponent = contenedorObjetivo.GetComponentInChildren<Animator>();
        if (animatorComponent != null && datos.Animator != null)
        {
            animatorComponent.runtimeAnimatorController = datos.Animator;
        }

        // 6. INYECTAR EL SCRIPT DE CONTROL DE ANIMACIONES (Si no lo tiene ya)
        // Como el objeto ya existe en la escena, verificamos si ya tiene el script para no duplicarlo
        if (skinnedRenderer != null && skinnedRenderer.gameObject.GetComponent<AnimatorMinijuegosController>() == null)
        {
            skinnedRenderer.gameObject.AddComponent<AnimatorMinijuegosController>();
        }


    }
}
