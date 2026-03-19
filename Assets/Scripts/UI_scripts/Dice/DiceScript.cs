using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class DiceScript : MonoBehaviour
{
    #region Old Version
    /*
    // La idea de los dados es que esten en un pool. De ahí que use el OnEnable

    [SerializeField] private Dice scriptableDice;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [SerializeField] private TMP_Text numberTxt;

    private bool isRotating = false;
    private bool npcRolled = false;
    private float randomAngle;


    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    

    void OnEnable()
    {
        SetDiceSettings();
    }

    void Update()
    {
        if (isRotating)
        {
            
            DiceRotation();
        }
        
    }





    /// <summary>
    /// Función para que el dado del jugador ruede y que cuando deje de rodar el jugador se mueva
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public IEnumerator DiceRolling(Player player)
    {
        int min = scriptableDice.minNumber, max = scriptableDice.maxNumber;
        int randNumber = Random.Range(min, max+1);
        int lastNumber = randNumber;
        numberTxt.text = randNumber.ToString("0");
        isRotating = true;

        while (!InputHandler.instance.IsSpacebarTouched())
        {
            randNumber = Random.Range(min, max+1);
            while(randNumber == lastNumber)
            {
                randNumber = Random.Range(min, max + 1);
            }
            lastNumber = randNumber;
            numberTxt.text = randNumber.ToString("0");

            // Cosas rotación
            float randomSpeed = 45f * Random.Range(0.8f, 1.2f);
            randomAngle = randomSpeed * scriptableDice.changeNumSpeed;

            yield return new WaitForSeconds(scriptableDice.changeNumSpeed);
        }

        isRotating = false;

        player.Move(randNumber);
        yield return null;
        Destroy(transform.parent.gameObject);
    }

    /// <summary>
    /// Función para que el dado del NPC ruede y que cuando deje de rodar el NPC se mueva
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public IEnumerator DiceRolling(NPC_Controller npc)
    {
        int min = scriptableDice.minNumber, max = scriptableDice.maxNumber;
        int randNumber = Random.Range(min, max+1);
        int lastNumber = randNumber;
        numberTxt.text = randNumber.ToString("0");
        isRotating = true;

        while (!npcRolled)
        {
            randNumber = Random.Range(min, max+1);
            while(randNumber == lastNumber)
            {
                randNumber = Random.Range(min, max + 1);
            }
            lastNumber = randNumber;
            numberTxt.text = randNumber.ToString("0");

            // Cosas rotación
            float randomSpeed = 45f * Random.Range(0.8f, 1.2f);
            randomAngle = randomSpeed * scriptableDice.changeNumSpeed;

            yield return new WaitForSeconds(scriptableDice.changeNumSpeed);
        }

        isRotating = false;

        npc.Move(randNumber);
        Debug.Log("Destruimos dado");
        Destroy(transform.parent.gameObject);
    }

    /// <summary>
    /// Función para que el dado ruede
    /// </summary>
    private void DiceRotation()
    {
        gameObject.transform.Rotate(-Vector3.up, randomAngle, Space.Self);
    }

    /// <summary>
    /// Función para cambiar el tipo de dado y sus propiedades
    /// </summary>
    /// <param name="scrDice"></param>
    public void ChangeScriptableDice(Dice scrDice)
    {
        scriptableDice = scrDice;
        SetDiceSettings();
    }

    private void SetDiceSettings()
    {
        meshFilter.mesh = scriptableDice.diceMesh;
        meshRenderer.material = scriptableDice.diceMat;
    }

    /// <summary>
    /// Función para que cuando el NPC decida parar el dado, lo pare
    /// </summary>
    public void NPC_Roll()
    {
        npcRolled = true;
    }*/
    #endregion
    [SerializeField] private float _minRotX, _maxRotX;
    [SerializeField] private float _rotSpeed;
    private bool _isRotating;
    [SerializeField] private bool _isGoingToMaxRotX = false;

    [SerializeField] private float _minNpcTime = 1.25f;
    [SerializeField] private float _maxNpcTime = 3f;

    private int _minNum, _maxNum;
    private float _numChangeSpeed;
    [SerializeField] private int _maxChangeNumAttemps = 100;
    [SerializeField] private float _upDistance = 1f;

    private GameObject _instanciedDice;
    private List<TMP_Text> _diceNums = new List<TMP_Text>();

    public static DiceScript Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetupDice(Dice dice, bool isNPC)
    {
        _isRotating = false;

        _minNum = dice.minNumber;
        _maxNum = dice.maxNumber+1;
        _numChangeSpeed = dice.changeNumSpeed;

        Vector3 diceSpawnPos = GameController.instance.GetCharacterOfTurn().transform.position + (Vector3.up * _upDistance);
        _instanciedDice = Instantiate(dice.diceObject, diceSpawnPos, Quaternion.identity, transform);

        for(int i = 0; i < _instanciedDice.transform.childCount; i++)
        {
            _diceNums.Add(_instanciedDice.transform.GetChild(i).GetComponent<TMP_Text>());
        }

        StartCoroutine(DiceRolling(isNPC));
    }

    void Update()
    {
        if (!_isRotating)
            return;

        _instanciedDice.transform.Rotate(Vector3.up * _rotSpeed, Space.Self);

        Vector3 newAngle = new Vector3(0f, _instanciedDice.transform.rotation.y, 0f);

        if (_isGoingToMaxRotX)
        {
            newAngle += Vector3.right * _rotSpeed;

            if (_instanciedDice.transform.rotation.y > _maxRotX)
                _isGoingToMaxRotX = false;
        }
        else
        {
            newAngle += Vector3.left * _rotSpeed;

            if (_instanciedDice.transform.rotation.y <= _minRotX)
                _isGoingToMaxRotX = true;

        }

        _instanciedDice.transform.rotation = new Quaternion(newAngle.x, newAngle.y, newAngle.z, 0f);
    }

    private IEnumerator DiceRolling(bool isNPC)
    {
        int randNumber = Random.Range(_minNum, _maxNum);
        int lastNumber = randNumber;

        float npcTime = 0f;

        if (isNPC)
            npcTime = Random.Range(_minNpcTime, _maxNpcTime);

        SetDiceNums(randNumber);

        float timeElapsedNumChange = 0f;

        _isRotating = true;

        while ((!InputHandler.instance.IsSpacebarTouched() && !isNPC) || (timeElapsedNumChange < npcTime && isNPC))
        {
            if(timeElapsedNumChange < _numChangeSpeed)
            {
                timeElapsedNumChange += Time.deltaTime;
            }
            else if(timeElapsedNumChange >= _numChangeSpeed)
            {
                int attemps = 0;

                do
                {
                    randNumber = Random.Range(_minNum, _maxNum);
                    attemps++;
                } while(randNumber == lastNumber && attemps < _maxChangeNumAttemps);

                lastNumber = randNumber;

                SetDiceNums(randNumber);

                timeElapsedNumChange = 0f;
            }

            yield return null;
        }

        _isRotating = false;
        _isGoingToMaxRotX = false;

        StartCoroutine(ShowDiceNum(randNumber));
    }

    private void SetDiceNums(int number)
    {
        foreach(var diceNum in _diceNums)
        {
            diceNum.SetText(number.ToString());
        }
    }

    // Debe mostrar el front con rotación 0, 0
    private IEnumerator ShowDiceNum(int diceNum)
    {
        _instanciedDice.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        yield return new WaitForSeconds(1.5f);
        Destroy(_instanciedDice);

        GameController.instance.GetCharacterOfTurn().Move(diceNum);
    }
}
