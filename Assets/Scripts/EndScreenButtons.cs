using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButtons : MonoBehaviour
{
    public TextMeshProUGUI levelReachedNumText;
    private int levelCountScore;
    public TextMeshProUGUI robotsDefeatedNumText;
    private int robotsDefeatedScore;
    public TextMeshProUGUI bombsKeptNumText;
    private int bombsKeptScore;
    public TextMeshProUGUI totalScoreNumText;
    private int totalScoreScore;

    private Leaderboard leaderboard;
    public GameObject leaderboardDisplay;
    public TextMeshProUGUI[] leaderboardNames;
    public TextMeshProUGUI leaderboardScores;
    public TextMeshProUGUI currentPlayerScore;

    // Start is called before the first frame update
    void Start()
    {
        levelCountScore = GameManager.levelCount * 100;
        levelReachedNumText.text = GameManager.levelCount + " X100 = " + levelCountScore;
        robotsDefeatedScore = GameManager.robotsDefeated * 100;
        robotsDefeatedNumText.text = GameManager.robotsDefeated + " X100 = " + robotsDefeatedScore;
        bombsKeptScore = GameManager.bombCount * 100;
        bombsKeptNumText.text = GameManager.bombCount + " X100 = " + bombsKeptScore;
        totalScoreScore = levelCountScore + robotsDefeatedScore + bombsKeptScore;
        totalScoreNumText.text = totalScoreScore.ToString();

        CallLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallLeaderboard()
    {
        leaderboard = GameObject.FindFirstObjectByType<Leaderboard>();
        leaderboard.AddScore(totalScoreScore);
    }

    public async void OpenLeaderboard()
    {
        leaderboardDisplay.SetActive(true);
        if (string.Equals(leaderboardNames[0].text, "Loading score..."))
        {
            string[,] scores = await leaderboard.GetScores();
            //leaderboardNames[0].text = string.Empty;
            leaderboardScores.text = string.Empty;
            for (int i = 0; i < scores.GetLength(0); i++)
            {
                leaderboardNames[i].text = scores[i, 0];
            }
            for (int i = 0; i < scores.GetLength(0); i++)
            {
                leaderboardScores.text += scores[i, 1] + "\n";
            }
        }
        currentPlayerScore.text = await leaderboard.GetPlayerScore();
    }

    public void CloseLeaderboard()
    {
        leaderboardDisplay.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("MovementAndCollisions");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
