using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;
    private const int MAX_ROUNDS = 10;
    private int playersToCreate;

    [SerializeField] private CharacterSetting[] _charactersSettings;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private GameObject dicePrefab;
    public Dice DiceToUse;
    [SerializeField] private Dice normalDice;

    [SerializeField] private Transform[] spawns;
    private bool gameEnded = false;

    private List<Player> players = new List<Player>();
    private List<NPC_Controller> npcs = new List<NPC_Controller>();
    private Character[] characters;

    private Character characterOfTurn;
    private Player playerOfTurn;
    private NPC_Controller npcOfTurn;
    [SerializeField] private BoardCameraController camFollow;

    // Orden y control
    private int actualTurn = 0;
    private int actualRound = 0; // 1 round son 4 turnos
    private int[] idOrder = new int[4];
    private bool[] isPlayer = new bool[4];
    private bool isPlayerRolling = false;
    public UnityEvent OnRoundEnded;

    public static GameController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
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
        characters = new Character[MAX_PLAYERS];
        CreatePlayers();

        StartCoroutine(Box.InitStarSystemNextFrame());
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

        // De momento solo esta preparado para 1 solo jugador humano
        int playerSetting = PlayerPrefs.GetInt("PlayerSelected", 0);

        // Creará tantos jugadores (que se puedan controlar) como se hayan especificado en el menú
        for(int i = 0; i < playersToCreate; i++)
        {
            // Instancia jugador
            //GameObject newPlayerGO = Instantiate(playerPrefab, initialPos + (Vector3.left * (i * 3)), Quaternion.identity);
            GameObject newPlayerGO = Instantiate(_charactersSettings[playerSetting].characterPrefab, initialPos + (Vector3.left * (i * 3)), Quaternion.identity);
            Destroy(newPlayerGO.GetComponent<NPC_Controller>());
            newPlayerGO.name = _charactersSettings[playerSetting].CharacterName;

            Player newPlayer = newPlayerGO.GetComponent<Player>();
            newPlayer.enabled = true;
            newPlayer.isPlayer = true;
            newPlayer.SetCharSetting(_charactersSettings[playerSetting]);
            newPlayerGO.transform.position = spawns[setId].position;

            // Asigna un character id al jugador instanciado
            newPlayer.SetCharId(setId);
            idOrder[i] = setId;
            isPlayer[i] = true;
            characters[i] = newPlayer;

            players.Add(newPlayer);

            setId++;
        }

        // Creará los NPCs necesarios hasta que sean un total de 4 jugadores (sumando players y NPCs)
        int npcsToCreate = (MAX_PLAYERS - playersToCreate);
        if(npcsToCreate > 0)
        {
            int[] npcsCharSettings = new int[npcsToCreate];
            HashSet<int> usedCharSettings = new HashSet<int>();

            usedCharSettings.Add(playerSetting);

            for(int i = 0; i < npcsToCreate; i++)
            {
                int rand = 0;

                do
                {
                    rand = Random.Range(0, _charactersSettings.Length);
                }while(!usedCharSettings.Add(rand));

                npcsCharSettings[i] = rand;
            }

            for (int i = 0; i < npcsToCreate; i++)
            {
                // Instanciamos un NPC
                //GameObject newNPCGO = Instantiate(npcPrefab, initialPos + (Vector3.right * (i * 3)), Quaternion.identity);
                GameObject newNPCGO = Instantiate(_charactersSettings[npcsCharSettings[i]].characterPrefab, initialPos + (Vector3.right * (i * 3)), Quaternion.identity);
                Destroy(newNPCGO.GetComponent<Player>());
                newNPCGO.name = _charactersSettings[npcsCharSettings[i]].CharacterName;

                NPC_Controller newNPC = newNPCGO.GetComponent<NPC_Controller>();
                newNPC.enabled = true;
                newNPC.isPlayer = false;
                newNPC.SetCharSetting(_charactersSettings[npcsCharSettings[i]]);
                newNPCGO.transform.position = spawns[setId].position;

                // Asigna un character id al NPC instanciado
                newNPC.SetCharId(setId);
                idOrder[(playersToCreate) + i] = setId;
                isPlayer[(playersToCreate) + i] = false;
                characters[setId] = newNPC;

                npcs.Add(newNPC);

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

        DiceToUse = normalDice;

        // Dependiendo de si es un jugador o npc el movimiento y el llamado a funciones será distinto
        if (isPlayer[thisCharTurn])
        {
            // Recorre la lista de players
            for (int i = 0; i < players.Count; i++)
            {
                // Cuando el jugador actual de la lista tenga el mismo Char id que el del turno actual...
                if (players[i].GetCharId() == idOrder[thisCharTurn])
                {
                    playerOfTurn = players[i];
                    characterOfTurn = players[i];

                    // Actualiza UI
                    UIManager.instance.SetActualCharacter(playerOfTurn);
                    UIManager.instance.ChangeCharacterUI(playerOfTurn);

                    UIManager.instance.ControlActionPanel(true);

                    // cam sigue jugador
                    camFollow.SetTarget(playerOfTurn.transform);
                    camFollow.SetBoxRotation(playerOfTurn.savedCameraRotationY);
                }

            }
        }
        else
        {
            // Recorre la lista de npcs para buscar al npc del turno actual
            for(int i = 0; i < npcs.Count; i++)
            {
                // Cuando el npc actual de la lista tenga el mismo char id que el del turno actual...
                if (npcs[i].GetCharId() == idOrder[thisCharTurn])
                {
                    npcOfTurn = npcs[i];
                    characterOfTurn = npcs[i];
                    UIManager.instance.SetActualCharacter(npcOfTurn);
                    UIManager.instance.ControlActionPanel(false);
                    UIManager.instance.ChangeCharacterUI(npcOfTurn);

                    // cam sigue npc
                    camFollow.SetTarget(npcOfTurn.transform);
                    camFollow.SetBoxRotation(npcOfTurn.savedCameraRotationY);

                    DiceScript.Instance.SetupDice(DiceToUse, true);

                    /*
                    // Instanciamos el dado encima del npc
                    GameObject dice = Instantiate(dicePrefab, npcOfTurn.transform.position + (Vector3.up * (3 * npcOfTurn.transform.localScale.x)), Quaternion.identity);

                    // Hacemos que el dado actual sea un dado normal (en un futuro cambiará)
                    DiceScript diceScr = dice.GetComponentInChildren<DiceScript>();
                    DiceToUse = normalDice;
                    diceScr.ChangeScriptableDice(DiceToUse); // De momento solo con dados normales
                    

                    // Hacemos que empiece la corutina del rolling del dado del NPC
                    StartCoroutine(diceScr.DiceRolling(npcOfTurn));

                    // Iniciamos la corutina para que el NPC pare el dado en algún momento
                    StartCoroutine(npcOfTurn.doRolling(diceScr));
                    */


                }
            }
        }
    }

    
    public void ThrowPlayerDice()
    {
        UIManager.instance.ControlActionPanel(false);
        isPlayerRolling = true;

        /*
        // Instanciamos el dado encima del jugador
        GameObject dice = Instantiate(dicePrefab, playerOfTurn.transform.position + (Vector3.up * (3 * playerOfTurn.transform.localScale.x)), Quaternion.identity);

        DiceScript diceScr = dice.GetComponentInChildren<DiceScript>();
        diceScr.ChangeScriptableDice(DiceToUse);

        // Hacemos que empiece la corutina del rolling del dado del jugador
        StartCoroutine(diceScr.DiceRolling(playerOfTurn));
        */

        DiceScript diceScr = FindFirstObjectByType<DiceScript>();
        diceScr.SetupDice(DiceToUse, false);
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

            // Activamos el evento de OnRoundEnded
            OnRoundEnded?.Invoke();

            yield return new WaitForSeconds(2f);
            if(actualRound < MAX_ROUNDS)
            {
                StartMovement();
            }
            
        }
        else
        {
            if(actualRound < MAX_ROUNDS)
            {
                // Que tire el siguiente jugador
                yield return new WaitForSeconds(2f);
                StartMovement();
            }
            
        }

        if (actualRound >= MAX_ROUNDS)
        {
            if (gameEnded) yield break; 
            gameEnded = true;

            UIManager.instance.ShowLeaderboard(characters);
            yield return null;
        }


    }

    public bool IsPlayerRolling()
    {
        return isPlayerRolling;
    }

    public void ChangeDiceToUse(Dice newDice)
    {
        DiceToUse = newDice;
    }

    public Character GetCharacter(int index)
    {
        return characters[index];
    }

    public Player GetPlayerOfTurn()
    {
        return playerOfTurn;
    }

    public Character GetCharacterOfTurn()
    {
        return characterOfTurn;
    }

    public int GetCharactersInParty()
    {
        return characters.Length;
    }

    // Llamado cuando un personaje pisa una Box
    public void UpdateCameraRotation(float newRotationY)
    {
        camFollow.SetBoxRotation(newRotationY);
    }

}
