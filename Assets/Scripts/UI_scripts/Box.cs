using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public enum BoxType
{
    Normal,
    Coin,
    Event,
    Path,
    Star
}

[System.Serializable]
public class PlayerOutBoxPos
{
    [HideInInspector] public bool IsFilled = false;
    public Transform TransfPos = null;

    public void GoToPosition(Transform charTransf)
    {
        Vector3 pos = new Vector3(TransfPos.position.x, charTransf.position.y, TransfPos.position.z);
        charTransf.DOMove(pos, 1f);
    }
}

public class Box : MonoBehaviour
{
    public enum AnimToThis
    {
        NoAnim,
        Jump,
        SpecialAnim
    }

    public BoxType type;
    [SerializeField] private int coins = 3;
    [SerializeField] private List<Box> possiblesBoxes;
    [SerializeField] private Box lastBox;
    public Box LastBox => lastBox;

    public bool IsTowerOnIt = false;
    [SerializeField] private GameObject towerObject;
    private int towerActiveTurns = 0;
    private const int MaxTowerTurns = 2;

    private bool areTrapOnIt = false;
    private GameObject trapObject;
    private int trapActiveTurns = 0;
    private const int MaxTrapTurns = 2;

    [SerializeField] private AnimToThis animToThis = AnimToThis.NoAnim;
    [SerializeField] private bool[] isPathToStar;
    [SerializeField] private UnityEvent EventAction;
    public float camRotationY;
    public float powerJump = 1f;
    public float timeJump = 1f;

    [SerializeField] private PlayerOutBoxPos[] playerBoxesPos;

    private const string ANIM_KEY_WIN_COINS = "isCelebrating";
    private const string ANIM_KEY_LOSE_COINS = "isMourning";

    private const string ANIM_NAME_WIN_COINS = "Victory";
    private const string ANIM_NAME_LOSE_COINS = "Defeat";

    [Header("Star System")]
    [SerializeField] private Renderer[] boxRenderers;

    private static List<Box> starBoxes = new List<Box>();
    private static Box currentStarBox;
    private static bool starInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStarStatics()
    {
        starBoxes = new List<Box>();
        currentStarBox = null;
        starInitialized = false;
    }

    private void CacheRenderers()
    {
        if (boxRenderers == null || boxRenderers.Length == 0)
            boxRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void SetBoxVisible(bool visible)
    {
        CacheRenderers();
        foreach (var r in boxRenderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    public static Box GetCurrentStarBox()
    {
        return currentStarBox;
    }

    private static void SetStarBox(Box box)
    {
        for (int i = 0; i < starBoxes.Count; i++)
        {
            if (starBoxes[i] != null)
                starBoxes[i].SetBoxVisible(false);
        }

        currentStarBox = box;

        if (currentStarBox != null)
            currentStarBox.SetBoxVisible(true);
    }

    public static void MoveStarToRandom(Box exclude)
    {
        if (starBoxes.Count == 0) return;

        List<Box> candidates = new List<Box>();
        for (int i = 0; i < starBoxes.Count; i++)
        {
            if (starBoxes[i] != null)
                candidates.Add(starBoxes[i]);
        }

        if (exclude != null && candidates.Count > 1)
            candidates.Remove(exclude);

        if (candidates.Count == 0) return;

        SetStarBox(candidates[Random.Range(0, candidates.Count)]);
    }

    private void OnEnable()
    {
        CacheRenderers();

        if (type == BoxType.Star)
        {
            if (!starBoxes.Contains(this))
                starBoxes.Add(this);

            if (!starInitialized)
            {
                SetBoxVisible(false);
            }
            else
            {
                SetBoxVisible(currentStarBox == this);
            }
        }
    }

    private void OnDisable()
    {
        if (type == BoxType.Star)
        {
            starBoxes.Remove(this);

            // Si esta era la estrella actual, la limpiamos
            if (currentStarBox == this)
                currentStarBox = null;
        }
    }

    private void Start()
    {
        if (!starInitialized && starBoxes.Count > 0)
        {
            starInitialized = true;

            for (int i = 0; i < starBoxes.Count; i++)
            {
                if (starBoxes[i] != null)
                    starBoxes[i].SetBoxVisible(false);
            }

            SetStarBox(starBoxes[Random.Range(0, starBoxes.Count)]);
        }

        GameController.instance.OnRoundEnded.AddListener(UpdateTurns);
    }

    public void ActivateEffect(Character character)
    {
        switch (type)
        {
            case BoxType.Coin:
                Debug.Log("Character cayo en casilla monedas");
                UIManager.instance.SetActualCharacter(character);

                int buffCoins = 0;

                foreach (Buff buff in character.buffs)
                {
                    if(coins > 0) { buffCoins += buff.extraCoinsBoxes; }
                    else if(coins < 0) { buffCoins += buff.lessLoseInBoxes; }
                    
                }

                StartCoroutine(UIManager.instance.UpdateTextCoins(character, (coins + buffCoins)));
                StartCoroutine(character.DoAnim(
                    (coins > 0) ? ANIM_KEY_WIN_COINS : ANIM_KEY_LOSE_COINS,
                    (coins > 0) ? ANIM_NAME_WIN_COINS : ANIM_NAME_LOSE_COINS
                ));
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
                if (GetCurrentStarBox() != this)
                    break;

                Debug.Log("Character cayo en casilla estrella");
                UIManager.instance.SetActualCharacter(character);
                UIManager.instance.OpenStarShop(character, coins, this);

                if (!character.isPlayer)
                {
                    StartCoroutine(character.gameObject.GetComponent<NPC_Controller>().ProcessBuyStar(coins));
                }

                break; 
        }

        StartCoroutine(GameController.instance.FinishTurn());
        StartCoroutine(MoveCharacterAway(character));
        
    }

    private IEnumerator MoveCharacterAway(Character charac)
    {
        yield return new WaitForSeconds(2.15f);

        for(int i = 0; i < playerBoxesPos.Length; i++)
        {
            if (!playerBoxesPos[i].IsFilled)
            {
                playerBoxesPos[i].GoToPosition(charac.transform);
            }
        }
    }

    public void SetTrap(GameObject trapGO)
    {
        trapObject = trapGO;
        trapActiveTurns = 0;
        areTrapOnIt = true;
    }

    public void SetTower(GameObject towerGO)
    {
        towerObject = towerGO;
        towerActiveTurns = 0;
        IsTowerOnIt = true;
    }

    public bool IsTrapActive()
    {
        return areTrapOnIt;
    }

    public IEnumerator ActivateTrap(Character character)
    {
        // Hacer animación de jugador perjudicado por la trampa
        Debug.Log("Trampa activada");
        areTrapOnIt = false;
        trapObject.GetComponent<Animator>().SetBool("close", true);
        //character.DoAnim("trapAnim", "trap");

        yield return new WaitForSeconds(0.3f);

        Destroy(trapObject);
        StartCoroutine(GameController.instance.FinishTurn());
    }

    private void UpdateTurns()
    {
        if (IsTowerOnIt)
        {
            Debug.Log($"Tower active turns: {towerActiveTurns}");

            towerActiveTurns++;
            if(towerActiveTurns >= MaxTowerTurns)
            {
                Debug.Log("Destruimos torre");

                Destroy(towerObject); // No se destruye por no tener referencia a este
                IsTowerOnIt = false;
            }
        }

        if (areTrapOnIt)
        {
            trapActiveTurns++;
            if(trapActiveTurns >= MaxTrapTurns)
            {
                Destroy(trapObject);
                areTrapOnIt = false;
            }
        }
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

    public static void InitStarSystemNow()
    {
        Box[] all = GameObject.FindObjectsOfType<Box>();

        starBoxes.Clear();
        currentStarBox = null;
        starInitialized = true;  

        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].type == BoxType.Star)
            {
                starBoxes.Add(all[i]);
                all[i].SetBoxVisible(false);
            }
        }

        if (starBoxes.Count > 0)
        {
            currentStarBox = starBoxes[Random.Range(0, starBoxes.Count)];
            currentStarBox.SetBoxVisible(true);
        }
    }

    public static IEnumerator InitStarSystemNextFrame()
    {
        // Espera a que Unity haya activado todo
        yield return null;
        InitStarSystemNow();
    }

}
