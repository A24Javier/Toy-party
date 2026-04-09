using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class PlayersChuteGol : MonoBehaviour
{
    public int IDCaracter;
    public int turn;
    int punts;
    public bool IA;
    public bool Portero;
    bool MisInputs;
    bool IaActive;
    //variable para salvar la informacion del input
    public string NomAction = "";

    PlayerInput MyInput;
    InputAction[] MisAcciones = new InputAction[3];
    [SerializeField] ManagerHUDChutechutegol Manager;

    private void OnEnable()
    {
        Manager.RegistroPlayer(this);
    }

    private void Start()
    {
        MyInput = GetComponent<PlayerInput>();
        for (int i = 0; i < 3; i++)
        {
            MisAcciones[i] = MyInput.actions[$"Chute{i + 1}"];
        }
        //IDCaracter = RecibirID();
        //IA = Decidir si es ia o no;
        
    }

    private void Update()
    {
        if (MisInputs)
        {
            Action();
        }
    }

    public void MiInputEnable() => MisInputs = true;

    public void MiInputDisable() => MisInputs = false;

    int RecibirID()
    {
        return 0;//Recibe el id del personaje en cuestion 
    }

    public int GetPunts() { return punts; }
    public void SetPunts() => punts++;
    public void SetTurn(int newTurn)
    {
        turn = newTurn;
    }

    public int getTurn()
    {
        return turn;
    }

    public void setTransform(Vector3 NewTransform)
    {
        this.transform.position = NewTransform;
    }
   public int GetID()
    {
        return IDCaracter;
    }

    public void SetPortero() { Portero = true; }
    public bool GetPortero() { return Portero; } 


    void Action()
    {
        if (IA)
        {
            ActionIA();
        }
        else
        {
            for (int i = 0; i < MisAcciones.Length; i++)
            {
                if (MisAcciones[i].triggered)
                {
                    NomAction = MisAcciones[i].name;
                    PrepararFinalDeTurno();
                    break;
                }
            }
        }
    }

    void ActionIA() 
    {
        if (IaActive)
            StartCoroutine(IAccion());       
    }

    IEnumerator IAccion()
    {
        IaActive = false;

        int random = Random.Range(0, 4);

        yield return new WaitForSeconds(2f);


        switch (random)
        {
            case 0: NomAction = "Chute1"; break;
            case 1: NomAction = "Chute2"; break;
            case 2: NomAction = "Chute3"; break;
            case 3: NomAction = "Chute4"; break;
        }
        PrepararFinalDeTurno();
    }

    void PrepararFinalDeTurno()
    {
        MiInputDisable(); // Bloqueamos nuevos inputs inmediatamente
    }
    
    public string GetNomAction() { return NomAction ?? ""; }
    public void IAACTIVE() => IaActive = true;
    public void SetNom() => NomAction = "";

    public int Score => punts;
}