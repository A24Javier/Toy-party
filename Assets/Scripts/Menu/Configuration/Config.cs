using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Config : MonoBehaviour
{
    [Header("UI")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    float brightness = 1f;

    const string BRIGHTNESS_KEY = "Brightness";
    const string FULLSCREEN_KEY = "Fullscreen";
    const string QUALITY_KEY = "Quality";
    const string RES_W_KEY = "ResW";
    const string RES_H_KEY = "ResH";

    Resolution[] resolutions;

    void Start()
    {
        brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
        brightnessSlider.value = brightness;
        SetBrightness(brightness);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);

        bool fs = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        Screen.fullScreen = fs;
        fullscreenToggle.isOn = fs;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        int q = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(q);
        qualityDropdown.value = q;
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resOptions = new List<string>();
        foreach (var r in resolutions)
            resOptions.Add(r.width + " x " + r.height);

        resolutionDropdown.AddOptions(resOptions);

        int w = PlayerPrefs.GetInt(RES_W_KEY, Screen.width);
        int h = PlayerPrefs.GetInt(RES_H_KEY, Screen.height);

        int resIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
            if (resolutions[i].width == w && resolutions[i].height == h)
                resIndex = i;

        resolutionDropdown.value = resIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetBrightness(float v)
    {
        brightness = v;
        Color c = brightnessOverlay.color;
        c.a = 1f - v;
        brightnessOverlay.color = c;
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, v);
    }

    void SetFullscreen(bool v)
    {
        Screen.fullScreen = v;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, v ? 1 : 0);
    }

    void SetQuality(int i)
    {
        QualitySettings.SetQualityLevel(i);
        PlayerPrefs.SetInt(QUALITY_KEY, i);
    }

    void SetResolution(int i)
    {
        Resolution r = resolutions[i];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        PlayerPrefs.SetInt(RES_W_KEY, r.width);
        PlayerPrefs.SetInt(RES_H_KEY, r.height);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
