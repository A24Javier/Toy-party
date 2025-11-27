using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;
    private int playersToCreate;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private GameObject dicePrefab;
    private Dice diceToUse;
    [SerializeField] private Dice normalDice;

    [SerializeField] private Transform[] spawns;

    private Player[] players;
    private NPC_Controller[] npcs;
    private Player playerOfTurn;
    private NPC_Controller npcOfTurn;
    [SerializeField]private CameraFollow camFollow;


    // Orden y control
    private int actualTurn = 0;
    private int actualRound = 0; // 1 round son 4 turnos
    private int[] idOrder = new int[4];
    private bool[] isPlayer = new bool[4];
    private bool isPlayerRolling = false;

    public static GameController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Start()
    {
        // Coge el valor de jugadores a crear almacenado en PlayerPrefs y lo almacena en 'playersToCreate'
        playersToCreate = PlayerPrefs.GetInt("PlayersToCreate");
        playersToCreate = 1; // BORRAR EN UN FUTURO
        CreatePlayers();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Función que crea a los jugadores y a los NPCs
    /// </summary>
    private void CreatePlayers()
    {
        // Instanciar jugadores en la primera casilla de Board
        Box firstBox = Board.instance.GetCasilla(0).GetComponent<Box>();
        Vector3 initialPos = firstBox.GetThisBoxTransf().position;

        // Id que se usará para asignarles a los jugadores y NPCs
        int setId = 0;

        // Creará tantos jugadores (que se puedan controlar) como se hayan especificado en el menú
        for(int i = 0; i < playersToCreate; i++)
        {
            // Instancia jugador
            GameObject newPlayerGO = Instantiate(playerPrefab, initialPos + (Vector3.left * (i * 3)), Quaternion.identity);
            Player newPlayer = newPlayerGO.GetComponent<Player>();
            newPlayer.isPlayer = true;
            newPlayerGO.transform.position = spawns[setId].position;

            // Asigna un character id al jugador instanciado
            newPlayer.SetCharId(setId);
            idOrder[i] = setId;
            isPlayer[i] = true;

            setId++;
        }

        // Creará los NPCs necesarios hasta que sean un total de 4 jugadores (sumando players y NPCs)
        if((MAX_PLAYERS - playersToCreate) > 0)
        {
            for (int i = 0; i < (MAX_PLAYERS - playersToCreate); i++)
            {
                // Instanciamos un NPC
                GameObject newNPCGO = Instantiate(npcPrefab, initialPos + (Vector3.right * (i * 3)), Quaternion.identity);
                NPC_Controller newNPC = newNPCGO.GetComponent<NPC_Controller>();
                newNPC.isPlayer = false;
                newNPCGO.transform.position = spawns[setId].position;

                // Asigna un character id al NPC instanciado
                newNPC.SetCharId(setId);
                idOrder[(playersToCreate) + i] = setId;
                isPlayer[(playersToCreate) + i] = false;

                setId++;
            }
        }

        // Mezclamos los numeros para que el orden sea aleatorio
        for (int i = 0; i < idOrder.Length; i++)
        {
            int rand = Random.Range(0, idOrder.Length);
            int temp = idOrder[i];
            bool isPlayerTemp = isPlayer[i];

            idOrder[i] = idOrder[rand];
            idOrder[rand] = temp;

            isPlayer[i] = isPlayer[rand];
            isPlayer[rand] = isPlayerTemp;
        }

        /*for(int i = 0; i < idOrder.Length; i++)
        {
            Debug.Log("Orden[" + i + "]: " + idOrder[i] + ", isPlayer: " + isPlayer[i]);
        }*/

        // Llenamos los arrays de players y npcs
        players = GameObject.FindObjectsOfType<Player>();
        npcs = GameObject.FindObjectsOfType<NPC_Controller>();
        StartMovement();
    }

    /// <summary>
    /// Función que se encarga de que el jugador/npc actual pueda tirar dado y moverse
    /// </summary>
    private void StartMovement()
    {
        //Resetea el espacio
        InputHandler.instance.ResetSpace();

        isPlayerRolling = false;
        // Hacemos una operación para saber a que jugador/npc le toca ahora dependiendo el turno actual
        int thisCharTurn = idOrder[actualTurn % 4];
        
        Debug.Log("El charTurn es: " +  thisCharTurn);
        Debug.Log("El turno actual es de un jugador: " + isPlayer[thisCharTurn]);

        // Dependiendo de si es un jugador o npc el movimiento y el llamado a funciones será distinto
        if (isPlayer[thisCharTurn])
        {
            // Recorre la lista de players
            for (int i = 0; i < players.Length; i++)
            {
                // Cuando el jugador actual de la lista tenga el mismo Char id que el del turno actual...
                if (players[i].GetCharId() == idOrder[thisCharTurn])
                {
                    playerOfTurn = players[i];

                    // Actualiza UI
                    UIManager.instance.ChangeCharacterUI(playerOfTurn);
                    UIManager.instance.SetActualCharacter(playerOfTurn);

                    diceToUse = normalDice;
                    UIManager.instance.ControlActionPanel(true);

                    // cam sigue jugador
                    camFollow.SetTarget(playerOfTurn.transform);
                    camFollow.cRotation = playerOfTurn.savedCameraRotationY;
                    
                }

            }
        }
        else
        {
            // Recorre la lista de npcs para buscar al npc del turno actual
            for(int i = 0; i < npcs.Length; i++)
            {
                // Cuando el npc actual de la lista tenga el mismo char id que el del turno actual...
                if (npcs[i].GetCharId() == idOrder[thisCharTurn])
                {
                    UIManager.instance.ControlActionPanel(false);

                    npcOfTurn = npcs[i];
                    UIManager.instance.ChangeCharacterUI(npcOfTurn);
                    UIManager.instance.SetActualCharacter(npcOfTurn);

                    // cam sigue npc
                    camFollow.SetTarget(npcOfTurn.transform);
                    camFollow.cRotation = npcOfTurn.savedCameraRotationY;
                    // Instanciamos el dado encima del npc
                    GameObject dice = Instantiate(dicePrefab, npcOfTurn.transform.position + (Vector3.up * (3 * npcOfTurn.transform.localScale.x)), Quaternion.identity);

                    // Hacemos que el dado actual sea un dado normal (en un futuro cambiará)
                    DiceScript diceScr = dice.GetComponentInChildren<DiceScript>();
                    diceToUse = normalDice;
                    diceScr.ChangeScriptableDice(diceToUse); // De momento solo con dados normales

                    // Hacemos que empiece la corutina del rolling del dado del NPC
                    StartCoroutine(diceScr.DiceRolling(npcOfTurn));

                    // Iniciamos la corutina para que el NPC pare el dado en algún momento
                    StartCoroutine(npcOfTurn.doRolling(diceScr));
                }
            }
        }
    }

    public void ThrowPlayerDice()
    {
        UIManager.instance.ControlActionPanel(false);
        isPlayerRolling = true;

        // Instanciamos el dado encima del jugador
        GameObject dice = Instantiate(dicePrefab, playerOfTurn.transform.position + (Vector3.up * (3 * playerOfTurn.transform.localScale.x)), Quaternion.identity);

        DiceScript diceScr = dice.GetComponentInChildren<DiceScript>();
        diceScr.ChangeScriptableDice(diceToUse);

        // Hacemos que empiece la corutina del rolling del dado del jugador
        StartCoroutine(diceScr.DiceRolling(playerOfTurn));
    }

    /// <summary>
    /// Función que se inicia cuando un turno debe terminar
    /// </summary>
    public IEnumerator FinishTurn()
    {
        Debug.Log("Entra en Finish Turn. Resultado: " + actualTurn % 4);

        // Sumamos un turno a 'actualTurn'
        actualTurn++;

        // Si todos los jugadores se han movido al menos una vez, empieza un minijuego.
        // Sino se inicia el movimiento del siguiente jugador
        if (actualTurn % 4 == 0)
        {
            Debug.Log("Ronda de minijuego");
            // Ronda de minijuego
            //MinigameController.instance.SelectMinigame("AllVSAll");

            actualRound++;
            yield return new WaitForSeconds(2f);
            StartMovement();
        }
        else
        {
            // Que tire el siguiente jugador
            yield return new WaitForSeconds(2f);
            StartMovement();
        }
    }

    public bool IsPlayerRolling()
    {
        return isPlayerRolling;
    }

    public void ChangeDiceToUse(Dice newDice)
    {
        diceToUse = newDice;
    }

    public Player GetPlayerOfTurn()
    {
        return playerOfTurn;
    }
}
