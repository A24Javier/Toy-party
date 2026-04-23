using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private bool alreadyContinuing = false;

    void Start()
    {
        continueText.SetActive(false);

        var mg = MinigameSession.SelectedMinigame;

        if (mg == null)
        {
            Debug.LogError("LoadingScreenUI: MinigameSession.SelectedMinigame es null.");
            return;
        }

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
        if (!canContinue || alreadyContinuing)
            return;

        if (AnyContinuePressed())
        {
            alreadyContinuing = true;
            SceneManager.LoadScene(nextScene);
        }
    }

    private bool AnyContinuePressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) return true;
            if (Gamepad.current.startButton.wasPressedThisFrame) return true;
        }

        return false;
    }
}