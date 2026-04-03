using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class UI_StatsInfo : MonoBehaviour
{
    [SerializeField]
    private LocalizedStringTable UITable;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    private int _appFrames;

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
        //CloseStatsInfo();
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
        string secTraduction = UITable.GetTable().GetEntry("UI.Stats.Seconds").Value;

        int totalSeconds = Mathf.FloorToInt(PlayerStats.Instance.PStats.Playtime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int secs = totalSeconds % 60;

        _playtimeTextValue.text = string.Concat(hours, "h ", minutes, "min ", secs, secTraduction);
    }

    private void SetStatsText()
    {
        string gamesPlayedDescText = GetDescriptionText("UI.Stats.GamesPlayed");
        string minigamesPlayedDescText = GetDescriptionText("UI.Stats.MinigamesPlayed");
        string minigamesWonDescText = GetDescriptionText("UI.Stats.MinigamesWon");
        string minigamesLostDescText = GetDescriptionText("UI.Stats.MinigamesLose");

        _gamesPlayedTextValue.text = string.Concat(PlayerStats.Instance.PStats.GamesPlayed, gamesPlayedDescText);
        _minigamesPlayedTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesPlayed, minigamesPlayedDescText);
        _minigamesWonTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesWon, minigamesWonDescText);
        _minigamesLostTextValue.text = string.Concat(PlayerStats.Instance.PStats.MinigamesLost, minigamesLostDescText);
    }

    private string GetDescriptionText(string entryKey)
    {
        string value = UITable.GetTable().GetEntry(entryKey).Value;
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
}
