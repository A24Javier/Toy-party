using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectArrow
{
    public CanvasGroup ArrowCanvasGroup;
    public Button ArrowButton;
}

public class TheTowerSelectPath : MonoBehaviour
{
    private CanvasGroup _canvasPath;
    [SerializeField] private SelectArrow _leftArrow, _rightArrow, _forwardArrow, _downArrow;

    private Box[] _paths;
    private int _pathIndex;
    private bool _pathSelected = false;
    private BoardCameraController _cameraController;

    [SerializeField] private GameObject _theTowerPrefab;
    private GameObject _instanciedTower;
    [SerializeField] private float _towerOffsetY;

    public static TheTowerSelectPath Instance { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _cameraController = FindAnyObjectByType<BoardCameraController>();
    }

    public void StartSelectPathTower()
    {
        _canvasPath.alpha = 1f;
        _canvasPath.blocksRaycasts = true;
        _canvasPath.interactable = true;

        StartCoroutine(TowerPathSelection());
    }

    private IEnumerator TowerPathSelection()
    {
        Box selectedPath = null;
        GameObject[] pathsGO = GameObject.FindGameObjectsWithTag("Path");

        _paths = new Box[pathsGO.Length];

        for(int i = 0; i < pathsGO.Length; i++)
        {
            _paths[i] = pathsGO[i].GetComponent<Box>();
        }

        Box actualPath = _paths[0];
        _cameraController.SetTarget(actualPath.transform);
        SetArrows(actualPath);

        while(!_pathSelected)
        {
            yield return null;
        }
    }

    private void SetArrows(Box pathBox)
    {
        

    }

    private void ShowNextPath(bool next)
    {
        _pathIndex = next ? _pathIndex++ : _pathIndex--;

        if(_paths.Length <= _pathIndex)
        {
            _pathIndex = 0;
        }
        else if(_pathIndex < 0)
        {
            _pathIndex = _paths.Length - 1;
        }

        _cameraController.SetTarget(_paths[_pathIndex].transform);
    }

    private void SelectPath()
    {
        _pathSelected = true;
    }

    private void SelectRoad(Box selectedPath)
    {
        _instanciedTower = Instantiate(_theTowerPrefab, selectedPath.transform.position + (Vector3.up * _towerOffsetY), Quaternion.identity);

        // Devolver la camara al jugador actual
    }
}
