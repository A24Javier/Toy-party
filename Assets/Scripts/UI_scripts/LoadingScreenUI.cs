using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image previewImage;
    public GameObject continueText;

    [Header("Config")]
    public float minimumWaitTime = 2f;

    private bool canContinue = false;
    private string nextScene;

    void Start()
    {
        continueText.SetActive(false);

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
            MinigameController.instance.LoadMinigame(nextScene);
        }
    }
}