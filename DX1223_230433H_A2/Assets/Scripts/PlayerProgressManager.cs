using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerProgressManager : MonoBehaviour
{
    public TextMeshProUGUI levelText; 
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    public void Start()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            FetchPlayerProgress();
        }
        else
        {
            Debug.LogError("User must be logged in to fetch XP and Level");
        }

        InitializePlayerData();

        GetUserData(data =>
        {
            int currentXP = int.Parse(data["XP"]);
            int currentLevel = int.Parse(data["Level"]);
            UpdateUI(currentXP, currentLevel);  
        });
    }

    private void InitializePlayerData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "XP", "0" },
                { "Level", "1" }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, result => Debug.Log("User data initialized successfully."), error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void AddXP(int amount)
    {
        GetUserData(data =>
        {
            int currentXP = int.Parse(data["XP"]);
            int currentLevel = int.Parse(data["Level"]);

            currentXP += amount;

            if (currentXP >= XPForNextLevel(currentLevel))
            {
                currentXP -= XPForNextLevel(currentLevel);
                currentLevel++;
            }

            UpdateUserData(currentXP, currentLevel);
            UpdateUI(currentXP, currentLevel); 
        });
    }

    private void GetUserData(System.Action<Dictionary<string, string>> onCompleted)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("XP") && result.Data.ContainsKey("Level"))
                onCompleted(result.Data.ToDictionary(kv => kv.Key, kv => kv.Value.Value));
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void UpdateUserData(int xp, int level)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "XP", xp.ToString() },
            { "Level", level.ToString() }
        },
            Permission = UserDataPermission.Private
        };
        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("User data updated successfully.");
            UpdateUI(xp, level);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    private int XPForNextLevel(int currentLevel)
    {
        return 100 * currentLevel;
    }

    private void UpdateUI(int currentXP, int currentLevel)
    {
        levelText.text = "Level " + currentLevel;
        xpText.text = currentXP + "/" + XPForNextLevel(currentLevel);
        xpSlider.maxValue = XPForNextLevel(currentLevel);
        xpSlider.value = currentXP;
    }

    public void FetchPlayerProgress()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("XP") && result.Data.ContainsKey("Level"))
            {
                int currentXP = int.Parse(result.Data["XP"].Value);
                int currentLevel = int.Parse(result.Data["Level"].Value);
                UpdateUI(currentXP, currentLevel);
            }
            else
            {
                Debug.Log("No XP or Level data found, initializing...");
                InitializePlayerData(); 
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }
}
