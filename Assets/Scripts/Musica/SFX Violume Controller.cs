using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class VolumeController : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private FMOD.Studio.VCA sfxVCA;
    private FMOD.Studio.VCA musicVCA;

    void Start()
    {
        sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        musicVCA = RuntimeManager.GetVCA("vca:/Music");

        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetSFXVolume(float value)
    {
        sfxVCA.setVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        musicVCA.setVolume(value);
    }
}