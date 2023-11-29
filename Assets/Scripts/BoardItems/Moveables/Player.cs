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
    private int bombCount = 0;
    private float bombCooldown = 0f;
    private float bombInterval = 3f;
    private UnityEngine.UI.Image bombUiImage;
    private TextMeshProUGUI bombCountText;

    private bool movementDisabled = false;
    private float moveCooldown = 0f;
    private float moveInterval = 0.5f;

    void Start()
    {
        bombUiImage = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        bombCountText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        AddBomb();
        //for testing
        bombCount = 99;
        bombCountText.text = bombCount.ToString();
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
        if (Input.GetKeyDown(KeyCode.Space) && bombCooldown <= 0)
        {
            UseBomb();
        }
        
        //Bomb cooldown timer
        if (bombCooldown > 0)
        {
            bombCooldown -= Time.deltaTime;
        }
        else if (bombCooldown <= 0 && bombCount > 0)
        {
            bombUiImage.color = Color.white;
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
        //Check if player has any bombs left
        if (bombCount > 0)
        {
            movementDisabled = true;
            bombCooldown = bombInterval;
            moveCooldown = moveInterval;
            gameManager.SpawnBomb(xPos, yPos);
            bombCount--;
            bombCountText.text = bombCount.ToString();
            //Gray out bomb button (make it look better later)
            bombUiImage.color = Color.gray;
        }
    }

    //Add bomb (for start of new level)
    public void AddBomb()
    {
        bombCount++;
        bombUiImage.color = Color.white;
        // *Enable bomb ui* 
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
