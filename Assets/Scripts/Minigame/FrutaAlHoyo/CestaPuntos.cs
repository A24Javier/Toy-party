using UnityEngine;
using TMPro;

public class CestaPuntos : MonoBehaviour
{
    public TMP_Text textoPuntos;
    public int puntos = 0;

    private void Start()
    {
        textoPuntos.text = "Puntos: " + puntos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pear"))
        {
            puntos++;
            textoPuntos.text = "Puntos: " + puntos;

            other.gameObject.SetActive(false);
        }
    }
}
