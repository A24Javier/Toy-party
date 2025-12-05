using UnityEngine;

public class FruitReturnToPool : MonoBehaviour
{
    [HideInInspector] public FruitSpawner spawner;
    [HideInInspector] public GameObject prefabOriginal;

    private void OnDisable()
    {
        if (spawner != null)
            spawner.DevolverFruta(gameObject, prefabOriginal);
    }
}
