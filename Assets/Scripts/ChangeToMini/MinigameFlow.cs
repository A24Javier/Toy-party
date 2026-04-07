using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameFlow : MonoBehaviour
{
    public static MinigameFlow instance;

    [Header("Config")]
    [SerializeField] private MinigameDatabase database;
    [SerializeField] private string loadingSceneName = "LoadingScene";

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartRandom(MinigameType type)
    {
        var list = database.GetMinigamesByType(type);

        if (list == null || list.Count == 0)
        {
            Debug.LogError($"No hay minijuegos del tipo {type}");
            return;
        }

        MinigameData chosen = list[Random.Range(0, list.Count)];
        StartMinigame(chosen);
    }

    public void StartMinigame(MinigameData mg)
    {
        // 1 Guardar estado de los 4 jugadores
        SavePartySnapshotFromBoard();

        // 2️ Guardar qué minijuego se va a jugar
        MinigameSession.SelectedMinigame = mg;

        // 3️ Cargar SIEMPRE la escena Loading
        //SceneManager.LoadScene(loadingSceneName);
        MinigameController.instance.LoadMinigame(mg.sceneName);
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
