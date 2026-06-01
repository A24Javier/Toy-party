using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class RaceController : MonoBehaviour
{
    [Header("Start race cooldown")]
    [SerializeField]
    [Range(1, 10)]
    private int _cooldownSeconds = 3;

    [SerializeField]
    private TMP_Text _textCooldown;

    [SerializeField]
    private LocalizedString _startLocalized;

    [Space(10)]

    [Header("Camera player show")]
    [SerializeField]
    private Vector3 _relayCamOffset;

    [SerializeField]
    private Camera _mainCamera;

    private Transform[] _relayPositions;

    [SerializeField]
    [Range(0.25f, 8f)]
    private float _secondsWaitingInRelay = 1.5f;

    [SerializeField]
    [Range(0.1f, 30f)]
    private float _cameraSpeed = 10f;

    [Header("Player values")]
    [SerializeField]
    private GameObject _prefabPlayer;
    public int TotalPlayers = 4;

    [SerializeField]
    [Range(-3.8f, -0.05f)]
    private float _minXPlayerSpawn;

    [SerializeField]
    [Range(0.05f, 3.8f)]
    private float _maxXPlayerSpawn;

    [SerializeField]
    private Vector3 _playerInstantiateOffset;

    [Header("Other")]
    public static RaceController Instance;

    void Awake()
    {
        _textCooldown.text = "";
        if(Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Initialize()
    {
        GameObject[] relaysGO = GameObject.FindGameObjectsWithTag("Path");
        _relayPositions = new Transform[relaysGO.Length];

        for(int i = 0; i < _relayPositions.Length; i++)
            _relayPositions[i] = relaysGO[i].GetComponent<Transform>();

        // Ponemos un player en el inicio de la carrera y otro en uno
        // en cada relay
        int playersPerRelay = TotalPlayers / (_relayPositions.Length - 1);
        for(int i = 0; i < TotalPlayers; i++)
        {
            int relayIndex = i % (_relayPositions.Length - 1);

            Vector3 playerSpawn = _relayPositions[relayIndex].position + _playerInstantiateOffset;

            float t = playersPerRelay <= 1 
                ? 0.5f
                : (float)(i / (_relayPositions.Length - 1)) / (playersPerRelay - 1);


            float offsetX = Mathf.Lerp(_minXPlayerSpawn, _maxXPlayerSpawn, t);

            playerSpawn.x += offsetX;

            Instantiate(_prefabPlayer, playerSpawn, Quaternion.identity);
        }

        StartCoroutine(ShowRelays());
    }

    // Función que muestra un poco el escenario y todos los relays (donde se
    // efectua el cambio de personaje)
    private IEnumerator ShowRelays()
    {
        _mainCamera.transform.position = _relayPositions[0].position + _relayCamOffset;

        for(int i = 0; i < _relayPositions.Length; i++)
        {
            Vector3 target = _relayPositions[i].position + _relayCamOffset;

            while (Vector3.Distance(_mainCamera.transform.position, target) > 0.01f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(
                _mainCamera.transform.position,
                target,
                _cameraSpeed * Time.deltaTime);

                yield return null;
            }
            
            yield return new WaitForSeconds(_secondsWaitingInRelay);
        }

        StartCoroutine(StartCooldownCoroutine());
    }

    private IEnumerator StartCooldownCoroutine()
    {
        _mainCamera.transform.position = _relayPositions[0].position + _relayCamOffset;

        for(int i = _cooldownSeconds; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);

            if (i > 0)
                _textCooldown.text = i.ToString("0") + "...";
            else
                _textCooldown.text = _startLocalized.GetLocalizedString();
        }

        yield return new WaitForSeconds(0.25f);
        _textCooldown.text = "";
        // Empezar carrera


    }


}
