using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class UI_StatsInfo : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private int _appFrames;

    [Header("Traduction values")]
    /*
    [SerializeField]
    private LocalizedStringTable UITable;
    */
    [SerializeField]
    private LocalizedString _secondsTraduc;
    private string _secondsTraducValue;

    [SerializeField]
    private LocalizedString _gamesPlayedTraduc;
    private string _gamesPlayedTraducValue;

    [SerializeField]
    private LocalizedString _minigamesPlayedTraduc;
    private string _minigamesPlayedTraducValue;

    [SerializeField]
    private LocalizedString _minigamesWonTraduc;
    private string _minigamesWonTraducValue;

    [SerializeField]
    private LocalizedString _minigamesLostTraduc;
    private string _minigamesLostTraducValue;

    [Space]
    [Header("TMP_Text components")]

    [SerializeField]
    private TMP_Text _playtimeTextValue;

    [SerializeField]
    private TMP_Text _gamesPlayedTextValue;

    [SerializeField]
    private TMP_Text _minigamesPlayedTextValue;

    [SerializeField]
    private TMP_Text _minigamesWonTextValue;

    [SerializeField]
    private TMP_Text _minigamesLostTextValue;

    void Awake()
    {
        _appFrames = Application.targetFrameRate;
    }

    void Start()
    {
        _secondsTraduc.StringChanged += UpdateSeconds;

        CloseStatsInfo();
        SetStatsText();
    }

    void Update()
    {
        if(Time.frameCount % _appFrames == 0)
        {
            CalculatePlaytime();
        }
    }

    private void CalculatePlaytime()
    {
        int totalSeconds = Mathf.FloorToInt(PlayerStats.Instance.PStats.Playtime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int secs = totalSeconds % 60;

        _playtimeTextValue.text = string.Concat(hours, "h ", minutes, "min ", secs, _secondsTraducValue);
    }

    private void SetStatsText()
    {
        string gamesPlayedDescText = GetDescriptionText(_gamesPlayedTraducValue);
        string minigamesPlayedDescText = GetDescriptionText(_minigamesPlayedTraducValue);
        string minigamesWonDescText = GetDescriptionText(_minigamesWonTraducValue);
        string minigamesLostDescText = GetDescriptionText(_minigamesLostTraducValue);

        _gamesPlayedTextValue.text = string.Concat(PlayerStats.Instance.PStats.GamesPlayed, gamesPlayedDescText);
        _minigamesPlayedTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesPlayed, minigamesPlayedDescText);
        _minigamesWonTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesWon, minigamesWonDescText);
        _minigamesLostTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesLost, minigamesLostDescText);
    }

    private string GetDescriptionText(string value)
    {
        value = value.ToLower();
        value = " " + value.Substring(0, value.Length - 1);

        return value;
    }

    public void OpenStatsInfo()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void CloseStatsInfo()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void UpdateSeconds(string value)
    {
        _secondsTraducValue = value;
    }
}
