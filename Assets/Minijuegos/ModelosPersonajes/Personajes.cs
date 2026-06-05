using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Personajes"))]
public class Personajes : ScriptableObject
{
    public string Nombre;
    public Sprite ImgPerfil;
    [Header("Strip Slack")]
    public RuntimeAnimatorController AnimatorSS;
    [Header("Spalsh Splash Shoot")]
    public RuntimeAnimatorController AniamtorSSS;
    [Header("Relay Race")]
    public RuntimeAnimatorController AnimatorRR;
    [Header("Silla a Sevilla")]
    public RuntimeAnimatorController AnimatorSaS;
    [Header("Chute Chute Gol")]
    public RuntimeAnimatorController AnimatorCCG;
    [Header("Speedy Track")]
    public RuntimeAnimatorController AnimatorST;
    [Header("Hud MiniRecompnesas")]
    public RuntimeAnimatorController AnimatorVictoria;
}
