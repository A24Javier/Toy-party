using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public Text titleText;
    public Text descriptionText;
    public Image previewImage;
    public GameObject continueText; // Texto "Click para continuar"

    [Header("Config")]
    public float minimumWaitTime = 2f; // Tiempo antes de poder continuar

    private bool canContinue = false;
    private string nextScene;

    void Start()
    {
        continueText.SetActive(false); // Ocultar el texto al inicio

        var mg = MinigameSession.SelectedMinigame;
        titleText.text = mg.minigameName;
        descriptionText.text = mg.description;
        previewImage.sprite = mg.previewImage;
        nextScene = mg.sceneName;

        StartCoroutine(WaitAndEnableContinue());
    }

    IEnumerator WaitAndEnableContinue()
    {
        yield return new WaitForSeconds(minimumWaitTime);

        continueText.SetActive(true);
        canContinue = true;
    }

    void Update()
    {
        if (!canContinue)
            return;

        if (InputHandler.instance != null && InputHandler.instance.IsSpacebarTouched())
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
    }
}
