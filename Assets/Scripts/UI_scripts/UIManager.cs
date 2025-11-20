using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using System;
using System.IO;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Elementos de la UI
    [Header("Elementos de la UI")]
    [SerializeField] private TMP_Text characterTextCoins;
    [SerializeField] private TMP_Text characterTextStars;
    [SerializeField] private Image characterImage;

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

    // Elementos UI para selección de minijuego
    [Header("Cosas selección de minijuego")]
    [SerializeField] private GameObject prefabMinigameObjList;
    [SerializeField] private CanvasGroup panelMinigameSelection;
    [SerializeField] private Transform minigameVerticalLayout;
    private const int MAX_MINIGAMES_SELECTION = 5;
    private Color unselectedColor = new Color(1, 1, 1, 0.5f);
    private Color selectedColor = new Color(1, 1, 1, 1);

    // Elementos UI para Fade in/out
    [SerializeField] private CanvasGroup panelFadeInOut;

    // Patrón Singleton (asegura que solo haya una instancia)
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
    }

    #region Selección de Camino
    public void CreateSelectionPath(Box pathBox)
    {
        Vector3 pathTransf = pathBox.transform.position;

        for (int i = 0; i < pathBox.PossiblesBoxesCount(); i++)
        {
            Vector3 possBoxTransf = pathBox.GetBoxTransf(i).position;
            Box possBox = pathBox.GetNewBox(i);

            float xDif = possBoxTransf.x - pathTransf.x;
            float zDif = possBoxTransf.z - pathTransf.z;

            float angle = CalculateAngle(pathTransf, possBoxTransf);

            bool arrowActivated = false;

            // Verificar dirección y activar la flecha correspondiente
            if (xDif > 0 && Mathf.Abs(zDif) < Mathf.Abs(xDif) && rightArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(rightArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
            else if (xDif < 0 && Mathf.Abs(zDif) < Mathf.Abs(xDif) && leftArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(leftArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }

            if (zDif > 0 && Mathf.Abs(zDif) > Mathf.Abs(xDif) && forwardArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(forwardArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
            else if (zDif < 0 && Mathf.Abs(zDif) > Mathf.Abs(xDif) && downArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(downArrow, angle, possBoxTransf, possBox);
                arrowActivated = true;
            }
        }
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
        characterTextCoins.text = character.GetCoins().ToString("Coins: 0");
        characterTextStars.text = character.GetStars().ToString("Stars: 0");
        characterImage.sprite = character.GetCharImage();
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
        characterTextStars.text = actualPlayer.GetStars().ToString("Stars: 0");
    }
    #endregion

    #region Gestión de la Tienda de Estrellas
    private void UIStarShopControl(bool open)
    {
        UI_buyStar.alpha = open ? 1 : 0;
        UI_buyStar.blocksRaycasts = open;
        UI_buyStar.interactable = actualCharacter.isPlayer;
    }

    public void OpenStarShop(Character character, int precio)
    {
        actualCharacter = character;
        ChangeCharacterUI(character);
        UIStarShopControl(true);
        textoPrecioEstrella.text = precio.ToString("Precio: 0 coins");
        if (character.GetCoins() >= precio)
        {
            canBuyStar = true;
        }
    }

    public void BuyStar()
    {
        if (!canBuyStar)
        {
            Debug.Log("El personaje no tiene las suficientes monedas para comprar la estrella");
            NotBuyStar();
        }
        else
        {
            actualCharacter.GetStars();
            UIStarShopControl(false);
            StartCoroutine(UpdateTextCoins(actualCharacter, -5));
            UpdateStarsText();
            canBuyStar = false;
            Debug.Log("El personaje compró una estrella");
        }
    }

    public void NotBuyStar()
    {
        UIStarShopControl(false);
    }
    #endregion

    #region Efectos de Fade
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
    #endregion

    // Método vacío para la selección de minijuegos (por implementar más tarde)
    public void ShowPossibleMinigamesList(List<string> possibleMinigames)
    {
        panelMinigameSelection.alpha = 1;
        int[] selectedMinigames = new int[5];

        // Quitar minijuegos hasta quedarnos solo con 5 seleccionados aleatoriamente
        if(possibleMinigames.Count > MAX_MINIGAMES_SELECTION)
        {
            for(int i = 0; i < MAX_MINIGAMES_SELECTION; i++)
            {
                selectedMinigames[i] = UnityEngine.Random.Range(0, possibleMinigames.Count);
            }

            for(int i = 0; i < possibleMinigames.Count; i++)
            {
                for(int j = 0; j < selectedMinigames.Length; j++)
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
        for(int i = 0; i < possibleMinigames.Count; i++)
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

        for(int i = 0; i < maxRounds; i++)
        {
            for(int j = 0; j < images.Length; i++)
            {
                if((float)i % images.Length == 0)
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

        for(int i = 0; i < images.Length; i++)
        {
            if(i != end)
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
}
