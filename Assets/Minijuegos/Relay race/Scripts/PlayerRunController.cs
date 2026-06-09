using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerRunController : MonoBehaviour
{
    private PlayerInfoRelayRace _infoRelayRace;

    //private PlayerInput _playerInput;
    //private InputAction _jumpAction;
    
    [SerializeField]
    private InputActionReference _jumpAction;

    private bool _jumpPressed = false;

    private Rigidbody _rb;
    private Vector3 _velocityPreJump = Vector3.zero;

    //Getters
    public Rigidbody GetRB() { return _rb; }
    public PlayerInfoRelayRace GetInfoJugador() { return _infoRelayRace; }

    [Header("CargarModelos")]
    [SerializeField] ModelController MiModelo;
    public int idPersonaje;
    public int player;
    public Image img;

    [Header("Progreso de Velocidad")]
    [SerializeField] private float _factorAumentoVelocidad = 0.1f; // Qué tan rápido escala la velocidad por segundo
    [SerializeField] private float _limiteVelocidadAbsoluto = 50f;  // Para que el jugador no alcance una velocidad infinita que rompa las físicas

    bool Recovery;
    float timeRecovery = 2f;

    [SerializeField] RaceController _raceController;

    private void OnEnable()
    {
       _raceController.AñadirJugador(this);
    }

    private void OnDisable()
    {
        _jumpAction.action.performed -= OnJumpPerformed;
    }

    private void Awake()
    {
        _infoRelayRace = GetComponent<PlayerInfoRelayRace>();
    }

    private void Start()
    {
        Recovery = false;
        timeRecovery = 2f;

        //_playerInput = GetComponent<PlayerInput>();
        //_playerInput.Enable();

        //_jumpAction = _playerInput.FindAction("Jump");
        //_jumpAction.Enable();

        _jumpAction.action.performed += OnJumpPerformed;
        _jumpAction.action.Enable();

        _rb = GetComponent<Rigidbody>();

        MiModelo.AsignarModeloAJugador(idPersonaje, player, img);
        AjustarBoxColliderAlModelo();

        Camera miCamaraHija = GetComponentInChildren<Camera>(true);
        if (miCamaraHija != null && miCamaraHija != Camera.main)
        {
            miCamaraHija.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        // ESCUDO TOTAL: Si el controlador de la carrera dice que este jugador NO debe moverse...
        if (!_infoRelayRace.CanMove)
        {
            // Vaciamos su velocidad horizontal para que no se desplace ni un milímetro,
            // pero mantenemos la velocidad en Y por si tiene que caer al suelo al spawnear.
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            _rb.angularVelocity = Vector3.zero; // Evita que rote solo
            return; // Bloquea por completo el resto del Update (no ejecuta Run)
        }

        // A partir de aquí solo entra el jugador al que SÍ le toca correr:
        if (!_infoRelayRace.GetIA())
        {
            SaltoJugador();
        }

        if (!Recovery)
            Run();
        else
            RecoveryTime();
    }

    void RecoveryTime()
    {
        if (timeRecovery > 0) 
            timeRecovery -= Time.deltaTime;
        else
        {
            timeRecovery = 2f;
            Recovery = false;
        }
    }

    public void Run()
    {
        if (!_infoRelayRace.CanMove)
            return;

        // 1. Calculamos el aumento progresivo basado en el tiempo que lleva corriendo
        // Modificamos directamente los valores en el contenedor de información del jugador
        float velocidadActualMax = _infoRelayRace.GetMaxSpeed();

        if (velocidadActualMax < _limiteVelocidadAbsoluto)
        {
            // Aumenta de forma constante tanto la velocidad máxima como la aceleración por cada segundo físico
            float nuevoMaxSpeed = velocidadActualMax + (_factorAumentoVelocidad * Time.fixedDeltaTime);
            _infoRelayRace.SetMaxSpeed(nuevoMaxSpeed); // *Nota: Asegúrate de tener un método SetMaxSpeed en tu otro script

            // También subimos la aceleración un poco para que le cueste menos llegar a ese nuevo tope
            _infoRelayRace.Acceleration += _factorAumentoVelocidad * 0.5f * Time.fixedDeltaTime;
        }

        // --- A partir de aquí es tu lógica física original mapeada con los nuevos valores ---
        float accel = _infoRelayRace.Acceleration;

        if (!_infoRelayRace.GetSuelo())
            accel *= 0.5f;

        Vector3 forwardDir = transform.forward;
        Vector3 targetVelocity = forwardDir * _infoRelayRace.GetMaxSpeed();

        Vector3 velocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        Vector3 delta = targetVelocity - horizontalVelocity;

        float maxChange = accel * Time.fixedDeltaTime;

        Vector3 velocityChange = Vector3.ClampMagnitude(delta, maxChange);

        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void SaltoJugador()
    {
        if (_jumpPressed)
        {
            if (_infoRelayRace.GetSuelo())
                Jump();
            _jumpPressed = false;
        }
    }

    public void Jump()
    {
        _velocityPreJump = _rb.velocity;

        // Cambiamos ForceMode.Impulse por ForceMode.VelocityChange
        _rb.AddForce(Vector3.up * _infoRelayRace.GetForceSalto(), ForceMode.VelocityChange);

        _infoRelayRace.SetSuelo(false);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _jumpPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            _infoRelayRace.SetSuelo(true);
            if(_velocityPreJump != Vector3.zero)
            {
                _rb.AddForce(_velocityPreJump, ForceMode.VelocityChange);
                _velocityPreJump = Vector3.zero;
            }
            Debug.Log("Gracias Tierra");
        }

        if (collision.gameObject.CompareTag("Valla"))
        {
            Debug.Log("¡Ayy! Choque con valla.");

            // 1. Reseteamos los valores de velocidad a los iniciales
            float velocidadNormal = 10f;
            float aceleracionNormal = 5f;
            _infoRelayRace.SetMaxSpeed(velocidadNormal);
            _infoRelayRace.Acceleration = aceleracionNormal;

            // 2. Cancelamos la inercia que traía hacia adelante para que el impacto sea limpio
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            // 3. Calculamos la fuerza del empujón hacia atrás
            // Combinamos un vector hacia atrás (-transform.forward) con un pequeño vector hacia arriba (Vector3.up)
            // para que el personaje haga un pequeño arco de rebote muy estético.
            float fuerzaRetroceso = 5f; // Modifica este número para que empuje más o menos lejos
            float fuerzaLevantamiento = 3f; // Modifica esto para controlar qué tanto despega del suelo al chocar

            Vector3 direccionEmpuje = (-transform.forward * fuerzaRetroceso) + (Vector3.up * fuerzaLevantamiento);

            // 4. Aplicamos el golpe usando ForceMode.Impulse (fuerza instantánea)
            _rb.AddForce(direccionEmpuje, ForceMode.Impulse);
            _infoRelayRace.SetSuelo(false);
            // 5. Activamos el tiempo de recuperación
            Recovery = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LineaCambio"))
        {
            Debug.Log("MiError");

            Frenar();
            
           _raceController.Relevo(this.GetInfoJugador().equipo);
        }
        if (other.gameObject.CompareTag("Meta"))
        {
            Frenar();
            // Fin minijuego
            _raceController.FinalizarPartida(this.GetInfoJugador().equipo);
        }
    }

    void Frenar()
    {
        // 1. Apagamos su capacidad de correr en el script de información
        _infoRelayRace.CanMove = false;

        // 2. Frenamos por completo su velocidad física (tanto lineal como de rotación)
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void AjustarBoxColliderAlModelo()
    {
        // 1. Buscamos el BoxCollider en este objeto
        BoxCollider miCollider = GetComponent<BoxCollider>();
        if (miCollider == null) return;

        // 2. Buscamos el Renderer del modelo (puede estar en un hijo)
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Transformamos los límites globales de la malla a espacio local del jugador
            Bounds limitesLocales = meshRenderer.bounds;

            // Asignamos el centro y el tamaño exacto de la malla al collider
            miCollider.center = transform.InverseTransformPoint(limitesLocales.center);
            miCollider.size = transform.InverseTransformDirection(limitesLocales.size);
        }
    }
}
