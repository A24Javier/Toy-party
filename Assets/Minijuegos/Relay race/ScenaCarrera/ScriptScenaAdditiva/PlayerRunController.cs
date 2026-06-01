using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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


    private void Start()
    {
        _infoRelayRace = GetComponent<PlayerInfoRelayRace>();
        /*
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.Enable();

        _jumpAction = _playerInput.FindAction("Jump");
        _jumpAction.Enable();
        */

        _jumpAction.action.performed += OnJumpPerformed;
        _jumpAction.action.Enable();

        _rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (!_infoRelayRace.GetIA())
        {
            SaltoJugador();
        }
    }

    private void FixedUpdate()
    {
        Run();
    }

    public void Run()
    {
        if (!_infoRelayRace.CanMove)
            return;

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

        // Accion de saltar
        _rb.AddForce(Vector3.up * _infoRelayRace.GetForceSalto(), ForceMode.Impulse);
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
        }

        if (collision.gameObject.CompareTag("Meta"))
        {
            // Fin minijuego
        }
    }
}
