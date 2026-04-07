using System.Collections;
using UnityEngine;

public class MinigameCharacter : Character
{
    public override void Move(int steps) { }
    protected override IEnumerator MoveCharacterBoard(int steps) { yield break; }
    public override IEnumerator DoAnim(string animationKey, string animationName) { yield break; }
}