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
        if (isLoadingMinigame) return;

        SavePartySnapshotFromBoard();
        MinigameSession.SelectedMinigame = mg;
        StartCoroutine(LoadLoadingSceneRoutine());
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
                characterImage = c.characterImage
            };
        }
    }
}