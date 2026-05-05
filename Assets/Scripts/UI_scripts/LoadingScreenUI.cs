using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject continueText;

    [Header("Config")]
    [SerializeField] private float minimumWaitTime = 2f;

    private bool canContinue = false;
    private bool alreadyContinuing = false;

    private void Start()
    {
        if (continueText != null)
            continueText.SetActive(false);

        MinigameData mg = MinigameSession.SelectedMinigame;

        if (mg == null)
        {
            Debug.LogError("LoadingScreenUI: MinigameSession.SelectedMinigame es null.");
            return;
        }

        if (titleText != null)
            titleText.text = mg.minigameName;

        if (descriptionText != null)
            descriptionText.text = mg.description;

        if (previewImage != null)
            previewImage.sprite = mg.previewImage;

        StartCoroutine(WaitAndEnableContinue());
    }

    private IEnumerator WaitAndEnableContinue()
    {
        yield return new WaitForSeconds(minimumWaitTime);

        if (continueText != null)
            continueText.SetActive(true);

        canContinue = true;
    }

    private void Update()
    {
        if (!canContinue || alreadyContinuing)
            return;

        if (AnyContinuePressed())
        {
            alreadyContinuing = true;

            if (MinigameController.instance == null)
            {
                Debug.LogError("LoadingScreenUI: no existe MinigameController.instance.");
                return;
            }

            MinigameController.instance.StartSelectedMinigame();
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
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                return true;

            if (Gamepad.current.startButton.wasPressedThisFrame)
                return true;
        }

        return false;
    }
}