using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Lista de músicas")]
    [SerializeField] private EventReference[] musics;

    private EventInstance musicInstance;
    private bool isPlaying = false;
    private int currentMusicIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMusic(int musicIndex)
    {
        if (musicIndex < 0 || musicIndex >= musics.Length)
        {
            Debug.LogError("Índice de música incorrecto: " + musicIndex);
            return;
        }

        if (musics[musicIndex].IsNull)
        {
            Debug.LogError("La música en el índice " + musicIndex + " está vacía.");
            return;
        }

        if (isPlaying)
        {
            Debug.Log("Ya hay una música sonando.");
            return;
        }

        musicInstance = RuntimeManager.CreateInstance(musics[musicIndex]);
        musicInstance.start();

        isPlaying = true;
        currentMusicIndex = musicIndex;
    }

    public void ChangeMusic(int musicIndex)
    {
        if (musicIndex == currentMusicIndex && isPlaying)
        {
            return;
        }

        StopMusic();
        StartMusic(musicIndex);
    }

    public void StopMusic()
    {
        if (!isPlaying) return;

        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();

        isPlaying = false;
        currentMusicIndex = -1;
    }
}