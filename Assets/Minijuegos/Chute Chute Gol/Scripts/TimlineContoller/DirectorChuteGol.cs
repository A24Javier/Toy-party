using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DirectorChuteGol : MonoBehaviour
{
    [SerializeField] ConrtoladorTimeLineChuteChuteGol MiConrtollerTime;

    PlayableDirector MiDire;

    private void Awake()
    {
        MiDire = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        if (MiDire == null || MiDire.playableAsset == null || MiConrtollerTime == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene PlayableDirector, PlayableAsset o controlador asignado.");
            return;
        }

        switch (MiDire.playableAsset.name)
        {
            case "MoverDelanteroTimeline":
                MiConrtollerTime.EstablecerDelantero(this);
                break;

            case "GolAction1Pelota":
                MiConrtollerTime.AñadirElementoGolPelota(0, this);
                break;

            case "GolAction2Pelota":
                MiConrtollerTime.AñadirElementoGolPelota(1, this);
                break;

            case "GolAction3Pelota":
                MiConrtollerTime.AñadirElementoGolPelota(2, this);
                break;

            case "GolAction4Pelota":
                MiConrtollerTime.AñadirElementoGolPelota(3, this);
                break;

            case "GolAction1Portero":
                MiConrtollerTime.AñadirElementoGolPortero(0, this);
                break;

            case "GolAction2Portero":
                MiConrtollerTime.AñadirElementoGolPortero(1, this);
                break;

            case "GolAction3Portero":
                MiConrtollerTime.AñadirElementoGolPortero(2, this);
                break;

            case "GolAction4Portero":
                MiConrtollerTime.AñadirElementoGolPortero(3, this);
                break;

            case "ParadaAction1":
                MiConrtollerTime.AñadirElementoParada(0, this);
                break;

            case "ParadaAction2":
                MiConrtollerTime.AñadirElementoParada(1, this);
                break;

            case "ParadaAction3":
                MiConrtollerTime.AñadirElementoParada(2, this);
                break;

            case "ParadaAction4":
                MiConrtollerTime.AñadirElementoParada(3, this);
                break;
        }
    }

    public void PausarTime()
    {
        if (MiDire != null)
            MiDire.Pause();
    }

    public void StopTime()
    {
        if (MiDire != null)
            MiDire.Stop();
    }

    public void InicarTime()
    {
        if (MiDire != null)
            MiDire.time = 0;
    }

    public void EveluarFrame()
    {
        if (MiDire != null)
            MiDire.Evaluate();
    }

    public void PlayTime()
    {
        if (MiDire != null)
            MiDire.Play();
    }

    public void AsingarAsset(string trackName, Animator anim)
    {
        if (MiDire == null || MiDire.playableAsset == null || anim == null)
            return;

        TimelineAsset timeline = MiDire.playableAsset as TimelineAsset;

        if (timeline == null)
            return;

        var track = timeline.GetOutputTracks().FirstOrDefault(t => t.name == trackName);

        if (track != null)
            MiDire.SetGenericBinding(track, anim);
        else
            Debug.LogWarning($"No se encontró el track {trackName} en {timeline.name}.");
    }
}