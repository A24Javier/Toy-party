using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectArrow
{
    public GameObject Object;
    public CanvasGroup CanvasGroup;
    public Button ArrowButton;

    public void CanvasControl(bool activate)
    {
        CanvasGroup.alpha = activate ? 1 : 0;
        CanvasGroup.interactable = activate;
        CanvasGroup.blocksRaycasts = activate;
    }
}

public class TheTowerSelectPath : MonoBehaviour
{
    private CanvasGroup _canvasPath;
    [SerializeField] private Button _backArrow, _nextArrow;

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
        _canvasPath = GetComponent<CanvasGroup>();
        _cameraController = FindAnyObjectByType<BoardCameraController>();
    }

    public void StartSelectPathTower()
    {
        _canvasPath.alpha = 1f;
        _canvasPath.blocksRaycasts = true;
        _canvasPath.interactable = true;

        // Quitar ActionPanel

        StartCoroutine(TowerPathSelection());
    }

    private IEnumerator TowerPathSelection()
    {
        Box selectedPath = null;
        GameObject[] pathsGO = GameObject.FindGameObjectsWithTag("Path");

        _paths = new Box[pathsGO.Length];
        _pathIndex = 0;

        for(int i = 0; i < pathsGO.Length; i++)
        {
            _paths[i] = pathsGO[i].GetComponent<Box>();
        }

        Box actualPath = _paths[0];
        _cameraController.SetTarget(actualPath.transform);
        PrepareButtons();

        while(!_pathSelected)
        {
            yield return null;
        }

        // Cosas que deben pasar por seleccionar camino
    }

    private void PrepareButtons()
    {
        SetArrows(_paths[_pathIndex]);

        // Setear los botones de selección
        _backArrow.onClick.RemoveAllListeners();
        _nextArrow.onClick.RemoveAllListeners();

        _backArrow.onClick.AddListener(delegate { ShowNextPath(false); PrepareButtons(); });
        _nextArrow.onClick.AddListener(delegate { ShowNextPath(true); PrepareButtons(); });
    }

    private void SetArrows(Box pathBox)
    {
        Vector3 pathTransf = pathBox.transform.position;

        for (int i = 0; i < pathBox.PossiblesBoxesCount(); i++)
        {
            Vector3 possBoxTransf = pathBox.GetBoxTransf(i).position;
            Box possBox = pathBox.GetNewBox(i);

            float angle = CalculateAngle(pathTransf, possBoxTransf);
            angle += Camera.main.transform.eulerAngles.y;

            bool arrowActivated = false;

            // Verificar dirección y activar la flecha correspondiente
            if ((angle <= 45f && angle > -45f || angle >= 360f && angle > 45f) && _rightArrow.CanvasGroup.alpha == 0 && !arrowActivated)
            {
                _rightArrow.CanvasControl(true);
                _rightArrow.Object.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

                // Aplicamos logica al botón
                _rightArrow.ArrowButton.onClick.AddListener(delegate {
                    SelectRoad(possBox);
                });

                continue;
            }
            else if ((angle > 135f && angle < 225f) && _leftArrow.CanvasGroup.alpha == 0 && !arrowActivated)
            {
                _leftArrow.CanvasControl(true);
                _leftArrow.Object.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

                // Aplicamos logica al botón
                _leftArrow.ArrowButton.onClick.AddListener(delegate {
                    SelectRoad(possBox);
                });

                continue;
            }

            if ((angle < 135f && angle > 45f) && _forwardArrow.CanvasGroup.alpha == 0 && !arrowActivated)
            {
                _forwardArrow.CanvasControl(true);
                _forwardArrow.Object.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

                // Aplicamos logica al botón
                _forwardArrow.ArrowButton.onClick.AddListener(delegate {
                    SelectRoad(possBox);
                });

            }
            else if ((angle < 315f && angle > 225f || angle < -45f && angle > -135f) && _downArrow.CanvasGroup.alpha == 0 && !arrowActivated)
            {
                _downArrow.CanvasControl(true);
                _downArrow.Object.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

                // Aplicamos logica al botón
                _downArrow.ArrowButton.onClick.AddListener(delegate {
                    SelectRoad(possBox);
                });
            }
        }
    }

    private float CalculateAngle(Vector3 P1, Vector3 P2)
    {
        Vector3 direction = (P2 - P1);
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        return angle;
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

    private void SelectRoad(Box selectedPath)
    {
        _pathSelected = true;

        Player player = GameController.instance.GetPlayerOfTurn();

        _instanciedTower = Instantiate(_theTowerPrefab, selectedPath.transform.position + (Vector3.up * _towerOffsetY), Quaternion.identity);

        // Devolver la camara al jugador actual

    }
}
