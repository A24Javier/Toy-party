using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _shopGroup;

    [SerializeField] private GameObject _shopItemUIPrefab;
    [SerializeField] private Transform _itemButtonsParent;

    [SerializeField] private Item[] _shopItems;
    private List<Button> _shopButtons = new List<Button>();
    [SerializeField] private int _extraItemPrice = 0;

    [SerializeField] private Button _closeButton;
    public UnityEvent OnCloseShop;
    public static ShopManager Instance { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CloseShop();
    }

    public void OpenShop(bool isNPC)
    {
        // Hacemos visible la tienda
        _shopGroup.alpha = isNPC ? 1f : 0f;
        _shopGroup.interactable = !isNPC;
        _shopGroup.blocksRaycasts = !isNPC;

        _closeButton.interactable = !isNPC;

        // Creamos la lista de objetos disponibles
        _shopButtons = CreateShopList();
    }

    private List<Button> CreateShopList(int itemsToCreate = 3)
    {
        List<Button> list = new List<Button>();

        for(int i = 0; i <  itemsToCreate; i++)
        {
            Button itemButton = Instantiate(_shopItemUIPrefab, Vector3.zero, Quaternion.identity, _itemButtonsParent).GetComponent<Button>();

            int randItem = Random.Range(0, _shopItems.Length);
            Item item = _shopItems[randItem];

            itemButton.onClick.AddListener(delegate {BuyItem(item, itemButton); });

            itemButton.transform.GetChild(1).GetComponent<TMP_Text>().SetText(item.itemPrice.ToString("0"));
            itemButton.transform.GetChild(2).GetComponent<Image>().sprite = item.itemSpr;
            itemButton.transform.GetChild(3).GetComponent<TMP_Text>().SetText(item.ItemName);

            list.Add(itemButton);
        }

        return list;
    }

    private IEnumerator NPC_Shopping()
    {
        Character actualChar = GameController.instance.GetCharacterOfTurn();
        int charCoins = actualChar.GetCoins();

        float buyChance = Mathf.Clamp01(charCoins / 25f);

        // Si el inventario del NPC esta lleno cerrará la tienda
        if (actualChar.GetInventory().GetTotalObjLoaded() >= actualChar.GetInventory().GetMaxObjects())
        {
            yield return new WaitForSeconds(1f);
            CloseShop();
        }

        for(int i = 0; i < _shopButtons.Count; i++)
        {
            int itemPrice = int.Parse(_shopButtons[i].transform.GetChild(1).GetComponent<TMP_Text>().text);

            if (itemPrice < charCoins && Random.value < buyChance)
            {
                charCoins -= itemPrice;
                buyChance = Mathf.Clamp01(charCoins / 25f);
                _shopButtons[i].onClick?.Invoke();
            }

            yield return new WaitForSeconds(0.5f);

        }

        yield return new WaitForSeconds(1f);
    }

    private void BuyItem(Item item, Button itemButton)
    {
        int finalPrice = (item.itemPrice + _extraItemPrice);
        Character actualChar = GameController.instance.GetCharacterOfTurn();

        if(actualChar.GetCoins() >= finalPrice && !actualChar.GetInventory().IsFull())
        {
            StartCoroutine(UIManager.instance.UpdateTextCoins(actualChar, -finalPrice));
            actualChar.GetInventory().AddItem(item);
            UIManager.instance.LoadInventory();
            itemButton.interactable = false;
        }
    }

    public void CloseShop()
    {
        // Hacemos invisible la tienda
        _shopGroup.alpha = 0;
        _shopGroup.interactable = false;
        _shopGroup.blocksRaycasts = false;

        // Quitamos los objetos de la tienda
        if(_shopButtons.Count > 0)
        {
            for (int i = (_shopButtons.Count - 1); i >= 0; i--)
            {
                Destroy(_shopButtons[i].gameObject);
            }

            _shopButtons.Clear();
        }

        OnCloseShop?.Invoke();

        OnCloseShop.RemoveAllListeners();

    }
}
