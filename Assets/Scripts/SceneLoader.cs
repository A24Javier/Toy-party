using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        if (sceneName.StartsWith("MG_"))
        {
            Debug.LogWarning($"Intento de cargar minijuego directo ({sceneName}). Redirigiendo a LoadingScene.");
            SceneManager.LoadScene("LoadingScene");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

}