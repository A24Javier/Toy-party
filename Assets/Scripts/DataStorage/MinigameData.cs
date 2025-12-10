using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinigameType { AllVSAll, TwoVSTwo, ThreeVSOne }

[CreateAssetMenu(fileName = "MinigameData", menuName = "Data/Minigame Data")]
public class MinigameData : ScriptableObject
{
    public string minigameName;
    public string description;
    public Sprite previewImage;
    public string sceneName;
    public AudioClip previewMusic;
    public MinigameType type; // ← NUEVO
}