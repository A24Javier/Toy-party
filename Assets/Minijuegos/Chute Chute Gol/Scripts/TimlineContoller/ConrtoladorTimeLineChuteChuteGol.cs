using UnityEngine;

public class ConrtoladorTimeLineChuteChuteGol : MonoBehaviour
{
    enum StatAniamtionChuteGol { Gol, Parada }

    StatAniamtionChuteGol ChuteGolAnim;

    [SerializeField] ManagerHUDChutechutegol Man;

    [SerializeField] GameObject Parada;
    [SerializeField] GameObject Gol;

    string ActionPelota = "";
    string ActionPortero = "";
    string ActionParada = "";

    DirectorChuteGol AnimationDelantero;

    DirectorChuteGol[] AnimationParada = new DirectorChuteGol[4];
    DirectorChuteGol[] AnimationGolPelota = new DirectorChuteGol[4];
    DirectorChuteGol[] AnimationGolPortero = new DirectorChuteGol[4];

    public bool continueScript = false;

    bool Assigned = true;

    string[] ActionsPorteroGol =
    {
        "ActionPortero1Gol",
        "ActionPortero2Gol",
        "ActionPortero3Gol",
        "ActionPortero4Gol"
    };

    string[] ActionsPorteroParada =
    {
        "ActionPortero1Parada",
        "ActionPortero2Parada",
        "ActionPortero3Parada",
        "ActionPortero4Parada"
    };

    public void AñadirElementoParada(int num, DirectorChuteGol Dire)
    {
        if (num >= 0 && num < AnimationParada.Length)
            AnimationParada[num] = Dire;
    }

    public void AñadirElementoGolPelota(int num, DirectorChuteGol Dire)
    {
        if (num >= 0 && num < AnimationGolPelota.Length)
            AnimationGolPelota[num] = Dire;
    }

    public void AñadirElementoGolPortero(int num, DirectorChuteGol Dire)
    {
        if (num >= 0 && num < AnimationGolPortero.Length)
            AnimationGolPortero[num] = Dire;
    }

    public void EstablecerDelantero(DirectorChuteGol Dire)
    {
        AnimationDelantero = Dire;
    }

    public void AssignarDelantero()
    {
        if (Man == null || AnimationDelantero == null)
            return;

        for (int i = 0; i < Man.players.Count; i++)
        {
            if (Man.players[i].getTurn() == 0)
            {
                AnimationDelantero.AsingarAsset("MoverDelantero", Man.players[i].GetAnim());
            }
        }
    }

    public void MoverDelantero()
    {
        if (AnimationDelantero != null)
            AnimationDelantero.PlayTime();
    }

    public void AssignarPortero()
    {
        if (Man == null)
            return;

        for (int i = 0; i < Man.players.Count; i++)
        {
            if (Man.players[i].GetPortero())
            {
                for (int j = 0; j < ActionsPorteroGol.Length; j++)
                {
                    if (AnimationGolPortero[j] != null)
                        AnimationGolPortero[j].AsingarAsset(ActionsPorteroGol[j], Man.players[i].GetAnim());
                }

                for (int j = 0; j < ActionsPorteroParada.Length; j++)
                {
                    if (AnimationParada[j] != null)
                        AnimationParada[j].AsingarAsset(ActionsPorteroParada[j], Man.players[i].GetAnim());
                }

                break;
            }
        }
    }

    public void StartAction()
    {
        if (Assigned)
        {
            continueScript = false;
            Assigned = false;
        }

        StatMachine();
    }

    void StatMachine()
    {
        switch (ChuteGolAnim)
        {
            case StatAniamtionChuteGol.Gol:
                StatGol();
                break;

            case StatAniamtionChuteGol.Parada:
                StatParada();
                break;
        }
    }

    public void Continue()
    {
        continueScript = true;
    }

    void StatGol()
    {
        if (Parada != null) Parada.SetActive(false);
        if (Gol != null) Gol.SetActive(true);

        ActionOnPelota();
        ActonOnPortero();
    }

    void StatParada()
    {
        if (Gol != null) Gol.SetActive(false);
        if (Parada != null) Parada.SetActive(true);

        ActionOnParada();
    }

    void ActionOnPelota()
    {
        int index = ObtenerIndexAction(ActionPelota);

        if (index != -1 && AnimationGolPelota[index] != null)
            AnimationGolPelota[index].PlayTime();
    }

    void ActonOnPortero()
    {
        int index = ObtenerIndexAction(ActionPortero);

        if (index != -1 && AnimationGolPortero[index] != null)
            AnimationGolPortero[index].PlayTime();
    }

    void ActionOnParada()
    {
        int index = ObtenerIndexAction(ActionParada);

        if (index != -1 && AnimationParada[index] != null)
            AnimationParada[index].PlayTime();
    }

    int ObtenerIndexAction(string action)
    {
        switch (action)
        {
            case "Chute1": return 0;
            case "Chute2": return 1;
            case "Chute3": return 2;
            case "Chute4": return 3;
            default: return -1;
        }
    }

    public void SetActions(string actionpelota, string actionportero)
    {
        if (actionpelota == actionportero)
        {
            ActionParada = actionportero;
            ChuteGolAnim = StatAniamtionChuteGol.Parada;
        }
        else
        {
            ActionPelota = actionpelota;
            ActionPortero = actionportero;
            ChuteGolAnim = StatAniamtionChuteGol.Gol;
        }
    }

    public void PausarAnimationPorteroPelota()
    {
        for (int i = 0; i < 4; i++)
        {
            if (AnimationGolPortero[i] != null) AnimationGolPortero[i].PausarTime();
            if (AnimationGolPelota[i] != null) AnimationGolPelota[i].PausarTime();
            if (AnimationParada[i] != null) AnimationParada[i].PausarTime();
        }
    }

    public void ReniciarTimeLines()
    {
        if (AnimationDelantero != null)
        {
            AnimationDelantero.InicarTime();
            AnimationDelantero.EveluarFrame();
        }

        for (int i = 0; i < 4; i++)
        {
            if (AnimationGolPortero[i] != null)
            {
                AnimationGolPortero[i].InicarTime();
                AnimationGolPortero[i].EveluarFrame();
            }

            if (AnimationGolPelota[i] != null)
            {
                AnimationGolPelota[i].InicarTime();
                AnimationGolPelota[i].EveluarFrame();
            }

            if (AnimationParada[i] != null)
            {
                AnimationParada[i].InicarTime();
                AnimationParada[i].EveluarFrame();
            }
        }

        ActionPelota = "";
        ActionPortero = "";
        ActionParada = "";
        Assigned = true;
    }
}