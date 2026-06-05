using FMODUnity;
using UnityEngine;
using UnityEngine.Playables;

public class ManagerTimeLine : MonoBehaviour
{
    PlayableDirector director;

    [Header("Asigna tus GameObjects de Audio aquí")]
    [SerializeField] private StudioEventEmitter emisorMiMusica;
    [SerializeField] private StudioEventEmitter emisorMusicaAdaptible;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    #region Control de Timeline
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
    #endregion

    #region Control de Música Especial
    public void ActivarMusicSpecial()
    {
        // Comprobamos si el emisor está asignado y si NO está sonando ya
        if (emisorMusicaAdaptible != null && !emisorMusicaAdaptible.IsPlaying())
        {
            emisorMusicaAdaptible.Play();
        }
    }

    public void DesactivarMusicSpecial()
    {
        if (emisorMusicaAdaptible != null && emisorMusicaAdaptible.IsPlaying())
        {
            emisorMusicaAdaptible.Stop();
        }
    }
    #endregion

    #region Control de Música Base
    public void ActivarMusicaBase()
    {
        if (emisorMiMusica != null && !emisorMiMusica.IsPlaying())
        {
            emisorMiMusica.Play();
        }
    }

    public void DesactivarMusicaBase()
    {
        if (emisorMiMusica != null && emisorMiMusica.IsPlaying())
        {
            emisorMiMusica.Stop();
        }
    }
    #endregion
}