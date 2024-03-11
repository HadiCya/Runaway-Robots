using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    private Leaderboard leaderboard;
    public TMP_InputField usernameInputfield;
    public Button usernameButton;
    public TextMeshProUGUI usernameTextbox;
    public GameObject leaderboardDisplay;
    public TextMeshProUGUI[] leaderboardNames;
    public TextMeshProUGUI leaderboardScores;
    public TextMeshProUGUI currentPlayerScore;
    public GameObject settingsMenu;
    private ProfanityList profanityList;
    public TextMeshProUGUI messageBox;
    public Slider sfxVolumeSlider;
    public Slider bgmVolumeSlider;
    public Button mobileControlsButton;
    public GameObject infoMenu;
    public GameObject dsaMenu;
    public TextMeshProUGUI dsaNotificationBox;

    // Start is called before the first frame update
    void Start()
    {
        leaderboard = GameObject.FindFirstObjectByType<Leaderboard>();
        usernameInputfield.characterLimit = 20;
        leaderboardDisplay.SetActive(false);
        settingsMenu.SetActive(false);
        infoMenu.SetActive(false);
        dsaMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //developer secret (remove later)
        if (Input.GetKey(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("nameChanged");
            Debug.Log("reset");
        }
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

    public async void UpdateName()
    {
        messageBox.text = string.Empty;
        //"The name should not contain any white space and should have a maximum length of 50 characters" - from unity
        if (usernameInputfield.text.Contains(" ") || usernameInputfield.text.Length < 2)
        {
            Debug.LogError("Invalid username: name must be 3-20 characters and contain no white space");
            messageBox.text = "Name must be 3-20 characters and contain no white space";
            return;
        }
        profanityList = gameObject.GetComponent<ProfanityList>();
        if (profanityList.isBadWord(usernameInputfield.text.ToLower()))
        {
            Debug.LogError($"Invalid username: {usernameInputfield.text} is a bad word >:(");
            messageBox.text = "That's a banned word";
            return;
        }

        await leaderboard.UpdateName(usernameInputfield.text);

        if (PlayerPrefs.GetString("nameChanged") == "true")
        {
            messageBox.text = "You can no longer change your name";
            usernameInputfield.text = string.Empty;
            usernameInputfield.interactable = false;
            usernameButton.interactable = false;
        }

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
        
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("bgmVolume");
        }
        if (PlayerPrefs.HasKey("nameChanged"))
        {
            if (PlayerPrefs.GetString("nameChanged") == "true")
            {
                messageBox.text = string.Empty;
                usernameInputfield.text = string.Empty;
                usernameInputfield.interactable = false;
                usernameButton.interactable = false;
            }
        }
        
        if (PlayerPrefs.HasKey("mobileControls"))
        {
            if (string.Equals(PlayerPrefs.GetString("mobileControls"), "joystick"))
            {
                mobileControlsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Joystick\n(Change)";
            }
            else
            {
                mobileControlsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Buttons\n(Change)";
            }
        }
        else
        {
            PlayerPrefs.SetString("mobileControls", "buttons");
            mobileControlsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Buttons\n(Change)";
        }

        if (string.Equals(usernameTextbox.text, "Username"))
        {
            usernameTextbox.text = await leaderboard.GetName();
        }
    }

    public void ChangeSFXVolume()
    {
        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);
    }

    public void ChangeBGMVolume()
    {
        PlayerPrefs.SetFloat("bgmVolume", bgmVolumeSlider.value);
    }

    public void ChangeMobileControls()
    {
        if (string.Equals(PlayerPrefs.GetString("mobileControls"), "joystick"))
        {
            PlayerPrefs.SetString("mobileControls", "buttons");
            mobileControlsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Buttons\n(Change)";
        }
        else
        {            
            PlayerPrefs.SetString("mobileControls", "joystick");
            mobileControlsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Joystick\n(Change)";
        }
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void OpenInfo()
    {
        infoMenu.SetActive(true);
    }

    public void CloseInfo()
    {
        infoMenu.SetActive(false);
    }

    public void OpenDSAPage()
    {
        dsaMenu.SetActive(true);
        dsaNotificationBox.text = leaderboard.GetNotifications();
    }

    public void CloseDSAPage()
    {
        dsaMenu.SetActive(false);
    }
}
