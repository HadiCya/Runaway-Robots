using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
