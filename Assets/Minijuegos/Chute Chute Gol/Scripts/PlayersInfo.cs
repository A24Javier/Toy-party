using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInfo : MonoBehaviour
{
    public int IDCaracter;
    public int turn;
    public int punts;
    public bool IA;
    public bool Portero;

    //Getters
    public int GetID() {  return IDCaracter; }
    public int GetTurn() { return turn; }
    public int GetPunts() {  return punts; }

    public bool GetIa() { return IA; }

    public bool GetPortero() {  return Portero; }

    //Setters 
    public void SetID(int newID) { this.IDCaracter = newID; }
    public void SetTurn(int newTurn) { this.turn = newTurn; }
    public void SetPunts(int newPunts) { this.punts = newPunts;}

    public void SetIa(bool newIa) { this.IA = newIa; }

    public void SetPortero(bool newPortero) { this.Portero = newPortero;}

}
