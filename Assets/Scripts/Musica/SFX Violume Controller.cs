using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
public class SFXViolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private FMOD.Studio.VCA musicVCA;

    void Start()
    {
        musicVCA = RuntimeManager.GetVCA("vca:/SFX");

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        musicVCA.setVolume(value);
    }
}
