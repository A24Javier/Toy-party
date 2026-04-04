using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Elementos de la UI
    [Header("Elementos de la UI")]
    [SerializeField] private LocalizedString coinsLocal;
    [SerializeField] private TMP_Text characterTextCoins;

    [SerializeField] private LocalizedString starsLocal;
    [SerializeField] private TMP_Text characterTextStars;
    [SerializeField] private Image characterImage;

    [Header("Elementos Inventario")]
    [SerializeField] private CanvasGroup actionPanel;
    [SerializeField] private CanvasGroup itemsPanel;
    [SerializeField] private Image actualDiceImage;
    [SerializeField] private Button[] itemsButtons;
    [SerializeField] private Sprite nullObjSpr;

    // Jugador y personaje actuales
    public Player actualPlayer;
    public Character actualCharacter;

    // Elementos UI para selección de camino
    [Header("Cosas encrucijada")]
    private Board board;
    [SerializeField] private CanvasGroup leftArrow, rightArrow, forwardArrow, downArrow;

    // Elementos UI para la compra de estrella
    [Header("Cosas comprar estrella")]
    [SerializeField] private TMP_Text textoPrecioEstrella;
    [SerializeField] private CanvasGroup UI_buyStar;
    private bool canBuyStar = false;
    private bool isStarCoupon = false;
    private int couponInvIndex = -1;

    private int currentStarPrice = 0;
    private Box currentStarBox = null;

    // Elementos UI para selección de minijuego
    [Header("Cosas selección de minijuego")]
    [SerializeField] private GameObject prefabMinigameObjList;
    [SerializeField] private CanvasGroup panelMinigameSelection;
    [SerializeField] private Transform minigameVerticalLayout;
    private const int MAX_MINIGAMES_SELECTION = 5;
    private Color unselectedColor = new Color(1, 1, 1, 0.5f);
    private Color selectedColor = new Color(1, 1, 1, 1);

    [Header("Cosas leaderboard")]
    [SerializeField] private CanvasGroup leaderboardGroup;
    [SerializeField] private Image[] characterImages;
    [SerializeField] private TMP_Text[] characterLeadPos;

    [Header("Cosas Seleccionar jugador")]
    [SerializeField] private CanvasGroup selectPlayerGroup;
    [SerializeField] private Image[] selectPlayerImgs;
    [SerializeField] private Button[] selectPlayerBtns;

    [Header("Elementos mensaje cupón estelar")]
    [SerializeField] private CanvasGroup starCouponMsgGroup;

    [Header("Elementos para habilidad personaje")]
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image abilityBackground;

    // Elementos UI para Fade in/out
    [SerializeField] private CanvasGroup panelFadeInOut;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        panelMinigameSelection.alpha = 0;
        ControlActionPanel(false);
        ControlSelectPlayer(false);
        ControlStarCouponMsg(false);
        itemsPanel.alpha = 0;
        HideLeaderboard();
    }

    #region Selección de Camino
    public void CreateSelectionPath(Box pathBox)
    {
        Vector3 pathTransf = pathBox.transform.position;

        for (int i = 0; i < pathBox.PossiblesBoxesCount(); i++)
        {
            Vector3 possBoxTransf = pathBox.GetBoxTransf(i).position;
            Box possBox = pathBox.GetNewBox(i);

            if (possBox.IsTowerOnIt) { continue; }

            float angle = CalculateAngle(pathTransf, possBoxTransf);
            angle += Camera.main.transform.eulerAngles.y;

            bool arrowActivated = false;

            // Verificar dirección y activar la flecha correspondiente
            if ((angle <= 45f && angle > -45f || angle >= 360f && angle > 45f) && rightArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(rightArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
            else if ((angle > 135f && angle < 225f) && leftArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(leftArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }

            if ((angle < 135f && angle > 45f) && forwardArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(forwardArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
            else if ((angle < 315f && angle > 225f || angle < -45f && angle > -135f) && downArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(downArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
        }

        ControlActionPanel(false);
    }

    private void ActivateArrow(CanvasGroup arrow, float angle, Vector3 boxTransf, Box box)
    {
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);
        arrow.GetComponent<ArrowInfo>().ArrangeVector(boxTransf);
        arrow.GetComponent<ArrowInfo>().SetPlayer(actualPlayer);
        arrow.GetComponent<ArrowInfo>().SetBox(box);

        arrow.alpha = 1.0f;
        arrow.interactable = true;
    }

    private void DeactivateArrow(CanvasGroup arrow)
    {
        arrow.alpha = 0.0f;
        arrow.interactable = false;
    }

    private float CalculateAngle(Vector3 P1, Vector3 P2)
    {
        Vector3 direction = (P2 - P1);
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        return angle;
    }

    public void DeactivatePathDecision()
    {
        DeactivateArrow(forwardArrow);
        DeactivateArrow(downArrow);
        DeactivateArrow(leftArrow);
        DeactivateArrow(rightArrow);
    }
    #endregion

    #region Gestión del Jugador y Personaje
    public void SetActualPlayer(Player player)
    {
        actualPlayer = player;
    }

    public void SetActualCharacter(Character character)
    {
        actualCharacter = character;
    }

    public void ChangeCharacterUI(Character character)
    {
        string coinsTraduction = coinsLocal.GetLocalizedString();
        characterTextCoins.text = string.Concat(coinsTraduction, character.GetCoins());

        string starsTraduction = starsLocal.GetLocalizedString();
        characterTextStars.text = string.Concat(starsTraduction, character.GetStars());

        characterImage.sprite = character.GetCharImage();
    }

    public void FunctionUpdateTextCoins(Character character, int coins)
    {
        StartCoroutine(UpdateTextCoins(character, coins));
    }

    public IEnumerator UpdateTextCoins(Character character, int coins)
    {
        ChangeCharacterUI(character);
        int actualCoins = character.GetCoins();

        for (int i = 0; i < Mathf.Abs(coins); i++)
        {
            if (coins > 0)
            {
                actualCoins++;
            }
            else
            {
                if ((actualCoins - 1) >= 0)
                {
                    actualCoins--;
                }
            }

            yield return new WaitForSeconds(0.25f);
            characterTextCoins.text = actualCoins.ToString("Coins: 0");
        }

        character.SetCoins(actualCoins);
    }

    private void UpdateStarsText()
    {
        characterTextStars.text = actualCharacter.GetStars().ToString("Stars: 0");
    }
    #endregion

    private void UIStarShopControl(bool open)
    {
        UI_buyStar.alpha = open ? 1 : 0;
        UI_buyStar.blocksRaycasts = open;
        UI_buyStar.interactable = actualCharacter.isPlayer;
    }

    public void OpenStarShop(Character character, int precio, Box starBox)
    {
        actualCharacter = character;
        currentStarPrice = Mathf.Abs(precio);
        currentStarBox = starBox;

        ChangeCharacterUI(character);
        UIStarShopControl(true);
        
        for(int i = 0; i < character.GetInventory().GetTotalObjLoaded(); i++)
        {
            if(character.GetInventory().GetItem(i).ItemName == "Star Coupon")
            {
                textoPrecioEstrella.text = precio.ToString("Price: Star Coupon");
                canBuyStar = true;
                isStarCoupon = true;
                couponInvIndex = i;
                break;
            }
        }

        if (!canBuyStar && !isStarCoupon)
        {
            textoPrecioEstrella.text = precio.ToString($"Price: {currentStarPrice} coins");
            canBuyStar = true;
        }

        canBuyStar = (character.GetCoins() >= currentStarPrice);
    }

    public void BuyStar()
    {
        if (actualCharacter.GetCoins() < currentStarPrice && !isStarCoupon)
        {
            Debug.Log("El personaje no tiene las suficientes monedas para comprar la estrella");
            NotBuyStar();
            return;
        }
        else
        {
            UIStarShopControl(false);

            if (isStarCoupon)
            {
                // Quitamos el cupón del inventario del jugador
                StarCoupon coupon = actualCharacter.GetInventory().GetItem(couponInvIndex).itemFunction as StarCoupon;
                coupon.DestroyItem();

                isStarCoupon = false;
            }
            else
            {
                StartCoroutine(UpdateTextCoins(actualCharacter, -currentStarPrice));
            }

            actualCharacter.SetStars(actualCharacter.GetStars() + 1);

            UpdateStarsText();

            canBuyStar = false;
            Debug.Log("El personaje compró una estrella");
            Box.MoveStarToRandom(currentStarBox);
            ContinueAfterStarShopOrFinishTurn();
        }
    }

    public void NotBuyStar()
    {
        canBuyStar = false;
        UIStarShopControl(false);

        ContinueAfterStarShopOrFinishTurn();
    }

    private void ContinueAfterStarShopOrFinishTurn()
    {
        if (actualCharacter != null && actualCharacter.waitingStarShop && actualCharacter.pendingStepsAfterShop > 0)
        {
            int remaining = actualCharacter.pendingStepsAfterShop;

            actualCharacter.waitingStarShop = false;
            actualCharacter.pendingStepsAfterShop = 0;

            actualCharacter.Move(remaining);
        }
        else
        {
            StartCoroutine(GameController.instance.FinishTurn());
        }
    }



    public IEnumerator FadeInOut(bool fadeIn, Action onFadeComplete)
    {
        float elapsedTime = 0;
        panelFadeInOut.alpha = fadeIn ? 0 : 1;
        panelFadeInOut.blocksRaycasts = true;

        float initialValue = panelFadeInOut.alpha;
        float finalFadeValue = fadeIn ? 1 : 0;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            panelFadeInOut.alpha = Mathf.Lerp(initialValue, finalFadeValue, elapsedTime / 1);
            yield return null;
        }

        panelFadeInOut.alpha = finalFadeValue;

        if (onFadeComplete != null)
        {
            onFadeComplete();
        }
    }

    // Método vacío para la selección de minijuegos (por implementar más tarde)
    public void ShowPossibleMinigamesList(List<string> possibleMinigames)
    {
        int[] selectedMinigames = new int[MAX_MINIGAMES_SELECTION];
        panelMinigameSelection.alpha = 1;

        // Quitar minijuegos hasta quedarnos solo con 5 seleccionados aleatoriamente
        if (possibleMinigames.Count > MAX_MINIGAMES_SELECTION)
        {
            Debug.Log("Entra en quitar minijuegos");
            for (int i = 0; i < MAX_MINIGAMES_SELECTION; i++)
            {
                selectedMinigames[i] = UnityEngine.Random.Range(0, possibleMinigames.Count);
            }

            for (int i = 0; i < possibleMinigames.Count; i++)
            {
                for (int j = 0; j < selectedMinigames.Length; j++)
                {
                    if (selectedMinigames[j] != i)
                    {
                        possibleMinigames.RemoveAt(i);
                    }
                }
            }
        }

        Image[] images = new Image[possibleMinigames.Count];
        // Creamos la lista de posibles minijuegos
        for (int i = 0; i < possibleMinigames.Count; i++)
        {
            GameObject go = Instantiate(prefabMinigameObjList, Vector3.zero, Quaternion.identity, minigameVerticalLayout.transform);
            go.GetComponentInChildren<TMP_Text>().text = possibleMinigames[i];
            images[i] = go.GetComponent<Image>();
        }

        int end = UnityEngine.Random.Range(0, possibleMinigames.Count);
        MinigameController.instance.SetMinigameToLoad(possibleMinigames[end]);

        StartCoroutine(MinigameRoulette(end, images));
    }

    private IEnumerator MinigameRoulette(int end, Image[] images)
    {
        int maxRounds = UnityEngine.Random.Range(15, 25);

        for (int i = 0; i < maxRounds; i++)
        {
            for (int j = 0; j < images.Length; j++)
            {
                if ((float)i % images.Length == 0)
                {
                    images[j].color = selectedColor;
                }
                else
                {
                    images[j].color = unselectedColor;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < images.Length; i++)
        {
            if (i != end)
            {
                images[i].color = unselectedColor;
            }
        }

        images[end].color = selectedColor;
        RectTransform selectedImage = images[end].GetComponent<RectTransform>();
        selectedImage.sizeDelta = new Vector2(selectedImage.sizeDelta.x * 1.25f, selectedImage.sizeDelta.y * 1.25f);
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeInOut(true, MinigameController.instance.LoadMinigame));
    }

    public void AddItem(Item newItem)
    {
        for (int i = 0; i < itemsButtons.Length; i++)
        {
            if (itemsButtons[i].image.sprite == nullObjSpr)
            {
                Debug.Log($"Button events: {itemsButtons[i].onClick.GetPersistentEventCount()}, newItem name: {newItem.ItemName}, i = {i} ");
                itemsButtons[i].image.sprite = newItem.itemSpr;
                itemsButtons[i].onClick.AddListener(delegate { 
                    newItem.itemFunction.UseItem();
                    if (newItem.ItemName != "Star Coupon" && newItem.ItemName != "Cupón estrella" && newItem.ItemName != "Cupó estrella")
                    {
                        DeleteItem(i);
                    }
                });
                break;
            }
        }
    }

    private void DeleteItem(int buttonIndex)
    {
        itemsButtons[buttonIndex].onClick.RemoveAllListeners();
        itemsButtons[buttonIndex].image.sprite = null;
    }

    public void ControlActionPanel(bool open)
    {
        actionPanel.alpha = open ? 1 : 0;
        actionPanel.interactable = open;
        actionPanel.blocksRaycasts = open;

        if (open)
        {
            LoadInventory();
            ChangeAbilityUI();
        }
    }

    public void ControlItemPanel()
    {
        if (itemsPanel.alpha >= 1f)
        {
            itemsPanel.alpha = 0f;
        }
        else
        {
            itemsPanel.alpha = 1f;
        }

        itemsPanel.interactable = !itemsPanel.interactable;
        itemsPanel.blocksRaycasts = !itemsPanel.blocksRaycasts;
    }

    public void ControlStarCouponMsg(bool open)
    {
        starCouponMsgGroup.alpha = open ? 1 : 0;
        starCouponMsgGroup.interactable = open;
        starCouponMsgGroup.blocksRaycasts = open;
    }

    public void LoadInventory()
    {
        Inventory characterInventory = actualCharacter.GetInventory();
        Debug.Log(characterInventory.GetTotalObjLoaded());

        for (int i = 0; i < itemsButtons.Length; i++)
        {
            itemsButtons[i].onClick.RemoveAllListeners();
            itemsButtons[i].image.sprite = nullObjSpr;
        }

        // Cargar objetos
        for (int i = 0; i < characterInventory.GetTotalObjLoaded(); i++)
        {
            Debug.Log($"Item cargado: {characterInventory.GetItem(i).name}");
            AddItem(characterInventory.GetItem(i));
        }
    }

    private void ChangeAbilityUI()
    {
        if(actualCharacter.ability.AbilityFunction != null)
        {
            abilityButton.onClick.RemoveAllListeners();
            abilityButton.onClick.AddListener(actualCharacter.ability.AbilityFunction.UseAbility);

            abilityImage.sprite = actualCharacter.ability.AbilitySprite;
            abilityBackground.color = actualCharacter.ability.BackgroundColor;

        }
    }

    public void ShowLeaderboard(Character[] chars)
    {
        leaderboardGroup.alpha = 1f;
        leaderboardGroup.interactable = true;
        leaderboardGroup.blocksRaycasts = true;

        // Ordenar: primero por estrellas (descendiente), luego por monedas (descendiente)
        Character[] authPos = chars.OrderByDescending(c => c.stars).ThenByDescending(c => c.coins).ToArray();

        int[] playerRanks = new int[authPos.Length];

        int currentRank = 1;
        playerRanks[0] = currentRank;

        // Calcular posiciones con empates
        for (int i = 1; i < authPos.Length; i++)
        {
            if (authPos[i].stars == authPos[i - 1].stars && authPos[i].coins == authPos[i - 1].coins)
            {
                playerRanks[i] = currentRank;
            }
            else
            {
                currentRank = i + 1;
                playerRanks[i] = currentRank;
            }
        }

        // Setear los textos e imágenes
        for (int i = 0; i < authPos.Length; i++)
        {
            characterImages[i].sprite = authPos[i].characterImage;
            characterLeadPos[i].text = $"#{playerRanks[i]}";
        }
    }

    private void HideLeaderboard()
    {
        leaderboardGroup.alpha = 0f;
        leaderboardGroup.interactable = false;
        leaderboardGroup.blocksRaycasts = false;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ControlSelectPlayer(bool show)
    {
        selectPlayerGroup.alpha = show ? 1f : 0f;
        selectPlayerGroup.interactable = selectPlayerGroup.blocksRaycasts = show;
    }

    public void ConfigureSelectPlayer(Character charItem, string functionName, int modifierValue = 0)
    {
        ControlSelectPlayer(true);
        ControlActionPanel(false);

        Character[] chars = new Character[3];
        int addedChars = 0;
        
        // Obtenemos los characters
        for(int i = 0; i < GameController.instance.GetCharactersInParty(); i++)
        {
            if(charItem.characterId != GameController.instance.GetCharacter(i).characterId)
            {
                chars[addedChars] = GameController.instance.GetCharacter(i);
                addedChars++;
            }
        }

        // Ponemos las imagenes
        for(int i = 0; i < selectPlayerImgs.Length; i++)
        {
            selectPlayerImgs[i].sprite = chars[i].GetCharImage();
        }

        // Nos aseguramos que los botones no tengan listeners de antes
        for(int i = 0; i < selectPlayerBtns.Length; i++)
        {
            selectPlayerBtns[i].onClick.RemoveAllListeners();
        }

        // Ponemos los nombres y la función
        for(int i = 0; i < selectPlayerBtns.Length; i++)
        {
            int index = i;
            selectPlayerBtns[index].GetComponentInChildren<TMP_Text>().SetText("Select " + chars[index].name);
            selectPlayerBtns[index].onClick.AddListener(delegate { ControlSelectPlayer(false); });
            if (charItem.isPlayer) { selectPlayerBtns[index].onClick.AddListener(delegate { ControlActionPanel(true); }); }

            switch (functionName)
            {
                case "AbstractMovement":
                    selectPlayerBtns[index].onClick.AddListener(delegate { chars[index].SetExtraStep(modifierValue); });
                    break;
                case "StealCoins":
                    // Objective coins
                    int objCoins = chars[index].GetCoins() - modifierValue;
                    objCoins = Mathf.Max(objCoins, 0);

                    // Coins difference
                    int diffCoins = 0;

                    selectPlayerBtns[index].onClick.AddListener(delegate { chars[index].SetCoins(objCoins); UpdateTextCoins(charItem, diffCoins); });
                    break;
                case "TP_OtherPlayer":
                    Box objBox = chars[index].GetActualBox();

                    selectPlayerBtns[index].onClick.AddListener(delegate{
                        charItem.actualBox = objBox;
                        charItem.actualBox.ActivateEffect(charItem);
                        charItem.transform.position = objBox.transform.position;
                    });

                    break;

                case "Bomb":
                    Box absBox = chars[index].GetActualBox();

                    for(int j = 0; j < modifierValue; j++)
                    {
                        absBox = absBox.LastBox;
                    }

                    selectPlayerBtns[index].onClick.AddListener(delegate { chars[index].actualBox = absBox; chars[index].transform.position = absBox.transform.position; });

                    break;
            }
        }
    }

    public void AddTenCoinsToActualChar()
    {
        StartCoroutine(UpdateTextCoins(actualCharacter, 10));
    }

    public void ChangeDiceSprite(Sprite diceSprite)
    {
        actualDiceImage.sprite = diceSprite;
    }
}
