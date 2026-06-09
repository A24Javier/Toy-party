using System.Collections.Generic;
using UnityEngine;

#region Explanation
/*
 * Este script se encarga unicamente de generar
 * el recorrido.
 * Tambi�n tiene una funci�n para eliminar todo
 * el recorrido instanciado, en caso de que sea
 * necesario.
 */
#endregion

public class RelayPathInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject _prefabStart;
    [SerializeField] private GameObject _prefabFinish;
    [SerializeField] private GameObject _prefabPath;
    [SerializeField] private GameObject _prefabRelay;

    [Space(10)]
    [SerializeField] private Transform _pathParent;
    [SerializeField][Range(2, 20)] private int _pathLength;
    [SerializeField] private int _relays = 2;

    [Space(10)]
    [SerializeField] private float _distanceBetweenPaths = 10f;
    [SerializeField] private Vector3 _startPosition = Vector3.zero;

    [Header("Configuración Multipista")]
    [SerializeField] private int _totalTracks = 2; // Cuántas carreras paralelas quieres (ej: 2 para split-screen)
    [SerializeField] private float _distanceBetweenTracks = 20f; // Separación en el eje X entre pistas

    private List<GameObject> _listPathGO = new List<GameObject>();

    private List<Transform> _relevosPista1 = new List<Transform>();
    private List<Transform> _relevosPista2 = new List<Transform>();

    void Start()
    {
        _relevosPista1.Clear();
        _relevosPista2.Clear();

        for (int t = 0; t < _totalTracks; t++)
        {
            CreatePath(t);
        }
    }

    private void CreatePath(int trackIndex)
    {
        float offsetX = trackIndex * _distanceBetweenTracks;
        Vector3 trackStartPos = _startPosition + new Vector3(offsetX, 0, 0);

        GameObject startGO = Instantiate(_prefabStart, trackStartPos, Quaternion.identity, _pathParent);
        _listPathGO.Add(startGO);

        Vector3 finalPos = Vector3.zero;

        for (int i = 0; i < _relays; i++)
        {
            for (int j = 0; j < _pathLength; j++)
            {
                Vector3 pathPos = Vector3.one * (_distanceBetweenPaths * (j + 1)) + (finalPos * i);
                pathPos.x = trackStartPos.x;
                pathPos.y = 0;

                GameObject pathGO = Instantiate(_prefabPath, pathPos, Quaternion.identity, _pathParent);
                _listPathGO.Add(pathGO);
            }

            finalPos = Vector3.one * ((_distanceBetweenPaths * (_pathLength + 2)) * (i + 1));
            finalPos.x = trackStartPos.x;
            finalPos.y = 0;

            GameObject finishGO = null;

            if (i == (_relays - 1))
                finishGO = Instantiate(_prefabFinish, finalPos, Quaternion.identity, _pathParent);
            else
                finishGO = Instantiate(_prefabRelay, finalPos, Quaternion.identity, _pathParent);

            // ¡AQUÍ GUARDAMOS EL RELEVO EN LA LISTA CORRECTA SEGÚN LA PISTA!
            if (trackIndex == 0)
                _relevosPista1.Add(finishGO.transform);
            else if (trackIndex == 1)
                _relevosPista2.Add(finishGO.transform);

            _listPathGO.Add(finishGO);
        }
    }
}