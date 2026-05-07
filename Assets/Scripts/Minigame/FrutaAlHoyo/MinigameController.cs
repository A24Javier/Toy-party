using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameController : MonoBehaviour
{
    public static MinigameController instance;

    [Header("Scene Names")]
    [SerializeField] private string boardSceneName = "Prototip";
    [SerializeField] private string loadingSceneName = "LoadingScreen";

    private bool isTransitioning = false;
    private string currentMinigameSceneName = "";

    private readonly List<ComponentState<Camera>> cachedBoardCameras = new List<ComponentState<Camera>>();
    private readonly List<ComponentState<AudioListener>> cachedAudioListeners = new List<ComponentState<AudioListener>>();
    private readonly List<ComponentState<Canvas>> cachedBoardCanvases = new List<ComponentState<Canvas>>();
    private readonly List<ComponentState<GraphicRaycaster>> cachedGraphicRaycasters = new List<ComponentState<GraphicRaycaster>>();
    private readonly List<ComponentState<EventSystem>> cachedEventSystems = new List<ComponentState<EventSystem>>();
    private readonly List<ComponentState<BaseInputModule>> cachedInputModules = new List<ComponentState<BaseInputModule>>();

    private readonly List<ComponentState<Behaviour>> cachedGlobalBoardInputs = new List<ComponentState<Behaviour>>();

    private bool boardComponentsCached = false;

    private class ComponentState<T> where T : Behaviour
    {
        public T component;
        public bool wasEnabled;

        public ComponentState(T component)
        {
            this.component = component;
            this.wasEnabled = component != null && component.enabled;
        }
    }

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
            Debug.LogError("MinigameController: la escena Prototip no está cargada.");
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

    private void SetBoardActive(bool active)
    {
        if (!boardComponentsCached)
            CacheBoardComponents();

        if (active)
            RestoreBoardComponents();
        else
            DisableBoardComponents();
    }

    private void CacheBoardComponents()
    {
        ClearCaches();

        Scene boardScene = SceneManager.GetSceneByName(boardSceneName);

        if (!boardScene.IsValid() || !boardScene.isLoaded)
        {
            Debug.LogError("MinigameController: no se puede cachear Prototip porque no está cargada.");
            return;
        }

        GameObject[] roots = boardScene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            CacheComponentsInRoot(roots[i]);
        }

        CacheGlobalBoardInputScripts();

        boardComponentsCached = true;
    }

    private void CacheComponentsInRoot(GameObject root)
    {
        Camera[] cameras = root.GetComponentsInChildren<Camera>(true);
        for (int i = 0; i < cameras.Length; i++)
            cachedBoardCameras.Add(new ComponentState<Camera>(cameras[i]));

        AudioListener[] audioListeners = root.GetComponentsInChildren<AudioListener>(true);
        for (int i = 0; i < audioListeners.Length; i++)
            cachedAudioListeners.Add(new ComponentState<AudioListener>(audioListeners[i]));

        Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
            cachedBoardCanvases.Add(new ComponentState<Canvas>(canvases[i]));

        GraphicRaycaster[] raycasters = root.GetComponentsInChildren<GraphicRaycaster>(true);
        for (int i = 0; i < raycasters.Length; i++)
            cachedGraphicRaycasters.Add(new ComponentState<GraphicRaycaster>(raycasters[i]));

        EventSystem[] eventSystems = root.GetComponentsInChildren<EventSystem>(true);
        for (int i = 0; i < eventSystems.Length; i++)
            cachedEventSystems.Add(new ComponentState<EventSystem>(eventSystems[i]));

        BaseInputModule[] inputModules = root.GetComponentsInChildren<BaseInputModule>(true);
        for (int i = 0; i < inputModules.Length; i++)
            cachedInputModules.Add(new ComponentState<BaseInputModule>(inputModules[i]));
    }

    private void CacheGlobalBoardInputScripts()
    {
        InputHandler[] inputHandlers = FindObjectsOfType<InputHandler>(true);
        for (int i = 0; i < inputHandlers.Length; i++)
            cachedGlobalBoardInputs.Add(new ComponentState<Behaviour>(inputHandlers[i]));

        VirtualMouseUI[] virtualMice = FindObjectsOfType<VirtualMouseUI>(true);
        for (int i = 0; i < virtualMice.Length; i++)
            cachedGlobalBoardInputs.Add(new ComponentState<Behaviour>(virtualMice[i]));

        MouseParticles[] mouseParticles = FindObjectsOfType<MouseParticles>(true);
        for (int i = 0; i < mouseParticles.Length; i++)
            cachedGlobalBoardInputs.Add(new ComponentState<Behaviour>(mouseParticles[i]));
    }

    private void DisableBoardComponents()
    {
        SetEnabled(cachedBoardCameras, false);
        SetEnabled(cachedAudioListeners, false);
        SetEnabled(cachedBoardCanvases, false);
        SetEnabled(cachedGraphicRaycasters, false);
        SetEnabled(cachedEventSystems, false);
        SetEnabled(cachedInputModules, false);
        SetEnabled(cachedGlobalBoardInputs, false);
    }

    private void RestoreBoardComponents()
    {
        RestoreEnabled(cachedBoardCameras);
        RestoreEnabled(cachedAudioListeners);
        RestoreEnabled(cachedBoardCanvases);
        RestoreEnabled(cachedGraphicRaycasters);
        RestoreEnabled(cachedEventSystems);
        RestoreEnabled(cachedInputModules);
        RestoreEnabled(cachedGlobalBoardInputs);

        boardComponentsCached = false;
        ClearCaches();
    }

    private void SetEnabled<T>(List<ComponentState<T>> list, bool enabled) where T : Behaviour
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].component != null)
                list[i].component.enabled = enabled;
        }
    }

    private void RestoreEnabled<T>(List<ComponentState<T>> list) where T : Behaviour
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].component != null)
                list[i].component.enabled = list[i].wasEnabled;
        }
    }

    private void ClearCaches()
    {
        cachedBoardCameras.Clear();
        cachedAudioListeners.Clear();
        cachedBoardCanvases.Clear();
        cachedGraphicRaycasters.Clear();
        cachedEventSystems.Clear();
        cachedInputModules.Clear();
        cachedGlobalBoardInputs.Clear();
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

    private void ContinueBoardAfterMinigame()
    {
        if (GameController.instance == null)
        {
            Debug.LogError("MinigameController: no existe GameController al volver del minijuego.");
            return;
        }

        GameController.instance.StartNextTurnAfterMinigame();
    }

    private bool IsBoardLoaded()
    {
        Scene boardScene = SceneManager.GetSceneByName(boardSceneName);
        return boardScene.IsValid() && boardScene.isLoaded;
    }
}