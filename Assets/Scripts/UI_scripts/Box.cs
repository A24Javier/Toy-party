using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Box : MonoBehaviour
{
    public enum BoxType
    {
        Normal,
        Coin,
        Event,
        Path,
        Star
    }

    public enum AnimToThis
    {
        NoAnim,
        Jump,
        SpecialAnim
    }

    [SerializeField] private BoxType type;
    [SerializeField] private int coins = 3;
    [SerializeField] private List<Box> possiblesBoxes;
    [SerializeField] private AnimToThis animToThis = AnimToThis.NoAnim;
    [SerializeField] private bool[] isPathToStar;
    [SerializeField] private UnityEvent EventAction;
    public float camRotationY;
    public float powerJump = 1f;
    public float timeJump = 1f;

    // Keys de animaciones
    private const string ANIM_KEY_WIN_COINS = "isCelebrating";
    private const string ANIM_KEY_LOSE_COINS = "isMourning";

    private const string ANIM_NAME_WIN_COINS = "Victory";
    private const string ANIM_NAME_LOSE_COINS = "Defeat";

    /*public void ActiveEffect(Player player)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log("Player cayo en casilla monedas");
                UIManager.instance.SetActualPlayer(player);
                StartCoroutine(UIManager.instance.UpdateTextCoins(player, rewardCoins));
                break;
            case BoxType.Trap:
                Debug.Log("Player cayo en casilla trampa");
                trapActions.Invoke();
                break;
            case BoxType.Path:
                break;
            case BoxType.Star:
                Debug.Log("Player cayo en casilla estrella");
                break;
        }

        GameController.instance.FinishTurn();
    }*/

    /*public void ActiveNPCEffect(NPC_Controller npc)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log("NPC cayo en casilla monedas");
                // UIManager.instance.SetActualPlayer(player);
                // StartCoroutine(UIManager.instance.UpdateTextCoins(rewardCoins));
                break;
            case BoxType.Trap:
                Debug.Log("NPC cayo en casilla trampa");
                trapActions.Invoke();
                break;
            case BoxType.Path:
                break;
            case BoxType.Star:
                Debug.Log("NPC cayo en casilla estrella");
                break;
        }

        GameController.instance.FinishTurn();
    }*/

    public void ActivateEffect(Character character)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log("Character cayo en casilla monedas");
                UIManager.instance.SetActualCharacter(character);
                StartCoroutine(UIManager.instance.UpdateTextCoins(character, coins));

                StartCoroutine(character.DoAnim((coins > 0) ? ANIM_KEY_WIN_COINS : ANIM_KEY_LOSE_COINS, (coins > 0) ? ANIM_NAME_WIN_COINS : ANIM_NAME_LOSE_COINS));

                break;
            case BoxType.Event:
                Debug.Log("Character cayo en casilla trampa");
                EventAction.Invoke();
                break;
            case BoxType.Path:
                if (character.isPlayer)
                {
                    UIManager.instance.SetActualPlayer(character.gameObject.GetComponent<Player>());
                }
                break;
            case BoxType.Star:
                Debug.Log("Character cayo en casilla estrella");
                UIManager.instance.SetActualCharacter(character);
                UIManager.instance.OpenStarShop(character, coins);

                if (!character.isPlayer)
                {
                    // Iniciamos corutina para que el NPC decida si comprar o no la estrella
                    StartCoroutine(character.gameObject.GetComponent<NPC_Controller>().ProcessBuyStar(coins));
                }
                break;
        }

        StartCoroutine(GameController.instance.FinishTurn());
    }

    public Transform GetNewBoxTransf()
    {
        return possiblesBoxes[0].transform;
    }

    public int PossiblesBoxesCount()
    {
        return possiblesBoxes.Count;
    }

    public Box GetNewBox(int i) { return possiblesBoxes[i]; }


    public Transform GetThisBoxTransf() { return this.transform; }

    public Transform GetBoxTransf(int box)
    {
        return possiblesBoxes[box].transform;
    }

    public bool[] GetIsPathToStar()
    {
        return isPathToStar;
    }

    public string GetAnimToThis()
    {
        return animToThis.ToString();
    }
}
