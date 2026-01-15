using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // Character info
    public int characterId;
    public int coins;
    public int stars;
    public Sprite characterImage;
    public Box actualBox;
    protected Inventory inventory = new Inventory();
    public bool isPlayer;
    public float savedCameraRotationY = 0f;


    // Character values
    public float speed = 0.001f;
    public int extraStep = 0;

    // Character things
    protected ParticleSystem runningParticles;

    public abstract void Move(int steps);
    protected abstract IEnumerator MoveCharacterBoard(int steps);
    public abstract IEnumerator DoAnim(string animationKey, string animationName);

    // Getters y setters
    public virtual int GetCharId() { return characterId; }
    public virtual void SetCharId(int id)
    {
        this.characterId = id;
    }

    public virtual int GetCoins() {  return coins; }
    public virtual void SetCoins(int coins)
    {
        this.coins = coins;
    }

    public virtual int GetStars() { return stars; }
    public virtual void SetStars(int stars)
    {
        this.stars = stars;
    }

    public virtual Sprite GetCharImage() { return characterImage; }
    public virtual void SetCharImage(Sprite characterImage)
    {
        this.characterImage = characterImage;
    }

    public virtual Box GetActualBox() { return actualBox; }

    public virtual Inventory GetInventory()
    {
        return inventory;
    }

    public virtual void DeleteInventory()
    {
        inventory = null;
        inventory = new Inventory();
    }

    public virtual void UseItem(int index)
    {
        GetInventory().GetItem(index);
    }

    public virtual void SetExtraStep(int value)
    {
        extraStep += value;
    }
}
