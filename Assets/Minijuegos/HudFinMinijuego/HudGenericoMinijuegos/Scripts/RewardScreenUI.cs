using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RewardScreenUI : MonoBehaviour
{
    [System.Serializable]
    public class RewardRow
    {
        public GameObject root;
        public Image characterImage;
        public TMP_Text positionText;
        public TMP_Text coinsText;
    }

    [Header("Rows")]
    [SerializeField] private RewardRow[] rows;

    [Header("Config")]
    [SerializeField] private bool applyRewardsOnStart = false;

    private bool rewardsApplied = false;
    private bool canContinue = false;

    private void Start()
    {
        ShowResults();

        if (applyRewardsOnStart)
        {
            ApplyRewardsToBoard();
        }

        canContinue = true;
    }

    private void Update()
    {
        if (!canContinue)
            return;

        if (AnyContinuePressed())
        {
            canContinue = false;

            if (!rewardsApplied)
            {
                ApplyRewardsToBoard();
            }

            MinigameResultSession.Clear();

            if (MinigameController.instance == null)
            {
                Debug.LogError("RewardScreenUI: no existe MinigameController.");
                return;
            }

            MinigameController.instance.EndRewardsAndReturnToBoard();
        }
    }

    private void ShowResults()
    {
        if (MinigameResultSession.Results == null || MinigameResultSession.Results.Count == 0)
        {
            Debug.LogWarning("RewardScreenUI: no hay resultados de minijuego.");
            return;
        }

        var orderedResults = MinigameResultSession.Results
            .OrderBy(r => r.position)
            .ToList();

        for (int i = 0; i < rows.Length; i++)
        {
            if (i >= orderedResults.Count)
            {
                if (rows[i].root != null)
                    rows[i].root.SetActive(false);

                continue;
            }

            MinigameResultData result = orderedResults[i];

            if (rows[i].root != null)
                rows[i].root.SetActive(true);

            if (rows[i].positionText != null)
                rows[i].positionText.text = "#" + result.position;

            if (rows[i].coinsText != null)
                rows[i].coinsText.text = "+" + result.coinsWon + " monedas";

            Character character = null;

            if (GameController.instance != null)
                character = GameController.instance.GetCharacterByCharacterId(result.characterId);

            if (character != null && rows[i].characterImage != null)
                rows[i].characterImage.sprite = character.GetCharImage();
        }
    }

    private void ApplyRewardsToBoard()
    {
        if (rewardsApplied)
            return;

        if (GameController.instance == null)
        {
            Debug.LogError("RewardScreenUI: no existe GameController.");
            return;
        }

        foreach (MinigameResultData result in MinigameResultSession.Results)
        {
            Character character = GameController.instance.GetCharacterByCharacterId(result.characterId);

            if (character == null)
            {
                Debug.LogWarning("RewardScreenUI: no se encontró personaje con characterId: " + result.characterId);
                continue;
            }

            character.SetCoins(character.GetCoins() + result.coinsWon);
        }

        rewardsApplied = true;
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