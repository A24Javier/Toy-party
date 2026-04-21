using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonRouletteRandom : MonoBehaviour
{
    [SerializeField] private List<Button> botones = new List<Button>();
    [SerializeField] private float duracionTotal = 2f;
    [SerializeField] private float tiempoCambio = 0.08f;

    private bool girando = false;

    public void EmpezarRuleta()
    {
        if (girando) return;
        if (botones == null || botones.Count == 0) return;

        StartCoroutine(Ruleta());
    }

    private IEnumerator Ruleta()
    {
        girando = true;

        int indiceActual = Random.Range(0, botones.Count);
        float tiempo = 0f;

        while (tiempo < duracionTotal)
        {
            if (botones[indiceActual] != null)
            {
                EventSystem.current.SetSelectedGameObject(botones[indiceActual].gameObject);
            }

            yield return new WaitForSeconds(tiempoCambio);
            tiempo += tiempoCambio;

            indiceActual = ObtenerIndiceAleatorioDistinto(indiceActual);
        }

        int indiceFinal = Random.Range(0, botones.Count);
        Button botonFinal = botones[indiceFinal];

        EventSystem.current.SetSelectedGameObject(botonFinal.gameObject);
        yield return new WaitForSeconds(0.05f);
        botonFinal.onClick.Invoke();

        girando = false;
    }

    private int ObtenerIndiceAleatorioDistinto(int indiceActual)
    {
        if (botones.Count <= 1) return indiceActual;

        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, botones.Count);
        }
        while (nuevoIndice == indiceActual);

        return nuevoIndice;
    }
}