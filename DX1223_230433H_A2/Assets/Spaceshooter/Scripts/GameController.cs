using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public Vector3 positionAsteroid;
    public GameObject asteroid;
    public GameObject asteroid2;
    public GameObject asteroid3;
    public int hazardCount;
    public float startWait;
    public float spawnWait;
    public float waitForWaves;
    public Text scoreText;
    public Text gameOverText;
    public Text restartText;
    public Text mainMenuText;

    private bool restart;
    private bool gameOver;
    private int score;
    private List<GameObject> asteroids;

    public PlayerProgressManager progressManager;
    private int accumulatedScore = 0; 

    private void Start() 
    {
        asteroids = new List<GameObject>
        {
            asteroid,
            asteroid2,
            asteroid3
        };
        gameOverText.text = "";
        restartText.text = "";
        mainMenuText.text = "";
        restart = false;
        gameOver = false;
        score = 0;
        StartCoroutine(spawnWaves());
        updateScore();

        if (progressManager == null)
        {
            progressManager = FindObjectOfType<PlayerProgressManager>();
        }


    }

    private void Update() {
        if(restart){
            if(Input.GetKey(KeyCode.R)){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } 
            else if(Input.GetKey(KeyCode.Q)){
                SceneManager.LoadScene("Menu");
            }
        }
        if (gameOver) {
            restartText.text = "Press R to restart game";
            mainMenuText.text = "Press Q to go back to main menu";
            restart = true;
        }
    }

    private IEnumerator spawnWaves(){
        yield return new WaitForSeconds(startWait);
        while(true){
            for (int i = 0; i < hazardCount;i++){
                Vector3 position = new Vector3(Random.Range(-positionAsteroid.x, positionAsteroid.x), positionAsteroid.y, positionAsteroid.z);
                Quaternion rotation = Quaternion.identity;
                Instantiate(asteroids[Random.Range(0,3)], position, rotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waitForWaves);
            if(gameOver){
                break;
            }
        }
    }

    public void gameIsOver(){
        gameOverText.text = "Game Over";
        gameOver = true;
        SubmitScoreToPlayFab(score);
    }

    public void addScore(int newScore)
    {
        score += newScore;
        accumulatedScore += newScore;

        while (accumulatedScore >= 50)
        {
            progressManager.AddXP(1);
            accumulatedScore -= 50;
        }

        updateScore();
    }

    void updateScore(){
        scoreText.text = "Score:" + score;
    }

    // Function to submit score to PlayFab
void SubmitScoreToPlayFab(int finalScore)
{
        Debug.Log($"Submitting score: {finalScore}");

        var request = new UpdatePlayerStatisticsRequest
    {
        Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = "highscore", 
                Value = finalScore
            }
        }
    };

    PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreUpdateSuccess, OnError);
}

void OnScoreUpdateSuccess(UpdatePlayerStatisticsResult result)
{
    Debug.Log("Score successfully submitted to PlayFab!");
}

void OnError(PlayFabError error)
{
    Debug.LogError("Error updating PlayFab leaderboard: " + error.GenerateErrorReport());
}
}
