using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : Moveable
{
    private bool movementDisabled = false;
    private float moveCooldown = 0f;
    private float moveInterval = 0.5f;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move player's icon in direction indicated by player
        if (!movementDisabled)
        {
            //Move up
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                MoveItem(0, -1);
            }
            //Move up-right
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                MoveItem(1, -1);
            }
            //Move right
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                MoveItem(1, 0);
            }
            //Move down-right
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                MoveItem(1, 1);
            }
            //Move down
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                MoveItem(0, 1);
            }
            //Move down-left
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                MoveItem(-1, 1);
            }
            //Move left
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                MoveItem(-1, 0);
            }
            //Move up-left
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                MoveItem(-1, -1);
            }
        }

        //Spawn Bomb
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.CanUseBomb())
        {
            UseBomb();
        }

        //Movement cooldown timer
        if (moveCooldown > 0)
        {
            moveCooldown -= Time.deltaTime;
        }
        else if (moveCooldown <= 0 && movementDisabled)
        {
            movementDisabled = false;
        }
    }

    //Spawn Bomb on player's position
    private void UseBomb()
    {
        movementDisabled = true;
        moveCooldown = moveInterval;
        gameManager.SpawnBomb(xPos, yPos);
    }

    //Check level for any remaining robots
    private void CheckForRobots()
    {
        if (GameObject.FindGameObjectWithTag("Robot") == null)
        {
            Debug.Log("All gone!");
        }
    }
}
