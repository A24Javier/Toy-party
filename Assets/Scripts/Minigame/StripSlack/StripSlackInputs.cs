using UnityEngine;
using UnityEngine.InputSystem; // Aseg�rate de tener instalado el Input System Package

public class StripSlackInputs: MonoBehaviour
{
    private InputAction accionTecla;
    PlayersControllers InstancePlayer;
    private void Start()
    {
        InstancePlayer = GetComponent<PlayersControllers>();
    }
    void Awake()
    {
        // 1. Creamos la acci�n y le asignamos la tecla directamente
        // Cambia "<Keyboard>/space" por la tecla que prefieras (ej: "<Keyboard>/e")
        accionTecla = new InputAction(binding: "<Mouse>/leftButton");

        // 2. Es obligatorio activar la acci�n para que funcione
        accionTecla.Enable();
    }

    void Update()
    {
        // 3. Comprobamos si la tecla fue presionada en este frame
        if (accionTecla.WasPressedThisFrame())
        {
            InstancePlayer.AugmentForce();
        }
    }

    void OnDestroy()
    {
        // 4. Limpieza para evitar errores de memoria
        accionTecla.Disable();
        accionTecla.Dispose();
    }
}