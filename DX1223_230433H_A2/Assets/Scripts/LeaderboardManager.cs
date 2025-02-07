using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] Transform leaderboardContainer;
    [SerializeField] GameObject leaderboardRowPrefab;
    [SerializeField] TextMeshProUGUI messageField;

    void Start()
    {
        GetGlobalLeaderboard();
        GetNearbyLeaderboard();
    }

    public void GetGlobalLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "highscore",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGlobalLeaderboardReceived, OnError);
    }

    private void OnGlobalLeaderboardReceived(GetLeaderboardResult result)
    {
        Debug.Log("Received Global Leaderboard Data");
        if (result.Leaderboard.Count == 0)
        {
            Debug.Log("Leaderboard is empty.");
            return;
        }

        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(leaderboardRowPrefab, leaderboardContainer);
            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = $"{item.DisplayName} ({item.PlayFabId})";
            texts[2].text = item.StatValue.ToString();
            Debug.Log($"Added leaderboard entry: {texts[1].text} with score {texts[2].text}");
        }
    }

    public void GetNearbyLeaderboard()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "highscore",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnNearbyLeaderboardReceived, OnError);
    }

    private void OnNearbyLeaderboardReceived(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log("Received Nearby Leaderboard Data");
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(leaderboardRowPrefab, leaderboardContainer);
            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = $"{item.DisplayName} ({item.PlayFabId})";
            texts[2].text = item.StatValue.ToString();
            Debug.Log($"Added nearby leaderboard entry: {texts[1].text} with score {texts[2].text}");
        }
    }

    private void OnError(PlayFabError error)
    {
        if (messageField != null)
        {
            messageField.text = "Error: " + error.GenerateErrorReport();
        }
        else
        {
            Debug.LogError("Leaderboard Error: " + error.GenerateErrorReport());
        }
    }
}