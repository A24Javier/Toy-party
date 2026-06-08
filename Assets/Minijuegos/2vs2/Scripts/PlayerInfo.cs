using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int ID;
    public bool IA;
    public int posicion;
    public int idPersonaje;

    //Getters
    public int GetID() { return ID; }
    public bool GetIA() { return IA; }
    public int GetPersonaje() { return idPersonaje; }

    //Setters
    public void SetID(int id) { this.ID = id; }
    public void SetIA(bool IA) { this.IA = IA; }
    public void SetIdPersonaje(int idPers) {  this.idPersonaje = idPers; }

}
