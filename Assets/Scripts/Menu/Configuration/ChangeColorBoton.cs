
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorBoton : MonoBehaviour
{
    public Button boton1;
    public Button boton2;
    public Button boton3;

    public void ChangeColor()
    {
        Color colorOscuro = new Color32(0, 0, 0, 237);

        Color colorClaro = new Color32(110, 85, 85, 237);

        CambiarColor(boton1, colorOscuro);
        CambiarColor(boton2, colorClaro);
        CambiarColor(boton3, colorClaro);
    }

    void CambiarColor(Button boton, Color color)
    {
        ColorBlock cb = boton.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        boton.colors = cb;
    }
}