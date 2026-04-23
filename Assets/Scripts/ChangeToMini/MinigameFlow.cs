using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinigameFlow : MonoBehaviour
{
    public static MinigameFlow instance;

    [Header("Config")]
    [SerializeField] private MinigameDatabase database;
    [SerializeField] private string loadingSceneName = "LoadingScene";

    private bool isLoadingMinigame = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartRoundEndMinigame()
    {
        if (isLoadingMinigame) return;

        var list = database.GetRoundEndMinigames();

        if (list == null || list.Count == 0)
        {
            Debug.LogError("No hay minijuegos válidos para final de ronda.");
            return;
        }

        MinigameData chosen = list[Random.Range(0, list.Count)];
        StartMinigame(chosen);
    }

    public void StartMinigame(MinigameData mg)
    {
        SavePartySnapshotFromBoard();
        SaveBoardState();

        MinigameSession.SelectedMinigame = mg;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadLoadingSceneRoutine()
    {
        isLoadingMinigame = true;

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
        if (!loadingScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        }

        isLoadingMinigame = false;
    }

    private void SavePartySnapshotFromBoard()
    {
        if (PartySession.instance == null)
        {
            Debug.LogError("No existe PartySession en escena.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            Character c = GameController.instance.GetCharacter(i);

            PartySession.instance.characters[i] = new CharacterSnapshot
            {
                characterId = c.characterId,
                coins = c.coins,
                stars = c.stars,
                isPlayer = c.isPlayer,
                characterImage = c.characterImage,
                actualBoxIndex = Board.instance.GetBoxIndex(c.actualBox)
            };
        }
    }

    private void SaveBoardState()
    {
        if (PartySession.instance == null || GameController.instance == null)
        {
            Debug.LogError("No se puede guardar BoardState.");
            return;
        }

        PartySession.instance.boardState.actualTurn = GameController.instance.GetActualTurn();
        PartySession.instance.boardState.actualRound = GameController.instance.GetActualRound();
        PartySession.instance.boardState.idOrder = GameController.instance.GetIdOrderCopy();
        PartySession.instance.boardState.isPlayerOrder = GameController.instance.GetIsPlayerOrderCopy();
    }
}