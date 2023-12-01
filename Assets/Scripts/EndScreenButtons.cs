using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButtons : MonoBehaviour
{
    public TextMeshProUGUI levelCountText;
    
    // Start is called before the first frame update
    void Start()
    {
        levelCountText.text = "Level Reached: " + GameManager.levelCount;
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
