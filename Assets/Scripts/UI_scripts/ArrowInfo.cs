using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Box boxArranged;
    private Vector3 nodeArranged;
    private Player player;

    private Transform lastTarget = null;
    private BoardCameraController _cameraController;
    private EventSystem _eventSystem;

    private bool _arrowActive = false;
    private bool _isShowingPath = false;

    private void Start()
    {
        UIManager.instance.OnArrowSelected.AddListener(delegate { _arrowActive = false; });

        _cameraController = FindAnyObjectByType<BoardCameraController>();
        _eventSystem = FindFirstObjectByType<EventSystem>();
    }

    void Update()
    {
        if (!_arrowActive)
            return;

        if (_eventSystem.currentSelectedGameObject == gameObject)
        {
            if (!_isShowingPath)
            {
                Debug.Log($"Mostrar camino. Object name: {gameObject.name}");
                ShowPath();
            }

        }
        else
        {
            if (_isShowingPath)
            {
                Debug.Log($"No mostrar camino. Object name: {gameObject.name}");
                UnshowPath();
            }
            
        }
    }

    public void ArrangeVector(Vector3 box)
    {
        nodeArranged = box;
    }

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    public void SetBox(Box box)
    {
        boxArranged = box;
        GetComponent<Button>().onClick.AddListener(SelectArrow);
        _arrowActive = true;
    }

    public void SelectArrow()
    {
        _cameraController.SetTarget(GameController.instance.GetPlayerOfTurn().transform);
        StartCoroutine(player.PathSelected(nodeArranged, boxArranged));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _eventSystem.SetSelectedGameObject(gameObject);

        if (!_isShowingPath)
        {
            ShowPath();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isShowingPath)
        {
            UnshowPath();
        }
        
    }

    private void ShowPath()
    {
        lastTarget = _cameraController.GetActualTarget();
        _cameraController.SetTarget(boxArranged.transform);
        _isShowingPath = true;
    }
    
    private void UnshowPath()
    {
        //_cameraController.SetTarget(lastTarget);
        _isShowingPath = false;

    }
}
