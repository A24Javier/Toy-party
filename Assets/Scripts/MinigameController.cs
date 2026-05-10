using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    public static MinigameController instance;

    [Header("Scene Names")]
    [SerializeField] private string boardSceneName = "Prototip";
    [SerializeField] private string loadingSceneName = "LoadingScreen";
    [SerializeField] private string rewardsSceneName = "NivelRecompensasMinijuegos";

    private string currentMinigameSceneName;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenLoadingScene()
    {
        if (isTransitioning) return;
        StartCoroutine(OpenLoadingSceneRoutine());
    }

    private IEnumerator OpenLoadingSceneRoutine()
    {
        isTransitioning = true;

        SetBoardActive(false);

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);

        if (!loadingScene.IsValid() || !loadingScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        }

        isTransitioning = false;
    }

    public void StartSelectedMinigame()
    {
        if (isTransitioning) return;

        if (MinigameSession.SelectedMinigame == null)
        {
            Debug.LogError("MinigameController: no hay MinigameSession.SelectedMinigame.");
            return;
        }

        StartCoroutine(StartSelectedMinigameRoutine());
    }

    private IEnumerator StartSelectedMinigameRoutine()
    {
        isTransitioning = true;

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);

        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        }

        currentMinigameSceneName = MinigameSession.SelectedMinigame.sceneName;

        if (string.IsNullOrEmpty(currentMinigameSceneName))
        {
            Debug.LogError("MinigameController: el sceneName del minijuego está vacío.");
            isTransitioning = false;
            yield break;
        }

        Scene minigameScene = SceneManager.GetSceneByName(currentMinigameSceneName);

        if (!minigameScene.IsValid() || !minigameScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(currentMinigameSceneName, LoadSceneMode.Additive);
        }

        isTransitioning = false;
    }

    public void OpenRewardScene()
    {
        if (isTransitioning) return;
        StartCoroutine(OpenRewardSceneRoutine());
    }

    private IEnumerator OpenRewardSceneRoutine()
    {
        isTransitioning = true;

        if (!string.IsNullOrEmpty(currentMinigameSceneName))
        {
            Scene minigameScene = SceneManager.GetSceneByName(currentMinigameSceneName);

            if (minigameScene.IsValid() && minigameScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(currentMinigameSceneName);
            }
        }

        currentMinigameSceneName = null;

        Scene rewardsScene = SceneManager.GetSceneByName(rewardsSceneName);

        if (!rewardsScene.IsValid() || !rewardsScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(rewardsSceneName, LoadSceneMode.Additive);
        }

        isTransitioning = false;
    }

    public void EndRewardsAndReturnToBoard()
    {
        if (isTransitioning) return;
        StartCoroutine(EndRewardsAndReturnToBoardRoutine());
    }

    private IEnumerator EndRewardsAndReturnToBoardRoutine()
    {
        isTransitioning = true;

        Scene rewardsScene = SceneManager.GetSceneByName(rewardsSceneName);

        if (rewardsScene.IsValid() && rewardsScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(rewardsSceneName);
        }

        SetBoardActive(true);

        isTransitioning = false;
    }

    private void SetBoardActive(bool active)
    {
        Scene boardScene = SceneManager.GetSceneByName(boardSceneName);

        if (!boardScene.IsValid() || !boardScene.isLoaded)
        {
            Debug.LogError($"MinigameController: la escena {boardSceneName} no está cargada.");
            return;
        }

        GameObject[] roots = boardScene.GetRootGameObjects();

        foreach (GameObject go in roots)
        {
            if (go == gameObject)
                continue;

            go.SetActive(active);
        }
    }
}