using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public List<GameObject> prefabsFruta = new List<GameObject>();
    public int cantidadPorPrefab = 2;

    public bool autoSpawn = true;
    public float tiempoEntreSpawns = 1f;

    public float radioAntiSolapamiento = 0.5f;
    public bool usarRotacionAleatoria = false;

    private Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>>();
    private CapsuleCollider cap;

    private void Start()
    {
        cap = GetComponent<CapsuleCollider>();
        CrearPool();

        if (autoSpawn)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            IntentarSpawn();
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    void IntentarSpawn()
    {
        GameObject fruta = ObtenerFrutaDisponible();
        if (fruta == null) return;

        Vector3 pos = PosicionValidaDentroCapsule();
        fruta.transform.position = pos;

        fruta.transform.rotation = usarRotacionAleatoria ? Random.rotation : Quaternion.identity;
        fruta.SetActive(true);
    }

    void CrearPool()
    {
        foreach (var prefab in prefabsFruta)
        {
            pool[prefab] = new List<GameObject>();

            for (int i = 0; i < cantidadPorPrefab; i++)
            {
                GameObject fruta = Instantiate(prefab);
                fruta.SetActive(false);

                var r = fruta.AddComponent<FruitReturnToPool>();
                r.spawner = this;
                r.prefabOriginal = prefab;

                pool[prefab].Add(fruta);
            }
        }
    }

    GameObject ObtenerFrutaDisponible()
    {
        List<GameObject> libres = new List<GameObject>();

        foreach (var lista in pool.Values)
            foreach (var fruta in lista)
                if (!fruta.activeInHierarchy)
                    libres.Add(fruta);

        if (libres.Count == 0) return null;

        return libres[Random.Range(0, libres.Count)];
    }

    Vector3 PosicionValidaDentroCapsule()
    {
        for (int i = 0; i < 40; i++)
        {
            Vector3 p = PuntoAleatorioDentroCapsule();

            Collider[] hits = Physics.OverlapSphere(p, radioAntiSolapamiento, ~0, QueryTriggerInteraction.Ignore);

            bool valido = true;

            foreach (var h in hits)
            {
                if (h == cap) continue; // IGNORA el collider del spawner
                valido = false;
                break;
            }

            if (valido)
                return p;
        }

        return transform.position; // fallback
    }


    Vector3 PuntoAleatorioDentroCapsule()
    {
        float height = cap.height;
        float radius = cap.radius;

        float h = (height / 2f) - radius;
        float y = Random.Range(-h, h);

        Vector2 circle = Random.insideUnitCircle * radius;

        Vector3 localPos = new Vector3(circle.x, y, circle.y);

        return transform.TransformPoint(localPos + cap.center);
    }

    public void DevolverFruta(GameObject fruta, GameObject prefab)
    {
        fruta.SetActive(false);
    }
}
