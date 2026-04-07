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

    private void Awake()
    {
        boardScene = SceneManager.GetActiveScene();

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

    /// <summary>
    /// Selecciona los posibles minijuegos según el tipo definido
    /// y los muestra en la UI correspondiente.
    /// </summary>
    public void SelectMinigame(string strMinigameType)
    {
        MinigameType type = (MinigameType)Enum.Parse(typeof(MinigameType), strMinigameType);

        List<MinigameData> possible = database.GetMinigamesByType(type);

        if (possible.Count == 0)
        {
            Debug.LogError("No hay minijuegos del tipo: " + type);
            return;
        }

        // Pasamos solo los nombres de escena a la UI
        List<string> sceneNames = possible.Select(m => m.sceneName).ToList();

        UIManager.instance.ShowPossibleMinigamesList(sceneNames);
    }

    public void SetMinigameToLoad(string sceneName)
    {
        // Buscar el minijuego real
        selectedMinigame = database.minigames
            .FirstOrDefault(m => m.sceneName == sceneName);

        if (selectedMinigame == null)
        {
            Debug.LogError("No se encontró un MinigameData para la escena: " + sceneName);
            return;
        }

        MinigameSession.SelectedMinigame = selectedMinigame;
    }

    public void LoadMinigame(string sceneName)
    {
        // Hacemos que la carga sea Aditiva para que la escena del tablero siga cargada
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        // Cuando carga la escena de minijuego desactivamos todos los objetos del tablero
        LoadBoardGameObjects(false);
    }

    public void LoadBoardGameObjects(bool activate)
    {
        foreach (GameObject go in boardScene.GetRootGameObjects())
        {
            Debug.Log(go.name);
            go.SetActive(activate);
        }
    }

    public void ReloadBoard()
    {
        SceneManager.UnloadSceneAsync(selectedMinigame.sceneName);
        LoadBoardGameObjects(true);
    }
}
