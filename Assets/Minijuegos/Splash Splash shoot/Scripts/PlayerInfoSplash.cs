using UnityEngine;

public class PlayerInfoSplash : MonoBehaviour
{
    [Header("Identidad")]
    public int idplayer;
    public bool ia;

    [Header("Stats")]
    public int life;
    private int municion;
    private float speed;
    private int municionMaxima = 100;

    [Header("Resultado")]
    private int posicionFinal = 0;
    private bool eliminado = false;

    public int GetID() { return idplayer; }
    public int GetLife() { return life; }
    public bool GetIA() { return ia; }
    public int GetMunicion() { return municion; }
    public int GetMunMax() { return municionMaxima; }
    public float GetSpeed() { return speed; }
    public int GetPosicionFinal() { return posicionFinal; }
    public bool GetEliminado() { return eliminado; }

    public void SetID(int id) { idplayer = id; }
    public void SetLife(int newLife) { life = newLife; }
    public void SetIA(bool isIA) { ia = isIA; }
    public void SetMunicion(int municionRestante) { municion = municionRestante; }
    public void SetSpped(float newSpeed) { speed = newSpeed; }

    public void SetPosicionFinal(int posicion)
    {
        posicionFinal = posicion;
    }

    public void SetEliminado(bool value)
    {
        eliminado = value;
    }
}