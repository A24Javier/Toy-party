using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoRelayRace : MonoBehaviour
{
    public int Id;
    public bool Ia;
    private float _fuerzaSalto = 6f;

    private float _speed = 3f;
    private const float MAX_SPEED = 8f;

    private float _acceleration = 16f;
    public float Acceleration => _acceleration;

    private bool _isGrounded;

    public bool CanMove = false;

    //geters
    public int GetID() { return Id; }
    public bool GetIA() { return Ia; }
    public float GetForceSalto() { return _fuerzaSalto; }

    public float GetMaxSpeed() { return MAX_SPEED; }

    public float GetSpeed() {  return _speed; }

    public bool GetSuelo() {  return _isGrounded; }
    //set
    public void SetID(int id) => this.Id = id;
    public void SetIa(bool ia) => this.Ia = ia;

    public void SetSuelo(bool suelo) => this._isGrounded = suelo; 
}