using UnityEngine;

public class TestMinigameLauncher : MonoBehaviour
{
    [Header("Minijuego a probar")]
    [SerializeField] private MinigameData minigameToTest;

    [Header("Input")]
    [SerializeField] private KeyCode startKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(startKey))
        {
            StartTestMinigame();
        }
    }

    private void StartTestMinigame()
    {
        if (minigameToTest == null)
        {
            Debug.LogError("TestMinigameLauncher: no has asignado ningún MinigameData.");
            return;
        }

        if (MinigameController.instance == null)
        {
            Debug.LogError("TestMinigameLauncher: no existe MinigameController en la escena.");
            return;
        }

        MinigameSession.SelectedMinigame = minigameToTest;

        Debug.Log("TEST: Abriendo LoadingScene para " + minigameToTest.minigameName);

        MinigameController.instance.OpenLoadingScene();
    }
}