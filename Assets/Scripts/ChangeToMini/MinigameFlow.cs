using System.Collections.Generic;
using UnityEngine;

public class MinigameFlow : MonoBehaviour
{
    public static MinigameFlow instance;

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
        if (isStartingMinigame) return;

        if (database == null)
        {
            Debug.LogError("MinigameFlow: no hay MinigameDatabase asignada.");
            return;
        }

        List<MinigameData> possibleMinigames = database.GetRoundEndMinigames();

        if (possibleMinigames == null || possibleMinigames.Count == 0)
        {
            Debug.LogError("MinigameFlow: no hay minijuegos válidos para final de ronda.");
            return;
        }

        int randomIndex = Random.Range(0, possibleMinigames.Count);
        MinigameData selectedMinigame = possibleMinigames[randomIndex];

        StartMinigame(selectedMinigame);
    }

    public void StartMinigame(MinigameData mg)
    {
        if (isStartingMinigame) return;

        if (mg == null)
        {
            Debug.LogError("MinigameFlow: el MinigameData recibido es null.");
            return;
        }

        if (MinigameController.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe MinigameController en escena.");
            return;
        }

        isStartingMinigame = true;

        SavePartySnapshotFromBoard();

        MinigameSession.SelectedMinigame = mg;

        MinigameController.instance.OpenLoadingScene();

        isStartingMinigame = false;
    }

    private void SavePartySnapshotFromBoard()
    {
        if (PartySession.instance == null)
        {
            Debug.LogWarning("MinigameFlow: no existe PartySession. Se continuará sin snapshots.");
            return;
        }

        if (GameController.instance == null)
        {
            Debug.LogError("MinigameFlow: no existe GameController.");
            return;
        }

        for (int i = 0; i < GameController.instance.GetCharactersInParty(); i++)
        {
            Character c = GameController.instance.GetCharacter(i);

            if (c == null)
                continue;

            PartySession.instance.characters[i] = new CharacterSnapshot
            {
                characterId = c.characterId,
                coins = c.coins,
                stars = c.stars,
                isPlayer = c.isPlayer,
                characterImage = c.characterImage
            };
        }
    }

    public void StartMinigameByType(MinigameType type)
    {
        if (isStartingMinigame)
            return;

        if (database == null)
        {
            Debug.LogError("MinigameFlow: no hay MinigameDatabase asignada.");
            return;
        }

        List<MinigameData> possibleMinigames = database.GetMinigamesByType(type);

        if (possibleMinigames == null || possibleMinigames.Count == 0)
        {
            Debug.LogError("MinigameFlow: no hay minijuegos del tipo: " + type);
            return;
        }

        int randomIndex = Random.Range(0, possibleMinigames.Count);
        MinigameData selectedMinigame = possibleMinigames[randomIndex];

        StartMinigame(selectedMinigame);
    }
}