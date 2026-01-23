using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _shopGroup;

    [SerializeField] private GameObject _shopItemUIPrefab;
    [SerializeField] private Item[] _shopItems;
    private List<Button> _shopButtons;
    [SerializeField] private int _extraItemPrice = 0;

    public void OpenShop()
    {
        // Hacemos visible la tienda
        _shopGroup.alpha = 1;
        _shopGroup.interactable = true;
        _shopGroup.blocksRaycasts = true;

        // Creamos la lista de objetos disponibles
        _shopButtons = CreateShopList();
    }

    private List<Button> CreateShopList(int itemsToCreate = 3)
    {
        List<Button> list = new List<Button>();

        for(int i = 0; i <  itemsToCreate; i++)
        {

        }

        return list;
    }

    public void CloseShop()
    {
        // Hacemos invisible la tienda
        _shopGroup.alpha = 0;
        _shopGroup.interactable = false;
        _shopGroup.blocksRaycasts = false;

        // Quitamos los objetos de la tienda
    }
}
