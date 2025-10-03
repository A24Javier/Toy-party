using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TMP_Text playerTextCoins;

    [Header("Cosas encrucijada")]
    private Board board;
    [SerializeField] private CanvasGroup leftArrow, rightArrow, forwardArrow, downArrow;

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

    public void CreateSelectionPath(Box pathBox, Player player)
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
                ActivateArrow(rightArrow, angle, possBoxTransf, player, possBox);
                arrowActivated = true;
            }
            else if (xDif < 0 && Mathf.Abs(zDif) < Mathf.Abs(xDif) && leftArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(leftArrow, angle, possBoxTransf, player, possBox);
                arrowActivated = true;
            }

            if (zDif > 0 && Mathf.Abs(zDif) > Mathf.Abs(xDif) && forwardArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(forwardArrow, angle, possBoxTransf, player, possBox);
                arrowActivated = true;
            }
            else if (zDif < 0 && Mathf.Abs(zDif) > Mathf.Abs(xDif) && downArrow.alpha == 0 && !arrowActivated)
            {
                ActivateArrow(downArrow, angle, possBoxTransf, player, possBox);
                arrowActivated = true;
            }
        }
    }

    private void ActivateArrow(CanvasGroup arrow, float angle, Vector3 boxTransf, Player player, Box box)
    {
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);
        arrow.GetComponent<ArrowInfo>().ArrangeVector(boxTransf);
        arrow.GetComponent<ArrowInfo>().SetPlayer(player);
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

    public IEnumerator UpdateTextCoins(int coins, Player player)
    {
        int actualCoins = player.GetPlayerCoins();

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

        player.SetPlayerCoins(actualCoins);
    }
}
