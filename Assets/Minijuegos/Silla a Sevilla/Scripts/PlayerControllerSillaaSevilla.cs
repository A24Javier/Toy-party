using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerControllerSillaaSevilla : MonoBehaviour
{
    public int id;
    public bool Sit = false;
    public bool IA;
    private UnityEngine.InputSystem.PlayerInput MiInput; 
    InputAction Action;
    IASillaASevilla MiIA;

    ChairController SillaMia;

    bool PlayerQuiet, ActionEnCurso;

    [SerializeField] SillaASevillaManager MiManagerContoller;

    Vector3 MiPosition;

    private void Start()
    {
        //id = WhoAmI();
        //IA = YoureARobot();
        MiInput = GetComponent<PlayerInput>();
        Action = MiInput.actions["Sit"];
        MiIA = GetComponent<IASillaASevilla>();
        PlayerQuiet = true;
        ActionEnCurso = false;
        MiPosition = this.gameObject.transform.position;
    }


    //Aqu� cunaod activemos el script de la ia, se llamar a la funcion de la ia, para que pueda coger una silla por un periodo tiempo.
    private void Update()
    {
        if (!PlayerQuiet)
        {
            if (!this.GetIA())
            {
                if (Action.triggered)
                {
                    ActionEnCurso = true;
                }
            }
            else
                ActionEnCurso = true;
        }
        
        if (ActionEnCurso)
        {
            if (GetIA())
                MiIA.MoveIA();
            else
                playerAction();
        }
    }
    private void OnEnable()
    {
        MiManagerContoller.RegistraJugador(this);
    }

    public void PosPlayer()
    {
        transform.position = MiPosition;
        Sit = false;
    }

    //dos funiones para activar los inputs y desconterlos
    //tambien incluimos excepciones en caso de que el player controller sea ia o no
    // Si es Ia actimvaremos su propio escript y desactivaremos el dicho escript

    public void MoveEnable()
    {
        PlayerQuiet = false;
    }

    public void MoveDisabled()
    {
        PlayerQuiet = true;
        ActionEnCurso = false;
    }

    //Aqu� habria que llamar al character para pedir que id tiene este jugador
    int WhoAmI()
    {
        return 0; //Character.GetCharId();
    }

    //aqu� para saber si es un jugador o es ia
    bool YoureARobot()
    {
        return false; //Character.isPlayer;
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
        ChairController[] sillas = Object.FindObjectsOfType<ChairController>();
        ChairController mejorSilla = null;
        float distMin = Mathf.Infinity;

        if (!Sit)
        {
            foreach (ChairController s in sillas)
            {
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
                // Mover al jugador a la posici�n (puedes usar NavMesh o Lerp)
                transform.position = SillaMia.puntoParaSentarse.position;
                Debug.Log(gameObject.name + " se ha sentado.");
                MoveDisabled();
            }
        }
    }
    public bool TieneSilla() => SillaMia != null;
}
