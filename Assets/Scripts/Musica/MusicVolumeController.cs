using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class MusicVolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private FMOD.Studio.VCA musicVCA;
    private bool vcaReady;

    private IEnumerator Start()
    {
        // Seguridad: slider asignado
        if (volumeSlider == null)
        {
            Debug.LogError("MusicVolumeController: volumeSlider no está asignado en el Inspector.");
            yield break;
        }

        // Espera 1-2 frames para que FMOD inicialice y cargue bancos
        yield return null;
        yield return null;

        musicVCA = RuntimeManager.GetVCA("vca:/Music");

        if (!musicVCA.isValid())
        {
            Debug.LogError("MusicVolumeController: No se encontró el VCA 'vca:/Music'. ¿Bancos sin actualizar o ruta incorrecta?");
            yield break;
        }

        vcaReady = true;

        // Conecta el slider
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Opcional: aplica el valor actual del slider al iniciar
        SetVolume(volumeSlider.value);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        if (!vcaReady) return;
        musicVCA.setVolume(Mathf.Clamp01(value));
    }
}