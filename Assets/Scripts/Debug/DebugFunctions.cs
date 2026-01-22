using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DebugEvent
{
    [Tooltip("Tecla que debe ser presionada para que se ejecute el evento")]
    public KeyCode keyCode;

    [Tooltip("Evento o eventos que se ejecutaran al presionar la tecla")]
    public UnityEvent debugEvent;

    [Tooltip("Asignación de si el evento o eventos asignados son pesados (en carga) o no")]
    public bool isHeavy;
}

public class DebugFunctions : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Añade una KeyCode y un evento para que se ejecute cuando pulses la tecla")]
    private List<DebugEvent> debugEvents;

    [SerializeField]
    [Tooltip("Frames de espera entre evento pesado y evento pesado (para optimización)")]
    private int heavyWaitFrames;

    [SerializeField]
    [Tooltip("Permite que este script funcione en Development builds")]
    private bool workInDevBuild;

    [SerializeField]
    [TextArea(3, 20)]
    [Tooltip("Caja de comentarios para el/los desarrollador/es")]
    private string comments;

    // Cola de eventos que se ejecutaran cada x frames (x es igual al valor de 'heavyWaitFrames' que asignes)
    private Queue<UnityEvent> heavyEvents;

    public static DebugFunctions instance;


    void Awake()
    {
#if DEVELOPMENT_BUILD
        // Si este script no debe trabajar en DevBuilds se desactiva
        if (!workInDevBuild)
        {
            this.enabled = false;
        }
#endif
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
    void Start()
    {
        // Inicializamos la cola de heavyEventsaa
        heavyEvents = new Queue<UnityEvent>();
    }

    void Update()
    {
        foreach (DebugEvent dgEvent in debugEvents)
        {
            if (Input.GetKeyDown(dgEvent.keyCode))
            {
                if (dgEvent.isHeavy)
                {
                    heavyEvents.Enqueue(dgEvent.debugEvent);
                }
                else
                {
                    dgEvent.debugEvent.Invoke();
                }
            }
        }

        if (Time.frameCount % heavyWaitFrames == 0 && heavyEvents.Count > 0)
        {
            heavyEvents.Dequeue().Invoke();
        }
    }
#endif

    #region Add and remove events
    public void AddEvent(DebugEvent dgEvent)
    {
        debugEvents.Add(dgEvent);
    }

    public void RemoveEvent(DebugEvent dgEvent)
    {
        dgEvent.debugEvent.RemoveAllListeners();
        debugEvents.Remove(dgEvent);
    }
    #endregion

}
