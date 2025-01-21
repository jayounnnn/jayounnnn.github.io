using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Msg;

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
            r =>
            {
                int coins = r.VirtualCurrency["NX"];
                UpdateMsg("Nexus Credits:" + coins);
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
                List<CatalogItem> items = result.Catalog;
                UpdateMsg("Catalog Items");
                foreach (CatalogItem i in items)
                {
                    UpdateMsg(i.DisplayName + "," + i.VirtualCurrencyPrices["NX"]);
                }
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

    public void BuyItem()
    {
        var buyreq = new PurchaseItemRequest
        {
            // Current sample is hardcoded, should make it more dynamic
            CatalogVersion = "MainStore",
            ItemId = "Weapon01PC", // Replace with your item ID
            VirtualCurrency = "NX",
            Price = 2
        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            result =>
            {
                UpdateMsg("Bought!");
            }, OnError);
    }
}
