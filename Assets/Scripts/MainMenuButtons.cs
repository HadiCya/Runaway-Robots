using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Leaderboards;

public class MainMenuButtons : MonoBehaviour
{
    private Leaderboard leaderboard;
    public TMP_InputField usernameInputfield;
    public TextMeshProUGUI usernameTextbox;
    public GameObject leaderboardDisplay;
    public TextMeshProUGUI[] leaderboardNames;
    public TextMeshProUGUI leaderboardScores;
    public TextMeshProUGUI currentPlayerScore;
    public GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        leaderboard = GameObject.FindFirstObjectByType<Leaderboard>();
        usernameInputfield.characterLimit = 20;
        leaderboardDisplay.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
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

    public async void GetName()
    {
        //TODO: CHECK IF LOGGED IN
        usernameTextbox.text = await leaderboard.GetName();
    }

    public void UpdateName()
    {
        //"The name should not contain any white space and should have a maximum length of 50 characters" - from unity
        if (usernameInputfield.text.Contains(" ") || usernameInputfield.text.Length < 2)
        {
            Debug.LogError("Invalid username: name must be 3-20 characters and contain no white space");
            return;
        }
        //and then proabbly figure out a profanity filter 
        // https://github.com/coffee-and-fun/google-profanity-words ?
        //and then limit it to once per month or just once period?
        leaderboard.UpdateName(usernameInputfield.text);
        usernameInputfield.text = string.Empty;
        GetName();
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

    public async void OpenSettings()
    {
        settingsMenu.SetActive(true);
        if (string.Equals(usernameTextbox.text, "Username"))
        {
            usernameTextbox.text = await leaderboard.GetName();
        }
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }
}
