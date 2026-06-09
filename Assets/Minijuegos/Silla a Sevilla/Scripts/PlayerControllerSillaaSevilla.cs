using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControllerSillaaSevilla : MonoBehaviour
{
    [Header("Datos")]
    public int id;
    public bool Sit = false;
    public bool IA;
    public Image perfil;
    public int NumPerosnaje;
    public int Player;

    private UnityEngine.InputSystem.PlayerInput MiInput; 
    InputAction Action;
    IASillaASevilla MiIA;

    ChairController SillaMia;

    bool PlayerQuiet;
    bool ActionEnCurso;

    [SerializeField] SillaASevillaManager MiManagerContoller;

    Vector3 MiPosition;

    [SerializeField] ModelController MiModelo;

    private void Awake()
    {
        MiInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        MiIA = GetComponent<IASillaASevilla>();
    }

    private void OnEnable()
    {
        if (MiManagerContoller != null)
            MiManagerContoller.RegistraJugador(this);
    }

    private void OnDisable()
    {
        if (MiManagerContoller != null)
            MiManagerContoller.DesregistrarJugador(this);
    }

    private void Start()
    {
        PlayerQuiet = true;
        ActionEnCurso = false;
        MiPosition = transform.position;
    }

    private void Update()
    {
        if (!PlayerQuiet)
        {
            if (!GetIA())
            {
                if (Action != null && Action.triggered)
                    ActionEnCurso = true;
            }
            else
            {
                ActionEnCurso = true;
            }
        }

        if (ActionEnCurso)
        {
            if (GetIA())
            {
                if (MiIA != null)
                    MiIA.MoveIA();
            }
            else
            {
                playerAction();
            }
        }
    }

    public void PosPlayer()
    {
        transform.position = MiPosition;

        Sit = false;
        SillaMia = null;
        ActionEnCurso = false;
    }

    public void MoveEnable()
    {
        PlayerQuiet = false;
    }

    public void MoveDisabled()
    {
        PlayerQuiet = true;
        ActionEnCurso = false;
    }

    public int GetId()
    {
        return id;
    }

    public bool GetIA()
    {
        return IA;
    }

    public void playerAction()
    {
        if (Sit)
            return;

        ChairController[] sillas = Object.FindObjectsOfType<ChairController>();

        ChairController mejorSilla = null;
        float distMin = Mathf.Infinity;

        foreach (ChairController s in sillas)
        {
            if (s == null || !s.gameObject.activeInHierarchy)
                continue;

            if (s.EstaLibre())
            {
                float d = Vector3.Distance(transform.position, s.transform.position);

                if (d < distMin)
                {
                    distMin = d;
                    mejorSilla = s;
                }
            }
        }

        if (mejorSilla != null)
        {
            SillaMia = mejorSilla;
            SillaMia.Ocupar(this);

            if (SillaMia.puntoParaSentarse != null)
                transform.position = SillaMia.puntoParaSentarse.position;
            else
                transform.position = SillaMia.transform.position;

            Debug.Log(gameObject.name + " se ha sentado.");

            MoveDisabled();
        }
    }

    public bool TieneSilla()
    {
        return SillaMia != null;
    }

    public void CargarDatosDesdeSnapshot(CharacterSnapshot snap)
    {
        if (snap == null)
            return;

        id = snap.characterId + 1;
        IA = !snap.isPlayer;
        NumPerosnaje = snap.characterSettingIndex;

        ConfigurarInputSegunIA();
        AplicarModeloVisual();

        Debug.Log(
            $"Silla slot {Player}: " +
            $"idJugador={id}, " +
            $"skin={NumPerosnaje}, " +
            $"IA={IA}"
        );
    }

    private void ConfigurarInputSegunIA()
    {
        if (IA)
        {
            Action = null;
            return;
        }

        if (MiInput != null && MiInput.currentActionMap != null)
        {
            Action = MiInput.currentActionMap.FindAction("Sit");
        }

        if (!IA && Action == null)
        {
            Debug.LogWarning($"{gameObject.name} no encontró la acción 'Sit'.");
        }
    }

    public void AplicarModeloVisual()
    {
        if (MiModelo != null)
        {
            MiModelo.AsignarModeloAJugador(NumPerosnaje, Player, perfil);
        }
    }
}