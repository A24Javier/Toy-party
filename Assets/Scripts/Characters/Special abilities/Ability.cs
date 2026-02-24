using UnityEngine;

[CreateAssetMenu(menuName = ("Abilities/Create Ability"))]
public class Ability : ScriptableObject
{
    public string AbilityName;

    [TextArea(3, 10)]
    public string Description;

    public Sprite AbilitySprite;
    public Color BackgroundColor;

    public bool PayForUse;
    public int AbilityPrice;

    public Buff[] PassiveBuffs;

    public AbilityFunction AbilityFunction;
}
