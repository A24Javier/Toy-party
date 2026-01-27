using UnityEngine;

public abstract class AbilityFunction : ScriptableObject
{
    public Ability Ability;
    public abstract void UseAbility();
}
