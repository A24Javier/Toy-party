using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMinigameFlowLauncher : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string loadingSceneName = "LoadingScreen";

    [Header("Minigame")]
    [SerializeField] private MinigameDatabase database;
    [SerializeField] private string minigameSceneName = "FrutaAlHoyo";

    private bool isLoading = false;

    private void Update()
    {
        if (isLoading)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TestFlowRoutine());
        }
    }

    private IEnumerator TestFlowRoutine()
    {
        isLoading = true;

        MinigameData selected = FindMinigameData();

        if (selected == null)
        {
            Debug.LogError("TestMinigameFlowLauncher: no se ha encontrado el MinigameData.");
            isLoading = false;
            yield break;
        }

        MinigameSession.SelectedMinigame = selected;

        Debug.Log("TEST: Cargando LoadingScreen...");

        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);

        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadingScene);
        }

        Debug.Log("TEST: LoadingScreen cargada. Pulsa cualquier tecla en LoadingScreen para ir al minijuego.");
    }

    private MinigameData FindMinigameData()
    {
        if (database == null)
        {
            Debug.LogError("TestMinigameFlowLauncher: falta asignar MinigameDatabase.");
            return null;
        }

        if (database.minigames == null)
        {
            Debug.LogError("TestMinigameFlowLauncher: database.minigames es null.");
            return null;
        }

        for (int i = 0; i < database.minigames.Length; i++)
        {
            MinigameData data = database.minigames[i];

            if (data == null)
                continue;

            if (data.sceneName == minigameSceneName)
                return data;
        }

        Debug.LogError("TestMinigameFlowLauncher: no existe minijuego con sceneName: " + minigameSceneName);
        return null;
    }
}