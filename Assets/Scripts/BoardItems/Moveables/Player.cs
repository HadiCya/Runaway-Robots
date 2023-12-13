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
        if (!gameManager.playerMovementDisabled)
        {
            PlayerControls();
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

    private void PlayerControls()
    {
        //Move player's icon in direction indicated by player
        if (!movementDisabled)
        {
            //Move up
            if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.W))
            {
                MoveItem(0, -1);
            }
            //Move up-right
            else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.E))
            {
                MoveItem(1, -1);
            }
            //Move right
            else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.D))
            {
                MoveItem(1, 0);
            }
            //Move down-right
            else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.C))
            {
                MoveItem(1, 1);
            }
            //Move down
            else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.X))
            {
                MoveItem(0, 1);
            }
            //Move down-left
            else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Z))
            {
                MoveItem(-1, 1);
            }
            //Move left
            else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.A))
            {
                MoveItem(-1, 0);
            }
            //Move up-left
            else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Q))
            {
                MoveItem(-1, -1);
            }
        }

        //Spawn Bomb
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.CanUseBomb())
        {
            UseBomb();
        }
    }

    //Spawn Bomb on player's position
    private void UseBomb()
    {
        movementDisabled = true;
        moveCooldown = moveInterval;
        gameManager.SpawnBomb(xPos, yPos);
    }
}
