using UnityEngine;

public class MusicButtonCaller : MonoBehaviour
{
    [Header("Número de música en la lista del MusicManager")]
    [SerializeField] private int musicIndex = 0;

    public void StartMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StartMusic(musicIndex);
        }
    }

    public void ChangeMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ChangeMusic(musicIndex);
        }
    }

    public void StopMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
        }
    }
}