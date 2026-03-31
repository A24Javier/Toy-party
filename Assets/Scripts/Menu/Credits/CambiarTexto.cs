using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CambiarTextos : MonoBehaviour
{
    [SerializeField] private TMP_Text Titulo;
    [SerializeField] private TMP_Text Descripcion;
    [SerializeField] private Image imagen;
    [SerializeField] private string nuevoTitulo;
    [SerializeField] private string nuevaDescripcion;
    [SerializeField] private Sprite nuevaImagen;

    private void Awake()
    {
        if (Titulo != null)
            Titulo.text = nuevoTitulo;

        if (Descripcion != null)
            Descripcion.text = nuevaDescripcion;

        if (imagen != null)
            imagen.sprite = nuevaImagen;
    }


}