using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Personajes"))]
public class Personajes : ScriptableObject
{
    [Header("ID")]
    public int IDPersonaje;
    [Header("Sprite")]
    public Sprite ImgPerfil;
    [Header("Animator Controller")]
    public RuntimeAnimatorController Animator;
    [Header("Modelo")]
    public Mesh Malla;
    [Header("Material")]
    public Material Mat;
    [Header("Scala")]
    public Vector3 Scalar;
}
