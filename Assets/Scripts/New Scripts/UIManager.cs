using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Players Texts
    [SerializeField] private TMP_Text playerTextCoins;
    [SerializeField] private TMP_Text playerTextStars;
    [SerializeField] private Image playerImage;
    public Player actualPlayer;

    [Header("Cosas encrucijada")]
    private Board board;
    [SerializeField] private CanvasGroup leftArrow, rightArrow, forwardArrow, downArrow;

    [Header("Cosas comprar estrella")]
    [SerializeField] private TMP_Text textoPrecioEstrella;
    [SerializeField] private CanvasGroup UI_buyStar;
    private bool canBuyStar = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateSelectionPath(Box pathBox)
    {
        Vector3 pathTransf = pathBox.transform.position;

        for(int i = 0; i < (pathBox.PossiblesBoxesCount()); i++)
        {
            Vector3 possBoxTransf = pathBox.GetBoxTransf(i).position;
            Box possBox = pathBox.GetNewBox(i);

            float xDif = possBoxTransf.x - pathTransf.x;
            float zDif = possBoxTransf.z - pathTransf.z;

            float angle = CalculateAngle(pathTransf, possBoxTransf);

            bool arrowActivated = false;

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

    public void SetActualPlayer(Player player)
    {
        actualPlayer = player;
    }

    public IEnumerator UpdateTextCoins(int coins)
    {
        int actualCoins = actualPlayer.GetPlayerCoins();

        for(int i = 0; i < Mathf.Abs(coins); i++)
        {
            if (coins > 0)
            {
                actualCoins++;
            }
            else
            {
                if((actualCoins - 1) >= 0)
                {
                    actualCoins--;
                }
            }

            yield return new WaitForSeconds(0.25f);
            playerTextCoins.text = actualCoins.ToString("Coins: 0");
        }

        actualPlayer.SetPlayerCoins(actualCoins);
    }

    private void UpdateStarsText()
    {
        playerTextStars.text = actualPlayer.GetPlayerStars().ToString("Stars: 0");
    }

    private void UIStarShopControl(bool open)
    {
        UI_buyStar.alpha = open ? 1 : 0;
        UI_buyStar.blocksRaycasts = open;
    }

    /// <summary>
    /// Abre el panel de control para comprar la estrella. Pide de valor el costo en monedas
    /// </summary>
    /// <param name="precio">Precio de la estrella</param>
    public void OpenStarShop(int precio)
    {
        UIStarShopControl(true);
        textoPrecioEstrella.text = precio.ToString("Precio: 0 coins");
        if(actualPlayer.GetPlayerCoins() >= precio)
        {
            canBuyStar = true;
        }
    }

    public void BuyStar()
    {
        if(!canBuyStar)
        {
            Debug.Log("El jugador no tiene las suficientes monedas para comprar la estrella");
            NotBuyStar();
        }
        else
        {
            actualPlayer.GetStar();
            UIStarShopControl(false);
            StartCoroutine(UpdateTextCoins(-5));
            UpdateStarsText();
            canBuyStar = false;
            Debug.Log("El jugador compro una estrella");
        }
    }
    
    public void NotBuyStar()
    {
        UIStarShopControl(false);
    }
}
