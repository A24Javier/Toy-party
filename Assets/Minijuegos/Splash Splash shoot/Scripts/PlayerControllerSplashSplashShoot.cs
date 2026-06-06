using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControllerSplashSplashShoot : MonoBehaviour
{
    private PlayerInfoSplash MiInfoSplash;

    private UnityEngine.InputSystem.PlayerInput inputJugadores;
    private InputAction actionMove;
    private InputAction actionShoot;

    private Rigidbody rb;

    [Header("Invencibilidad")]
    [SerializeField] private float tiempoImbatibleMax = 3f;
    private float timeImbatible;
    private bool imbatible;

    [Header("UI Vida")]
    [SerializeField] private Image[] Lifes;
    [SerializeField] private Sprite LifeSprite;

    [Header("Manager")]
    [SerializeField] private ManagerSplashSplashShoot ManagerJoc;

    [Header("Disparo")]
    [SerializeField] private GameObject Shoot;
    [SerializeField] private Transform PosDisparo;

    [Header("UI Munición")]
    [SerializeField] private Image water;
    [SerializeField] private TextMeshProUGUI textMun;

    private int municionObject;

    private void Awake()
    {
        MiInfoSplash = GetComponent<PlayerInfoSplash>();
        rb = GetComponent<Rigidbody>();
        inputJugadores = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    private void Start()
    {
        if (MiInfoSplash == null)
        {
            Debug.LogError("PlayerControllerSplashSplashShoot: falta PlayerInfoSplash en " + gameObject.name);
            return;
        }

        MiInfoSplash.SetLife(3);
        MiInfoSplash.SetMunicion(MiInfoSplash.GetMunMax());
        MiInfoSplash.SetSpped(8);

        imbatible = false;
        timeImbatible = tiempoImbatibleMax;

        if (!MiInfoSplash.GetIA())
        {
            PrepararInput();
        }
    }

    private void PrepararInput()
    {
        if (inputJugadores == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene UnityEngine.InputSystem.PlayerInput.");
            return;
        }

        if (inputJugadores.currentActionMap == null)
        {
            Debug.LogWarning($"{gameObject.name} tiene PlayerInput pero currentActionMap es null.");
            return;
        }

        inputJugadores.currentActionMap.Enable();

        actionShoot = inputJugadores.currentActionMap.FindAction("Shoot");
        actionMove = inputJugadores.currentActionMap.FindAction("MovePlayer");

        if (actionShoot == null)
            Debug.LogWarning($"{gameObject.name} no encontró la acción 'Shoot'.");

        if (actionMove == null)
            Debug.LogWarning($"{gameObject.name} no encontró la acción 'MovePlayer'.");
    }

    private void OnEnable()
    {
        if (ManagerJoc != null)
            ManagerJoc.AddPlayer(this);
    }

    private void OnDisable()
    {
        if (ManagerJoc != null)
            ManagerJoc.RemovePlayer(this);
    }

    private void Update()
    {
        if (ManagerJoc == null || ManagerJoc.GameTerminated)
            return;

        if (MiInfoSplash == null)
            return;

        if (!MiInfoSplash.GetIA())
            ShootPlayer();

        ImbatibleAction();
    }

    private void FixedUpdate()
    {
        if (ManagerJoc == null || ManagerJoc.GameTerminated)
            return;

        if (MiInfoSplash == null)
            return;

        UpdateInterfaz();

        if (!MiInfoSplash.GetIA())
            MovePlayer();
    }

    private void UpdateInterfaz()
    {
        if (Lifes != null && Lifes.Length >= 3)
        {
            switch (MiInfoSplash.GetLife())
            {
                case 2:
                    Lifes[2].sprite = LifeSprite;
                    break;
                case 1:
                    Lifes[1].sprite = LifeSprite;
                    break;
                case 0:
                    Lifes[0].sprite = LifeSprite;
                    break;
            }
        }

        UpdateMun();
    }

    private void UpdateMun()
    {
        if (ManagerJoc != null && ManagerJoc.Mun != null && ManagerJoc.Mun.Length >= 10 && water != null)
        {
            int mun = MiInfoSplash.GetMunicion();
            int max = MiInfoSplash.GetMunMax();

            if (mun == max)
                water.sprite = ManagerJoc.Mun[0];
            else if (mun < max && mun > 80)
                water.sprite = ManagerJoc.Mun[1];
            else if (mun <= 80 && mun > 70)
                water.sprite = ManagerJoc.Mun[2];
            else if (mun <= 70 && mun > 60)
                water.sprite = ManagerJoc.Mun[3];
            else if (mun <= 60 && mun > 50)
                water.sprite = ManagerJoc.Mun[4];
            else if (mun <= 50 && mun > 40)
                water.sprite = ManagerJoc.Mun[5];
            else if (mun <= 40 && mun > 30)
                water.sprite = ManagerJoc.Mun[6];
            else if (mun <= 30 && mun > 20)
                water.sprite = ManagerJoc.Mun[7];
            else if (mun <= 20 && mun > 10)
                water.sprite = ManagerJoc.Mun[8];
            else if (mun <= 10 && mun > 0)
                water.sprite = ManagerJoc.Mun[9];
        }

        if (textMun != null)
            textMun.text = MiInfoSplash.GetMunicion().ToString();
    }

    private void MovePlayer()
    {
        if (actionMove == null || rb == null)
            return;

        Vector2 input = actionMove.ReadValue<Vector2>();
        Vector3 direccion = new Vector3(input.x, 0, input.y);

        rb.velocity = direccion * MiInfoSplash.GetSpeed();

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 15f);
        }
    }

    private void ShootPlayer()
    {
        if (actionShoot == null)
            return;

        if (actionShoot.triggered)
            Disparo();
    }

    public void Disparo()
    {
        if (MiInfoSplash == null)
            return;

        if (MiInfoSplash.GetMunicion() > 5)
        {
            if (Shoot != null && PosDisparo != null)
                Instantiate(Shoot, PosDisparo.position, transform.rotation);

            int newMun = MiInfoSplash.GetMunicion() - Random.Range(5, 10);
            MiInfoSplash.SetMunicion(Mathf.Max(newMun, 0));
        }
    }

    private void ImbatibleAction()
    {
        if (!imbatible)
            return;

        if (timeImbatible >= 0)
        {
            timeImbatible -= Time.deltaTime;
        }
        else
        {
            timeImbatible = tiempoImbatibleMax;
            imbatible = false;
        }
    }

    private void QuitarVida()
    {
        int life = MiInfoSplash.GetLife();
        life--;
        MiInfoSplash.SetLife(life);

        if (life <= 0)
        {
            UpdateInterfaz();

            if (ManagerJoc != null)
                ManagerJoc.PlayerEliminado(this);

            gameObject.SetActive(false);
        }
        else
        {
            imbatible = true;
        }
    }

    private void RecuperarMunicion(int num)
    {
        int municion = MiInfoSplash.GetMunicion() + num;
        MiInfoSplash.SetMunicion(municion);

        if (MiInfoSplash.GetMunicion() > MiInfoSplash.GetMunMax())
            MiInfoSplash.SetMunicion(MiInfoSplash.GetMunMax());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("shoot"))
        {
            Debug.Log("Le he dado");

            if (!imbatible)
                QuitarVida();
        }
        else if (collision.gameObject.CompareTag("gotaPro"))
        {
            municionObject = Random.Range(5, MiInfoSplash.GetMunMax());
            RecuperarMunicion(municionObject);
            Destroy(collision.gameObject);
        }
    }

    public PlayerInfoSplash GetInfo()
    {
        return MiInfoSplash;
    }

    public ManagerSplashSplashShoot GetManager()
    {
        return ManagerJoc;
    }

    public Rigidbody GetRig()
    {
        return rb;
    }

    public bool GetImbatible()
    {
        return imbatible;
    }
}