using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButtons : MonoBehaviour
{
    public TextMeshProUGUI levelReachedNumText;
    public TextMeshProUGUI robotsDefeatedNumText;
    public TextMeshProUGUI bombsKeptNumText;
    public TextMeshProUGUI totalScoreNumText;

    // Start is called before the first frame update
    void Start()
    {
        levelReachedNumText.text = (GameManager.levelCount * 100).ToString();
        robotsDefeatedNumText.text = (GameManager.robotsDefeated * 100).ToString();
        bombsKeptNumText.text = (GameManager.bombCount * 100).ToString();
        totalScoreNumText.text = ((GameManager.levelCount * 100) + (GameManager.robotsDefeated * 100) + (GameManager.bombCount * 100)).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
