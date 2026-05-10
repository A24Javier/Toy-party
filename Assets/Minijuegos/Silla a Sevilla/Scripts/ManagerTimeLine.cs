using UnityEngine;
using UnityEngine.Playables;

public class ManagerTimeLine : MonoBehaviour
{
    PlayableDirector director;

    [SerializeField] AudioSource MiMusica;
    [SerializeField] AudioSource MusicaAddaptible;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    public void PausarAnimation()
    {
        if (director != null)
            director.Pause();
    }

    public void RenaudarAnimation()
    {
        if (director != null)
        {
            director.time = 0;
            director.Play();
        }
    }

    public void ActivarMusicSpecial()
    {
        if (MusicaAddaptible != null && !MusicaAddaptible.isPlaying)
            MusicaAddaptible.Play();
    }

    public void DesactivarMusicSpecial()
    {
        if (MusicaAddaptible != null)
            MusicaAddaptible.Stop();
    }
}