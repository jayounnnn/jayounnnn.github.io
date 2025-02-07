using System.Collections.Generic;
using PlayFab;
using UnityEngine.UI;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI currencyDisplay;
    [SerializeField] TextMeshProUGUI Msg;

    public GameObject itemPrefab;  
    public Transform shopPanel;

    void Start()
    {
        GetCatalog();
        GetVirtualCurrencies();
    }


    void UpdateMsg(string msg)
    {
        Debug.Log(msg);
        Msg.text += msg + '\n';
    }

    void OnError(PlayFabError e)
    {
        UpdateMsg(e.GenerateErrorReport());
    }

    public void LoadScene(string scn)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scn);
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                int coins = result.VirtualCurrency["NX"];  // Assuming 'NX' is your currency code
                UpdateCurrencyDisplay(coins);  // Update the UI with the fetched currency value
            }, OnError);
    }

    public void GetCatalog()
    {
        var catreq = new GetCatalogItemsRequest
        {
            CatalogVersion = "MainStore"
        };
        PlayFabClientAPI.GetCatalogItems(catreq,
            result =>
            {
                DisplayCatalogItems(result.Catalog);
            }, OnError);
    }

    public void GetPlayerInventory()
    {
        var UserInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInv,
            result =>
            {
                List<ItemInstance> ii = result.Inventory;
                UpdateMsg("Player Inventory");
                foreach(ItemInstance i  in ii)
                {
                    UpdateMsg(i.DisplayName + "," + i.ItemId + "," + i.ItemInstanceId);
                }
            }, OnError);
    }

    public void BuyItem(string itemId, uint price)
    {
        var buyreq = new PurchaseItemRequest
        {
            CatalogVersion = "MainStore",
            ItemId = itemId,
            VirtualCurrency = "NX",
            Price = (int)price 
        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result =>
            {
                UpdateMsg("Bought " + itemId + "!");
                GetVirtualCurrencies();  // Optionally update currency after purchase
            }, OnError);
    }

    public void DisplayCatalogItems(List<CatalogItem> items)
    {
        foreach (Transform child in shopPanel.transform)
        {
            Destroy(child.gameObject);  // Clear previous items
        }

        foreach (CatalogItem item in items)
        {
            GameObject itemObj = Instantiate(itemPrefab, shopPanel);
            TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = item.DisplayName;
            texts[1].text = $"Price: {item.VirtualCurrencyPrices["NX"]} NX";

            Button buyButton = itemObj.GetComponentInChildren<Button>();
            buyButton.onClick.RemoveAllListeners();
            uint itemPrice = item.VirtualCurrencyPrices.ContainsKey("NX") ? item.VirtualCurrencyPrices["NX"] : 0;
            buyButton.onClick.AddListener(delegate { BuyItem(item.ItemId, itemPrice); });
        }
    }

    public void UpdateCurrencyDisplay(int coins)
    {
        currencyDisplay.text = $"Nexus Coins: {coins}";  // Update the TextMeshPro text
    }

    private string ParseCustomData(string customDataJson, string key)
    {
        // Assuming custom data is stored in JSON format
        var dict = JsonUtility.FromJson<Dictionary<string, string>>(customDataJson);
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        return "Not specified";
    }

    private Dictionary<string, string> ParseCustomData(string customDataJson)
    {
        return JsonUtility.FromJson<Dictionary<string, string>>(customDataJson);
    }
}
