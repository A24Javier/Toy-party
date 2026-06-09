using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControllerStripSlack : MonoBehaviour
{
    [Header("Datos")]
    public int id;
    public bool EsIA;
    public int player; // Slot del minijuego: 1 o 2

    private float force;

    [Header("Modelo")]
    public int personaje;
    [SerializeField] private ModelController MiModelo;

    [Header("UI")]
    [SerializeField] private Image PerfilImg;
    [SerializeField] private RectTransform MoverCursor;
    [SerializeField] private RectTransform CuadroForce;

    [Header("Manager")]
    [SerializeField] private ManagerStripSlack ManagerStripSlack;

    [Header("Input")]
    private UnityEngine.InputSystem.PlayerInput MiInput;
    private InputAction MiAction;

    private bool clickDetectado = false;

    [Header("Fuerza")]
    [SerializeField] private float fuerzaPorClick = 40f;
    [SerializeField] private float gravedad = 100f;
    [SerializeField] private float limiteMaximo = 340f;

    [Header("IA")]
    [SerializeField] private float dificultadIA = 0.6f;
    private float cronometroIA = 0f;

    [Header("Cuadro")]
    [SerializeField] private float timeDurationPosition = 5f;
    [SerializeField] private float MaxLimiteCuadro = 270f;

    [Header("Puntuación")]
    [SerializeField] private float DarForce = 4.5f;
    [SerializeField] private float LimitForce = 0f;

    private float impulsoActual = 0f;

    private void Awake()
    {
        MiInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    private void OnEnable()
    {
        if (ManagerStripSlack != null)
        {
            ManagerStripSlack.AddPlayer(this);
        }
    }

    private void OnDisable()
    {
        if (ManagerStripSlack != null)
        {
            ManagerStripSlack.RemovePlayer(this);
        }
    }

    private void Start()
    {
        if (id <= 0)
        {
            id = player;
        }

        // El manager asignará EsIA, id y personaje desde PartySession.
        // Por eso aquí no cargamos modelo todavía.
    }

    public void CargarDatosDesdeSnapshot(CharacterSnapshot snap)
    {
        if (snap == null)
            return;

        id = snap.characterId + 1;
        EsIA = !snap.isPlayer;
        SetPersonaje(snap.characterSettingIndex);

        ConfigurarInputSegunIA();
        AplicarModeloVisual();

        Debug.Log(
            $"StripSlack slot {player}: " +
            $"idJugador={id}, " +
            $"skin={snap.characterSettingIndex}, " +
            $"EsIA={EsIA}"
        );
    }

    private void ConfigurarInputSegunIA()
    {
        if (EsIA)
        {
            MiAction = null;
            return;
        }

        if (MiInput == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene UnityEngine.InputSystem.PlayerInput.");
            return;
        }

        if (MiInput.currentActionMap == null)
        {
            Debug.LogWarning($"{gameObject.name} tiene PlayerInput, pero currentActionMap es null.");
            return;
        }

        MiInput.currentActionMap.Enable();

        MiAction = MiInput.currentActionMap.FindAction("Force");

        if (MiAction == null)
        {
            Debug.LogWarning($"{gameObject.name} no encontró la acción 'Force'.");
        }
    }

    public void AplicarModeloVisual()
    {
        if (MiModelo != null)
        {
            MiModelo.AsignarModeloAJugador(GetPersonaje(), player, PerfilImg);
        }
    }

    private void Update()
    {
        if (ManagerStripSlack == null)
            return;

        if (ManagerStripSlack.GetTime() <= 1)
            return;

        if (EsIA)
        {
            ActionIA();
        }
        else
        {
            ActionPlayer();
        }

        TimePostion();
    }

    private void ActionPlayer()
    {
        if (MiAction == null)
            return;

        clickDetectado = MiAction.WasPressedThisFrame();
        MoveAction();
    }

    private void ActionIA()
    {
        clickDetectado = false;

        if (IA_DebePresionar())
        {
            cronometroIA += Time.deltaTime;

            float intervalo = Mathf.Lerp(0.5f, 0.05f, dificultadIA);

            if (cronometroIA >= intervalo)
            {
                impulsoActual += fuerzaPorClick;
                cronometroIA = 0f;
            }
        }
        else
        {
            cronometroIA = 0f;
        }

        MoveAction();
    }

    private void MoveAction()
    {
        if (clickDetectado)
        {
            impulsoActual += fuerzaPorClick;
        }

        impulsoActual -= gravedad * Time.deltaTime;

        float distanciaTotal = limiteMaximo * 2f;
        impulsoActual = Mathf.Clamp(impulsoActual, 0f, distanciaTotal);

        float xFinal;

        if (player == 1)
        {
            xFinal = limiteMaximo - impulsoActual;
        }
        else
        {
            xFinal = -limiteMaximo + impulsoActual;
        }

        if (MoverCursor != null)
        {
            MoverCursor.anchoredPosition = new Vector2(xFinal, 0);
        }

        ComprobarAction();
    }

    private void ComprobarAction()
    {
        if (IsInsideBox())
        {
            AumentForcePersonaje(DarForce);
        }
        else
        {
            LowForcePersonaje(DarForce);
        }
    }

    private void AumentForcePersonaje(float aument)
    {
        force += aument * Time.deltaTime;
    }

    private void LowForcePersonaje(float low)
    {
        if (force > LimitForce)
        {
            force -= low * Time.deltaTime;
        }

        if (force < LimitForce)
        {
            force = LimitForce;
        }
    }

    private void TimePostion()
    {
        if (timeDurationPosition > 0)
        {
            timeDurationPosition -= Time.deltaTime;
        }
        else
        {
            timeDurationPosition = 5f;
            MoveCuadro();
        }
    }

    private void MoveCuadro()
    {
        if (CuadroForce == null)
            return;

        float nuevaPosicion = Random.Range(-MaxLimiteCuadro, MaxLimiteCuadro);
        CuadroForce.localPosition = new Vector2(nuevaPosicion, 0);
    }

    private bool IA_DebePresionar()
    {
        if (CuadroForce == null || MoverCursor == null)
            return false;

        float posicionCuadroX = CuadroForce.localPosition.x;
        float miPosicionX = MoverCursor.localPosition.x;

        float margen = 10f;

        if (player == 1)
        {
            return miPosicionX > posicionCuadroX - margen;
        }
        else
        {
            return miPosicionX < posicionCuadroX + margen;
        }
    }

    private bool IsInsideBox()
    {
        if (CuadroForce == null || MoverCursor == null)
            return false;

        float cuadroX = CuadroForce.localPosition.x;
        float mitadAncho = CuadroForce.rect.width / 2f;

        float limiteIzquierdo = cuadroX - mitadAncho;
        float limiteDerecho = cuadroX + mitadAncho;

        float jugadorX = MoverCursor.localPosition.x;

        return jugadorX >= limiteIzquierdo && jugadorX <= limiteDerecho;
    }

    public float GetForce()
    {
        return force;
    }

    public void SetPersonaje(int IDPersonaje)
    {
        personaje = IDPersonaje;
    }

    public int GetPersonaje()
    {
        return personaje;
    }

    public Image GetPerfil()
    {
        return PerfilImg;
    }
}