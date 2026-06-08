using UnityEngine;

#region Explanation
/*
 * Este script se encarga unicamente de instanciar
 * la valla dentro de cada prefab del recorrido.
 * La valla se instanciara dentro del rango que
 * tu determines en _minZSpawn y _maxZSpawn.
 */
#endregion

public class RecorridoScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabFence;

    [SerializeField]
    [Range(-5f, 5f)]
    private float _minZSpawn, _maxZSpawn; 

    void Start()
    {
        float randZ = Random.Range(_minZSpawn, _maxZSpawn);
        Vector3 spawnPos = new Vector3(0f, 0.47f, randZ);
        Transform fenceTransf = Instantiate(_prefabFence, Vector3.zero, _prefabFence.transform.rotation, transform).GetComponent<Transform>();
        fenceTransf.localPosition = spawnPos;
    }
}
