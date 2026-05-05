using UnityEngine;

public class MinigameFlow : MonoBehaviour
{
    public static MinigameFlow instance;

    [Header("Config")]
    [SerializeField] private MinigameDatabase database;

    private bool isStartingMinigame = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRoundEndMinigame()
    {
        if (isStartingMinigame)
            return;

        isStartingMinigame = true;

        if (database == null)
        {
            Debug.LogError("MinigameFlow: no hay MinigameDatabase asignada.");
            isStartingMinigame = false;
            return;
        }

        var list = database.GetRoundEndMinigames();

        if (list == null || list.Count == 0)
        {
            Debug.LogError("MinigameFlow: no hay minijuegos válidos para final de ronda. Recuerda excluir OneVSOne.");
            isStartingMinigame = false;
            return;
        }

        MinigameData chosen = list[Random.Range(0, list.Count)];

        StartMinigame(chosen);
    }

    public void StartMinigame(MinigameData mg)
    {
        if (mg == null)
        {
            Debug.LogError("MinigameFlow: MinigameData recibido es null.");
            isStartingMinigame = false;
            return;
        }

        SavePartySnapshotFromBoard();
        SaveBoardState();

        MinigameSession.SelectedMinigame = mg;

        if (MinigameController.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe MinigameController.instance.");
            isStartingMinigame = false;
            return;
        }

        MinigameController.instance.OpenLoadingScene();

        isStartingMinigame = false;
    }

    private void SavePartySnapshotFromBoard()
    {
        if (PartySession.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe PartySession en escena.");
            return;
        }

        if (GameController.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe GameController.instance.");
            return;
        }

        if (Board.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe Board.instance.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            Character c = GameController.instance.GetCharacter(i);

            if (c == null)
            {
                Debug.LogError("MinigameFlow: character " + i + " es null.");
                continue;
            }

            PartySession.instance.characters[i] = new CharacterSnapshot
            {
                characterId = c.characterId,
                coins = c.coins,
                stars = c.stars,
                isPlayer = c.isPlayer,
                characterImage = c.characterImage,
                actualBoxIndex = Board.instance.GetBoxIndex(c.actualBox),
                characterSettingIndex = c.characterSettingIndex
            };
        }
    }

    private void SaveBoardState()
    {
        if (PartySession.instance == null || GameController.instance == null)
        {
            Debug.LogError("MinigameFlow: no se puede guardar BoardState.");
            return;
        }

        PartySession.instance.boardState.actualTurn = GameController.instance.GetActualTurn();
        PartySession.instance.boardState.actualRound = GameController.instance.GetActualRound();
        PartySession.instance.boardState.idOrder = GameController.instance.GetIdOrderCopy();
        PartySession.instance.boardState.isPlayerOrder = GameController.instance.GetIsPlayerOrderCopy();
    }
}