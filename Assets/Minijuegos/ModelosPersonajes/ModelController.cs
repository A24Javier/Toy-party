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

    [Header("Configuración de Seguridad")]
    [Tooltip("El script solo borrará los hijos que tengan esta etiqueta para no romper el minijuego.")]
    [SerializeField] private string etiquetaModelo = "Player";

    [SerializeField] private ScalaPersonajes scalaPersonajesScript;

    public void AsignarModeloAJugador(int idPersonaje, int numeroJugador, Image Perfil)
    {
        // 1. Validaciones iniciales de índices
        if (idPersonaje < 0 || idPersonaje >= listaPersonajes.Count) return;
        int indiceArray = numeroJugador - 1;
        if (indiceArray < 0 || indiceArray >= contenedoresJugadores.Length) return;

        Transform contenedorObjetivo = contenedoresJugadores[indiceArray];
        if (contenedorObjetivo == null) return;

        // 2. Obtener los datos del ScriptableObject
        Personajes datos = listaPersonajes[idPersonaje];
        if (datos.PrefabPersonaje == null) return;

        if (Perfil != null && datos.ImgPerfil != null)
        {
            Perfil.sprite = datos.ImgPerfil;
        }

        // 3. LIMPIEZA QUIRÚRGICA (NUEVO)
        // En lugar de borrar todo, solo borramos el hijo que sea el "Modelo Visual" anterior
        for (int i = contenedorObjetivo.childCount - 1; i >= 0; i--)
        {
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
        scalaPersonajesScript.ObtenerTransformaciones(idPersonaje, out escalaFinal, out posicionFinal, out rotacionFinal);

        // Reseteamos posición y rotación local
        nuevoModelo.transform.localPosition = posicionFinal;
        nuevoModelo.transform.localRotation = Quaternion.Euler(rotacionFinal);
        nuevoModelo.transform.localScale = escalaFinal;

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

        if (animatorComponent != null && datos.Animator != null)
        {
            animatorComponent.runtimeAnimatorController = datos.Animator;
        }

        // 8. INYECTAR EL SCRIPT DE CONTROL DE ANIMACIONES
        if (animatorComponent != null && nuevoModelo.GetComponent<AnimatorMinijuegosController>() == null)
        {
            animatorComponent.gameObject.AddComponent<AnimatorMinijuegosController>();
        }
    }
}
