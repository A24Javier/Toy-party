using System.Collections.Generic;
using UnityEngine;

#region Explanation
/*
 * Este script se encarga unicamente de generar
 * el recorrido.
 * También tiene una función para eliminar todo
 * el recorrido instanciado, en caso de que sea
 * necesario.
 */
#endregion

public class RelayPathInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabStart;
    [SerializeField]
    private GameObject _prefabFinish;
    [SerializeField]
    private GameObject _prefabPath;
    [SerializeField]
    private GameObject _prefabRelay;

    [Space(10)]

    [SerializeField]
    private Transform _pathParent;

    [SerializeField]
    [Range(2, 20)]
    private int _pathLength;

    [SerializeField]
    private int _relays = 2;

    [Space(10)]

    [SerializeField]
    private float _distanceBetweenPaths = 10f;

    [SerializeField]
    private Vector3 _startPosition = Vector3.zero;

    private List<GameObject> _listPathGO = new List<GameObject>();

    void Start()
    {
        CreatePath();
    }

    private void CreatePath()
    {
        if (_listPathGO.Count > 0)
            DeletePath();

        // Creamos la linea de salida
        GameObject startGO = Instantiate(_prefabStart, _startPosition, Quaternion.identity, _pathParent);
        _listPathGO.Add(startGO);

        Vector3 finalPos = Vector3.zero;

        for(int i = 0; i < _relays; i++)
        {
            // Creamos el resto del camino
            for (int j = 0; j < _pathLength; j++)
            {
                Vector3 pathPos = Vector3.one * (_distanceBetweenPaths * (j + 1)) + (finalPos * i);
                pathPos.x = 0;
                pathPos.y = 0;

                GameObject pathGO = Instantiate(_prefabPath, pathPos, Quaternion.identity, _pathParent);
                _listPathGO.Add(pathGO);
            }

            // Creamos la linea de meta/relay
            finalPos = Vector3.one * ((_distanceBetweenPaths * (_pathLength + 2)) * (i+1));
            finalPos.x = 0;
            finalPos.y = 0;

            GameObject finishGO = null;

            if (i == (_relays-1))
                finishGO = Instantiate(_prefabFinish, finalPos, Quaternion.identity, _pathParent);
            else
                finishGO = Instantiate(_prefabRelay, finalPos, Quaternion.identity, _pathParent);


            _listPathGO.Add(finishGO);
        }

        RaceController.Instance.Initialize();
    }

    private void DeletePath()
    {
        for(int i = (_listPathGO.Count - 1); i >= 0; i--)
        {
            GameObject pathGO = _listPathGO[i];
            Destroy(pathGO);
        }

        _listPathGO.Clear();
    }
}
