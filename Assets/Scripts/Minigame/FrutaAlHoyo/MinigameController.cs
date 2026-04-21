using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    public static MinigameController instance;

    [Header("Database")]
    [SerializeField] private MinigameDatabase database;

    private MinigameData selectedMinigame;
    private Scene boardScene;
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

    private void Start()
    {
        boardScene = SceneManager.GetSceneByName("Prototip");
    }

    public void LoadMinigame(string sceneName)
    {
        if (isTransitioning) return;

        selectedMinigame = database.minigames
            .FirstOrDefault(m => m.sceneName == sceneName);

        if (selectedMinigame == null)
        {
            Debug.LogError("No se encontró un MinigameData para la escena: " + sceneName);
            return;
        }

        StartCoroutine(LoadMinigameRoutine(sceneName));
    }

    private IEnumerator LoadMinigameRoutine(string sceneName)
    {
        isTransitioning = true;

        boardScene = SceneManager.GetSceneByName("Prototip");
        if (!boardScene.IsValid() || !boardScene.isLoaded)
        {
            Debug.LogError("MinigameController: la escena Prototip no está cargada.");
            isTransitioning = false;
            yield break;
        }

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        LoadBoardGameObjects(false);

        Scene loadingScene = SceneManager.GetSceneByName("LoadingScene");
        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync("LoadingScene");
        }

        isTransitioning = false;
    }

    public void LoadBoardGameObjects(bool activate)
    {
        boardScene = SceneManager.GetSceneByName("Prototip");

        if (!boardScene.IsValid() || !boardScene.isLoaded)
        {
            Debug.LogError("MinigameController: la escena Prototip no está cargada.");
            return;
        }

        foreach (GameObject go in boardScene.GetRootGameObjects())
        {
            if (go == gameObject)
                continue;

            go.SetActive(activate);
        }
    }

    public void ReloadBoard()
    {
        if (isTransitioning) return;
        StartCoroutine(ReloadBoardRoutine());
    }

    private IEnumerator ReloadBoardRoutine()
    {
        isTransitioning = true;

        if (selectedMinigame != null)
        {
            Scene mgScene = SceneManager.GetSceneByName(selectedMinigame.sceneName);
            if (mgScene.IsValid() && mgScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(selectedMinigame.sceneName);
            }
        }

        LoadBoardGameObjects(true);

        isTransitioning = false;
    }
}