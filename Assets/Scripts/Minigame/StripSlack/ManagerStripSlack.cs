using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerStripSlack : MonoBehaviour
{
    [Header("Toy Party integration")]
    [SerializeField] private MinigameCharacterManager characterManager;
    [SerializeField] private string boardSceneName = "Prototip";

    [Header("Match config (1vs1)")]
    [Tooltip("Índices de los 4 slots (0..3) que juegan este 1vs1.")]
    [SerializeField] private int activeIndexA = 0;
    [SerializeField] private int activeIndexB = 1;

    [Header("Timer")]
    [SerializeField] private float matchTimeSeconds = 30f;

    [Header("Rewards by position")]
    [Tooltip("coinsAward[0]=1º, [1]=2º, [2]=3º, [3]=4º")]
    [SerializeField] private int[] coinsAward = new int[4] { 10, 5, 2, 0 };
    [Tooltip("starsAward[0]=1º, [1]=2º, [2]=3º, [3]=4º")]
    [SerializeField] private int[] starsAward = new int[4] { 0, 0, 0, 0 };

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI forceAText;
    [SerializeField] private TextMeshProUGUI forceBText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject victoryPanel;

    private Character[] chars;                 // 4 spawneados
    private PlayersControllers ctrlA;
    private PlayersControllers ctrlB;
    private IAController iaA;
    private IAController iaB;

    private float timeLeft;
    private bool finished;

    private void Start()
    {
        if (characterManager == null)
            characterManager = FindObjectOfType<MinigameCharacterManager>();

        chars = characterManager.GetCharacters();
        if (chars == null || chars.Length < 4)
        {
            Debug.LogError("StripSlack: No hay 4 personajes spawneados. Revisa MinigameCharacterManager.");
            return;
        }

        timeLeft = matchTimeSeconds;
        finished = false;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (resultText != null) resultText.text = "";

        SetupParticipants();
        UpdateUI(0f, 0f);
    }

    private void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        float fA = GetForce(activeIndexA);
        float fB = GetForce(activeIndexB);

        UpdateUI(fA, fB);

        if (timeLeft <= 0f)
            FinishMatch(fA, fB);
    }

    private void SetupParticipants()
    {
        // 1) Desactivar inputs/IA de todos por defecto
        for (int i = 0; i < 4; i++)
            SetControlEnabled(i, enabled: false);

        // 2) Activar solo los dos índices del 1vs1
        SetControlEnabled(activeIndexA, enabled: true);
        SetControlEnabled(activeIndexB, enabled: true);

        // cache de componentes (para UI)
        ctrlA = chars[activeIndexA].GetComponent<PlayersControllers>();
        ctrlB = chars[activeIndexB].GetComponent<PlayersControllers>();
        iaA = chars[activeIndexA].GetComponent<IAController>();
        iaB = chars[activeIndexB].GetComponent<IAController>();
    }

    private void SetControlEnabled(int idx, bool enabled)
    {
        var c = chars[idx];

        // input humano
        var input = c.GetComponent<StripSlackInputs>();
        if (input != null) input.enabled = enabled && c.isPlayer;

        // IA NPC
        var ia = c.GetComponent<IAController>();
        if (ia != null) ia.enabled = enabled && !c.isPlayer;

        // Player force script siempre puede estar (no molesta)
        var pc = c.GetComponent<PlayersControllers>();
        if (pc != null) pc.enabled = enabled;
    }

    private float GetForce(int idx)
    {
        var c = chars[idx];

        // Si es player: usamos PlayersControllers.forcePlayer
        var pc = c.GetComponent<PlayersControllers>();
        if (c.isPlayer && pc != null) return pc.forcePlayer;

        // Si es NPC: usamos IAController.forceIA
        var ia = c.GetComponent<IAController>();
        if (!c.isPlayer && ia != null) return ia.forceIA;

        // Fallback (si faltan componentes)
        if (pc != null) return pc.forcePlayer;
        if (ia != null) return ia.forceIA;
        return 0f;
    }

    private void UpdateUI(float fA, float fB)
    {
        if (timeText != null)
        {
            float minutes = Mathf.FloorToInt(timeLeft / 60f);
            float seconds = Mathf.FloorToInt(timeLeft % 60f);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (forceAText != null) forceAText.text = $"Fuerza A: {fA:0.0}";
        if (forceBText != null) forceBText.text = $"Fuerza B: {fB:0.0}";
    }

    private void FinishMatch(float fA, float fB)
    {
        finished = true;

        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Ranking simple:
        // - En 1vs1: ganador = puesto 1, perdedor = puesto 2
        // - Los otros dos: puestos 3 y 4 (en orden de índice, o puedes cambiarlos)
        int winner = (fA >= fB) ? activeIndexA : activeIndexB;
        int loser = (winner == activeIndexA) ? activeIndexB : activeIndexA;

        // Los no participantes (los 2 restantes)
        int[] rest = new int[2];
        int r = 0;
        for (int i = 0; i < 4; i++)
            if (i != winner && i != loser)
                rest[r++] = i;

        int[] positions = new int[4]; // positions[pos] = index jugador en ese puesto
        positions[0] = winner;  // 1º
        positions[1] = loser;   // 2º
        positions[2] = rest[0]; // 3º
        positions[3] = rest[1]; // 4º

        // Recompensas por puesto
        for (int pos = 0; pos < 4; pos++)
        {
            int idx = positions[pos];
            chars[idx].coins += SafeGet(coinsAward, pos);
            chars[idx].stars += SafeGet(starsAward, pos);
        }

        if (resultText != null)
            resultText.text = (winner == activeIndexA) ? "Gana A" : "Gana B";

        // Guardar resultados a PartySession
        characterManager.SaveResults();

        // Volver al tablero
        SceneManager.LoadScene(boardSceneName);
    }

    private int SafeGet(int[] arr, int i)
    {
        if (arr == null || arr.Length <= i) return 0;
        return arr[i];
    }
}