using UnityEngine;

public class FruitReset : MonoBehaviour
{
    private FruitSpawner spawner;
    public GameObject prefabOriginal;

    private void Start()
    {
        spawner = FindObjectOfType<FruitSpawner>();
        if (spawner == null)
        {
            Debug.LogError("No se encontró un FruitSpawner en la escena");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reset")|| other.CompareTag("Cesta1"))
        {
            if (spawner != null)
                spawner.DevolverFruta(gameObject, prefabOriginal);
        }
    }
}
