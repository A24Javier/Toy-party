using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSetting.asset", menuName = ("Data/Create Character setting"))]
public class CharacterSetting : ScriptableObject
{
    public Mesh characterMesh;
    public Ability characterAbility;
    public string characterName;
    public string characterDescription;
    public Sprite characterSprite;
}
