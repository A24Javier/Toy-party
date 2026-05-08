using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BoxType
{
    Normal,
    Coin,
    Event,
    Path,
    Star,
    Shop,
    MinigameOneVSOne
}

[System.Serializable]
public class PlayerOutBoxPos
{
    [HideInInspector] public bool IsFilled = false;
    public Transform TransfPos = null;

    public void GoToPosition(Character character)
    {
        IsFilled = true;
        Vector3 pos = new Vector3(TransfPos.position.x, character.transform.position.y, TransfPos.position.z);
        character.transform.DOMove(pos, 1f);

        UnityAction ua = null;

        ua = () =>
        {
            IsFilled = false;
            Transform parentTransf = TransfPos.parent.gameObject.transform;
            character.transform.position = new Vector3(parentTransf.position.x, character.transform.position.y, parentTransf.position.z);
            character.OnStartMove.RemoveListener(ua);
        };

        character.OnStartMove.AddListener(ua);
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
    private int coins = 3;
    [SerializeField] private int blueBoxCoins = 3;
    [SerializeField] private int starCoins = 10;
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

    [SerializeField] private GameObject prefabCoins;
    [SerializeField] private GameObject prefabParticleLose;

    private const string ANIM_KEY_WIN_COINS = "isCelebrating";
    private const string ANIM_KEY_LOSE_COINS = "isMourning";

    private const string ANIM_NAME_WIN_COINS = "Victory";
    private const string ANIM_NAME_LOSE_COINS = "Defeat";

    [Header("Star System")]
    [SerializeField] private Renderer[] boxRenderers;
    [SerializeField] private Material blueBoxMat;
    [SerializeField] private Material starBoxMat;

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

    /*
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
    */

    private void SetBoxAsStarBox()
    {
        type = BoxType.Star;
        transform.GetChild(5).GetComponent<MeshRenderer>().material = starBoxMat;
        coins = starCoins;
    }

    private void SetBoxAsBlueBox()
    {
        type = BoxType.Coin;
        transform.GetChild(5).GetComponent<MeshRenderer>().material = blueBoxMat;
        coins = blueBoxCoins;
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
            {
                //starBoxes[i].SetBoxVisible(false);
                starBoxes[i].SetBoxAsBlueBox();
            }
                
        }

        currentStarBox = box;

        if (currentStarBox != null)
        {
            //currentStarBox.SetBoxVisible(true);
            currentStarBox.SetBoxAsStarBox();
        }
            
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
        //CacheRenderers();

        if (type == BoxType.Star)
        {
            if (!starBoxes.Contains(this))
                starBoxes.Add(this);

            if (!starInitialized)
            {
                //SetBoxVisible(false);
                SetBoxAsBlueBox();
            }
            else
            {
                //SetBoxVisible(currentStarBox == this);
                if (currentStarBox == this)
                    SetBoxAsStarBox();
                else
                    SetBoxAsBlueBox();
            }

            coins = starCoins;
        }
        else if(type == BoxType.Coin)
        {
            coins = blueBoxCoins;
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
                {
                    //starBoxes[i].SetBoxVisible(false);
                    starBoxes[i].SetBoxAsBlueBox();
                }
                    
            }

            SetStarBox(starBoxes[Random.Range(0, starBoxes.Count)]);
        }

        GameController.instance.OnRoundEnded.AddListener(UpdateTurns);
    }

    public void ActivateEffect(Character character)
    {
        bool autoFinishTurn = true;

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

                if (coins > 0)
                    CreateCoins();
                else if (coins < 0)
                    CharLoseCoins();

                StartCoroutine(UIManager.instance.UpdateTextCoins(character, (coins + buffCoins)));
                StartCoroutine(character.DoAnim(
                    (coins > 0) ? ANIM_KEY_WIN_COINS : ANIM_KEY_LOSE_COINS,
                    (coins > 0) ? ANIM_NAME_WIN_COINS : ANIM_NAME_LOSE_COINS
                ));
                break;

            case BoxType.Event:
                Debug.Log("Character cayo en casilla de evento");
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

                autoFinishTurn = false;

                break;

            case BoxType.Shop:
                Debug.Log("Character cayo en casilla tienda");

                UnityAction ua = null;

                ua = () =>
                {
                    if(character.pendingStepsAfterShop > 0)
                        character.Move(character.pendingStepsAfterShop);
                    else
                    {
                        StartCoroutine(GameController.instance.FinishTurn());
                        StartCoroutine(MoveCharacterAway(character));
                    }

                    //StartCoroutine(GameController.instance.FinishTurn());
                    //StartCoroutine(MoveCharacterAway(character));

                    ShopManager.Instance.OnCloseShop.RemoveListener(ua);
                };

                ShopManager.Instance.OnCloseShop.AddListener(ua);
                ShopManager.Instance.OpenShop(!character.isPlayer);

                autoFinishTurn = false;

                break;

            case BoxType.MinigameOneVSOne:
                UIManager.instance.MinigameOneVSOneBoxFunction();
                autoFinishTurn = false;

                break;
        }

        if (!autoFinishTurn)
            return;

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
                playerBoxesPos[i].GoToPosition(charac);
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
        trapObject.GetComponentInChildren<Animator>().SetBool("close", true);
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
                //all[i].SetBoxVisible(false);
                all[i].SetBoxAsBlueBox();
            }
        }

        if (starBoxes.Count > 0)
        {
            currentStarBox = starBoxes[Random.Range(0, starBoxes.Count)];
            //currentStarBox.SetBoxVisible(true);
            currentStarBox.SetBoxAsStarBox();
        }
    }

    public static IEnumerator InitStarSystemNextFrame()
    {
        // Espera a que Unity haya activado todo
        yield return null;
        InitStarSystemNow();
    }


    private void CreateCoins()
    {
        Vector3 initialPos = gameObject.transform.position;
        initialPos.y += 0.5f;

        for(int i = 0; i < blueBoxCoins; i++)
        {
            Instantiate(prefabCoins, initialPos, Quaternion.Euler(prefabCoins.transform.localRotation.eulerAngles.x, camRotationY, gameObject.transform.localRotation.eulerAngles.z));
            initialPos.y += 0.1f;
        }
    }

    private void CharLoseCoins()
    {
        ParticleSystem particles = Instantiate(prefabParticleLose, Vector3.zero, Quaternion.identity, transform).GetComponent<ParticleSystem>();
        particles.transform.localPosition = new Vector3(0f, 5f, 0f);
    }
}
