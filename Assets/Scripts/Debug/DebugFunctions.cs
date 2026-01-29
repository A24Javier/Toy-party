using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DebugEvent
{
    [Tooltip("Tecla que debe ser presionada para que se ejecute el evento")]
    public KeyCode KeyCode;

    [Tooltip("Evento o eventos que se ejecutaran al presionar la tecla")]
    public UnityEvent DebugAction;

    [Tooltip("Asignación de si el evento o eventos asignados son pesados (en carga) o no")]
    public bool IsHeavy;
}

public class DebugFunctions : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Añade una KeyCode y un evento para que se ejecute cuando pulses la tecla")]
    private List<DebugEvent> _debugEvents;

    [SerializeField]
    [Tooltip("Frames de espera entre evento pesado y evento pesado (para optimización)")]
    private int _heavyWaitFrames;

    [SerializeField]
    [Tooltip("Permite que este script funcione en Development builds")]
    private bool _worksInDevBuild;

    [SerializeField]
    [TextArea(3, 20)]
    [Tooltip("Caja de comentarios para el/los desarrollador/es")]
    private string _comments;

    // Cola de eventos que se ejecutaran cada x frames (x es igual al valor de 'heavyWaitFrames' que asignes)
    private Queue<UnityEvent> _heavyEvents;

    public static DebugFunctions Instance;


    void Awake()
    {
#if DEVELOPMENT_BUILD
        // Si este script no debe trabajar en DevBuilds se desactiva
        if (!_worksInDevBuild)
        {
            this.enabled = false;
        }
#endif
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
    void Start()
    {
        // Inicializamos la cola de heavyEvents
        _heavyEvents = new Queue<UnityEvent>();
    }

    void Update()
    {
        foreach (DebugEvent dgEvent in _debugEvents)
        {
            if (Input.GetKeyDown(dgEvent.KeyCode))
            {
                if (dgEvent.IsHeavy)
                {
                    _heavyEvents.Enqueue(dgEvent.DebugAction);
                }
                else
                {
                    dgEvent.DebugAction.Invoke();
                }
            }
        }

        if (Time.frameCount % _heavyWaitFrames == 0 && _heavyEvents.Count > 0)
        {
            _heavyEvents.Dequeue().Invoke();
        }
    }
#endif

    #region Add and remove events
    public void AddEvent(DebugEvent dgEvent)
    {
        _debugEvents.Add(dgEvent);
    }

    public void RemoveEvent(DebugEvent dgEvent)
    {
        dgEvent.DebugAction.RemoveAllListeners();
        _debugEvents.Remove(dgEvent);
    }
    #endregion

}
