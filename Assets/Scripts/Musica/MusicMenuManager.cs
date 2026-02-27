using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MenuMusicManager : MonoBehaviour
{
    [SerializeField] private EventReference menuMusic;

    private EventInstance musicInstance;
    private bool isPlaying = false;

    public void PlayMusic()
    {
        if (isPlaying) return;

        musicInstance = RuntimeManager.CreateInstance(menuMusic);
        musicInstance.start();
        isPlaying = true;
    }

    public void StopMusic()
    {
        if (!isPlaying) return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); musicInstance.release();
        isPlaying = false;
    }
}