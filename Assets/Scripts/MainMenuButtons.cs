using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuButtons : MonoBehaviour
{
    private Leaderboard leaderboard;
    public TMP_InputField usernameInputfield;
    public TextMeshProUGUI usernameTextbox;

    // Start is called before the first frame update
    void Start()
    {
        leaderboard = GameObject.FindFirstObjectByType<Leaderboard>();

        Invoke(nameof(GetName), 5);
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
        usernameTextbox.text = await leaderboard.GetName();
    }

    public void UpdateName()
    {
        //"The name should not contain any white space and should have a maximum length of 50 characters" - from unity
        if (usernameInputfield.text.Contains(" ") || usernameInputfield.text.Length > 49 || usernameInputfield.text.Length < 1)
        {
            Debug.LogError("Invalid name");
            return;
        }
        //and then proabbly figure out a profanity filter 
        // https://github.com/coffee-and-fun/google-profanity-words ?
        //and then limit it to once per month or just once period?
        leaderboard.UpdateName(usernameInputfield.text);
        usernameInputfield.text = string.Empty;
    }
}
