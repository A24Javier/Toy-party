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
        SkinnedMeshRenderer skinnedRenderer = contenedorObjetivo.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedRenderer != null)
        {
            if (datos.Malla != null) skinnedRenderer.sharedMesh = datos.Malla;
            if (datos.Mat != null) skinnedRenderer.material = datos.Mat;
        }
        else
        {
            Debug.LogError($"No se encontró un SkinnedMeshRenderer en el Jugador {numeroJugador}. ¡Asegúrate de que el prefab base lo tenga!");
        }

        // 4. CAMBIAR LA ESCALA DEL OBJETO
        if (skinnedRenderer != null)
        {
            Transform hijo = contenedorObjetivo.GetChild(i);

            // SI el hijo tiene la etiqueta del modelo, lo borramos
            if (hijo.CompareTag(etiquetaModelo))
            {
                Destroy(hijo.gameObject);
            }
        }

        // 4. INSTANCIAR EL NUEVO PREFAB
        GameObject nuevoModelo = Instantiate(datos.PrefabPersonaje, contenedorObjetivo);

        nuevoModelo.tag = etiquetaModelo;

        Vector3 escalaFinal, posicionFinal, rotacionFinal;

        scalaPersonajesScript.ObtenerTransformaciones(
            idPersonaje,
            out escalaFinal,
            out posicionFinal,
            out rotacionFinal,
            numeroJugador
        );

        nuevoModelo.transform.localPosition = posicionFinal;
        nuevoModelo.transform.localRotation = Quaternion.Euler(rotacionFinal);
        nuevoModelo.transform.localScale = escalaFinal;

        if (scalaPersonajesScript.JuegoActual == ScalaPersonajes.Minijuegos.HudRecompensas)
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
        }

        // 5. ASIGNAR EL ANIMATOR CONTROLLER (El cerebro de las animaciones)
        Animator animatorComponent2 = contenedorObjetivo.GetComponentInChildren<Animator>();
        if (animatorComponent2 != null && datos.Animator != null)
        {
            animatorComponent2.runtimeAnimatorController = datos.Animator;
        }

        // 6. INYECTAR EL SCRIPT DE CONTROL DE ANIMACIONES
        if (skinnedRenderer != null && skinnedRenderer.gameObject.GetComponent<AnimatorMinijuegosController>() == null)
        {
            skinnedRenderer.gameObject.AddComponent<AnimatorMinijuegosController>();
        }
    }
}