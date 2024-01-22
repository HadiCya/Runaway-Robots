using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombButton : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public void UseBomb()
    {
        if (gameManager != null && gameManager.CanUseBomb())
        {
            gameManager.SpawnBombs(player.xPos, player.yPos);
        }
    }
}
