using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using static UnityEditor.Progress;

public class PlayFabUserMgtTMP : MonoBehaviour
{
    [SerializeField] TMP_InputField userEmail, userPassword, userName, currentScore, displayName;
    [SerializeField] TextMeshProUGUI Msg;
    void UpdateMsg(string msg) // To display in console and messagebox
    {
        Debug.Log(msg);
        Msg.text = msg;
    }
    void OnError(PlayFabError e) //report any errors here!
    {
        UpdateMsg("Error" + e.GenerateErrorReport());
    }

    public void OnButtonRegUser() // For button click
    {
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,
            Username = userName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegSuccess, OnError);
    }

    void OnRegSuccess(RegisterPlayFabUserResult r)
    {
        UpdateMsg("Registration success!");

        // To create a player display name
        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName.text,
        };
        // Update to profile
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMsg("display name updated!" + r.DisplayName);
    }

    void OnLoginSuccess(LoginResult r)
    {
        UpdateMsg("Login Success!" + r.PlayFabId + r.InfoResultPayload.PlayerProfile.DisplayName);
    }
    public void OnButtonLoginEmail() // Login using email + password
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,

            // To get player profile, to get displayname
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
    }

    public void onButtonLoginUserName() // Login using username + password
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = userName.text,
            Password = userPassword.text,

            // To get player profile, including displayname
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, OnError);
    }

    public void OnButtonGetLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highscore", // Playfab leaderboard statistic name StartPosition = 0,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);
    }


    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        string LeaderboardStr = "Leaderboard\n";
        foreach (var item in r.Leaderboard)
        {
            string onerow = item.Position + "/" + item.PlayFabId + "/" + item.DisplayName + "/" + item.StatValue + "\n";
            LeaderboardStr += onerow; // Combine all display into one string 1.
        }
        UpdateMsg(LeaderboardStr);
    }
    public void onButtonSendLeaderboard()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> // Playfab leaderboard statistic name
            { 
                new StatisticUpdate
                {
                    StatisticName = "highscore",
                    Value = int.Parse(currentScore.text)
                }
            }
        };
        UpdateMsg("Submitting score:" + currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMsg("Successful leaderboard sent:" + r.ToString());
    }
}

