using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSnapshot
{
    public int characterId;
    public int coins;
    public int stars;
    public bool isPlayer;
    public Sprite characterImage;

    public int actualBoxIndex;
    public int characterSettingIndex;
}
