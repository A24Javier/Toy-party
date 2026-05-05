using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    public static MinigameController instance;

    [Header("Scene Names")]
    [SerializeField] private string boardSceneName = "Prototip";
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("Board Objects To Disable During Minigames")]
    [Tooltip("Cámaras del tablero. Ejemplo: Main Camera, cámaras de Cinemachine, cámara del dado, etc.")]
    [SerializeField] private GameObject[] boardCameras;

    [Tooltip("Canvas o roots de UI del tablero. Ejemplo: HUD, Canvas principal, paneles de acción, pausa, etc.")]
    [SerializeField] private GameObject[] boardCanvasRoots;

    [Tooltip("Scripts de input del tablero que no deben funcionar durante LoadingScene/minijuego.")]
    [SerializeField] private Behaviour[] boardInputBehaviours;

    [Tooltip("EventSystem del tablero. Se apaga para evitar warning de 2 EventSystems.")]
    [SerializeField] private EventSystem boardEventSystem;

    [Tooltip("Opcional. Objetos visuales pesados del tablero que quieras ocultar sin romper lógica.")]
    [SerializeField] private GameObject[] optionalBoardVisualRoots;

    private bool isTransitioning = false;
    private string currentMinigameSceneName = "";

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
        if (isTransitioning)
            return;

        StartCoroutine(OpenLoadingSceneRoutine());
    }

    private IEnumerator OpenLoadingSceneRoutine()
    {
        isTransitioning = true;

        if (MinigameSession.SelectedMinigame == null)
        {
            Debug.LogError("MinigameController: no hay MinigameSession.SelectedMinigame.");
            isTransitioning = false;
            yield break;
        }

        if (!IsBoardLoaded())
        {
            Debug.LogError("MinigameController: Prototip no está cargada. El flujo aditivo necesita Prototip persistente.");
            isTransitioning = false;
            yield break;
        }

        SetBoardActive(false);

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);

        if (!loadingScene.IsValid() || !loadingScene.isLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
            yield return loadOperation;
        }

        Scene loadedLoadingScene = SceneManager.GetSceneByName(loadingSceneName);
        if (loadedLoadingScene.IsValid() && loadedLoadingScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedLoadingScene);
        }

        isTransitioning = false;
    }

    public void StartSelectedMinigame()
    {
        if (isTransitioning)
            return;

        StartCoroutine(StartSelectedMinigameRoutine());
    }

    private IEnumerator StartSelectedMinigameRoutine()
    {
        isTransitioning = true;

        MinigameData selected = MinigameSession.SelectedMinigame;

        if (selected == null)
        {
            Debug.LogError("MinigameController: no hay minijuego seleccionado.");
            isTransitioning = false;
            yield break;
        }

        if (string.IsNullOrWhiteSpace(selected.sceneName))
        {
            Debug.LogError("MinigameController: el sceneName del minijuego está vacío.");
            isTransitioning = false;
            yield break;
        }

        currentMinigameSceneName = selected.sceneName;

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);

        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            AsyncOperation unloadLoading = SceneManager.UnloadSceneAsync(loadingScene);
            yield return unloadLoading;
        }

        Scene minigameScene = SceneManager.GetSceneByName(currentMinigameSceneName);

        if (!minigameScene.IsValid() || !minigameScene.isLoaded)
        {
            AsyncOperation loadMinigame = SceneManager.LoadSceneAsync(currentMinigameSceneName, LoadSceneMode.Additive);
            yield return loadMinigame;
        }

        Scene loadedMinigameScene = SceneManager.GetSceneByName(currentMinigameSceneName);
        if (loadedMinigameScene.IsValid() && loadedMinigameScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedMinigameScene);
        }

        isTransitioning = false;
    }

    public void EndMinigameAndReturnToBoard()
    {
        if (isTransitioning)
            return;

        StartCoroutine(EndMinigameAndReturnToBoardRoutine());
    }

    private IEnumerator EndMinigameAndReturnToBoardRoutine()
    {
        isTransitioning = true;

        ApplyPartySessionResultsToBoard();

        if (!string.IsNullOrWhiteSpace(currentMinigameSceneName))
        {
            Scene minigameScene = SceneManager.GetSceneByName(currentMinigameSceneName);

            if (minigameScene.IsValid() && minigameScene.isLoaded)
            {
                AsyncOperation unloadMinigame = SceneManager.UnloadSceneAsync(minigameScene);
                yield return unloadMinigame;
            }
        }

        Scene boardScene = SceneManager.GetSceneByName(boardSceneName);
        if (boardScene.IsValid() && boardScene.isLoaded)
        {
            SceneManager.SetActiveScene(boardScene);
        }

        SetBoardActive(true);

        currentMinigameSceneName = "";
        MinigameSession.SelectedMinigame = null;

        ContinueBoardAfterMinigame();

        isTransitioning = false;
    }

    private void ContinueBoardAfterMinigame()
    {
        if (GameController.instance == null)
        {
            Debug.LogError("MinigameController: no existe GameController al volver del minijuego.");
            return;
        }

        GameController.instance.StartNextTurnAfterMinigame();
    }

    private void ApplyPartySessionResultsToBoard()
    {
        if (PartySession.instance == null)
            return;

        if (GameController.instance == null)
            return;

        if (PartySession.instance.characters == null)
            return;

        for (int i = 0; i < PartySession.instance.characters.Length; i++)
        {
            CharacterSnapshot snap = PartySession.instance.characters[i];

            if (snap == null)
                continue;

            Character boardCharacter = GameController.instance.GetCharacterById(snap.characterId);

            if (boardCharacter == null)
                continue;

            boardCharacter.coins = snap.coins;
            boardCharacter.stars = snap.stars;
        }
    }

    public void SetBoardActive(bool active)
    {
        SetGameObjectsActive(boardCameras, active);
        SetGameObjectsActive(boardCanvasRoots, active);
        SetGameObjectsActive(optionalBoardVisualRoots, active);

        SetBehavioursEnabled(boardInputBehaviours, active);

        if (boardEventSystem != null)
            boardEventSystem.enabled = active;
    }

    private void SetGameObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
            return;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(active);
        }
    }

    private void SetBehavioursEnabled(Behaviour[] behaviours, bool enabled)
    {
        if (behaviours == null)
            return;

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] != null)
                behaviours[i].enabled = enabled;
        }
    }

    private bool IsBoardLoaded()
    {
        Scene boardScene = SceneManager.GetSceneByName(boardSceneName);
        return boardScene.IsValid() && boardScene.isLoaded;
    }
}