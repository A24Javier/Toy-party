using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersChuteGol : MonoBehaviour
{
    private PlayersInfo MiInfo;

    private bool IaActive;
    private bool MisInputs;
    private bool AsignarAction;
    private bool corrutinaIAActiva;

    [HideInInspector] public string NomAction;

    private UnityEngine.InputSystem.PlayerInput MyInput;
    private InputAction[] MisAcciones = new InputAction[4];

    [SerializeField] private ManagerHUDChutechutegol Manager;

    private Animator MyAnim;

    private void Awake()
    {
        MiInfo = GetComponent<PlayersInfo>();
        MyInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        MyAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (Manager != null)
        {
            Manager.RegistroPlayer(this);
        }
    }

    private void Start()
    {
        if (MyInput == null)
        {
            Debug.LogError("PlayersChuteGol: no se encontró UnityEngine.InputSystem.PlayerInput en " + gameObject.name);
            return;
        }

        if (MyInput.currentActionMap == null)
        {
            Debug.LogError("PlayersChuteGol: currentActionMap es null. Revisa el componente PlayerInput del prefab " + gameObject.name);
            return;
        }

        for (int i = 0; i < MisAcciones.Length; i++)
        {
            string actionName = $"Chute{i + 1}";
            MisAcciones[i] = MyInput.currentActionMap.FindAction(actionName);

            if (MisAcciones[i] == null)
            {
                Debug.LogError($"PlayersChuteGol: no se encontró la acción {actionName} en {gameObject.name}.");
            }
        }
    }

    private void Update()
    {
        if (MisInputs && MiInfo != null && !MiInfo.GetIa())
        {
            CheckHumanInput();
        }
    }

    public void MiInputEnable()
    {
        MisInputs = true;
        AsignarAction = false;

        if (MiInfo != null && MiInfo.GetIa())
        {
            if (!corrutinaIAActiva)
            {
                IaActive = true;
                StartCoroutine(IAccion());
            }
        }
    }

    public void MiInputDisable()
    {
        MisInputs = false;
    }

    private void CheckHumanInput()
    {
        for (int i = 0; i < MisAcciones.Length; i++)
        {
            if (MisAcciones[i] != null && MisAcciones[i].triggered)
            {
                NomAction = MisAcciones[i].name;
                AsignarAction = true;
                PrepararFinalDeTurno();
                break;
            }
        }
    }

    private IEnumerator IAccion()
    {
        if (!IaActive)
            yield break;

        corrutinaIAActiva = true;
        IaActive = false;

        yield return new WaitForSeconds(2f);

        if (!AsignarAction)
        {
            int random = Random.Range(1, MisAcciones.Length + 1);
            NomAction = "Chute" + random;
            AsignarAction = true;
        }

        PrepararFinalDeTurno();

        corrutinaIAActiva = false;
    }

    private void PrepararFinalDeTurno()
    {
        MiInputDisable();
    }

    public string GetNomAction()
    {
        return NomAction ?? "";
    }

    public void SetNom()
    {
        NomAction = "";
    }

    public Animator GetAnim()
    {
        return MyAnim;
    }

    public int GetPunts()
    {
        if (MiInfo == null)
            return 0;

        return MiInfo.GetPunts();
    }

    public void SetPunts()
    {
        if (MiInfo == null)
            return;

        MiInfo.SetPunts(MiInfo.GetPunts() + 1);
    }

    public void SetTurn(int newTurn)
    {
        if (MiInfo == null)
            return;

        MiInfo.SetTurn(newTurn);
    }

    public int getTurn()
    {
        if (MiInfo == null)
            return 0;

        return MiInfo.GetTurn();
    }

    public void setTransform(Vector3 NewTransform)
    {
        transform.position = NewTransform;
    }

    public int GetID()
    {
        if (MiInfo == null)
            return -1;

        return MiInfo.GetID();
    }

    public void SetPortero()
    {
        if (MiInfo == null)
            return;

        MiInfo.SetPortero(true);
    }

    public bool GetPortero()
    {
        if (MiInfo == null)
            return false;

        return MiInfo.GetPortero();
    }

    public bool NewAction()
    {
        AsignarAction = false;
        corrutinaIAActiva = false;
        NomAction = "";
        return AsignarAction;
    }

    public PlayersInfo InfoPlayer()
    {
        return MiInfo;
    }
}