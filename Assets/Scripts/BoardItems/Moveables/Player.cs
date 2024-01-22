using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : Moveable
{
    private bool movementDisabled = false;
    private float moveCooldown = 0f;
    private readonly float moveInterval = 0.5f;

    //Touchscreen stuff
    private Touch playerTouch;
    private Vector2 touchStartPosition, touchEndPosition;
    private float swipeX, swipeY;
    private string direction;
    //maybe make adjustable in settings?
    private readonly float swipeXThreshold = 15;
    private readonly float swipeYThreshold = 30;

    //Joystick stuff
    private PlayerJoystick playerJoystick;

    //Buffers
    private int bufferX;  // x axis input buffer
    private int bufferY;  // y axis input buffer

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerJoystick = new PlayerJoystick();
        playerJoystick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.playerMovementDisabled)
        {
            KeyBoardControls();
            //TouchControls();
            JoystickControls();
            UseBomb();
            CheckMovementBuffer();
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

    private void KeyBoardControls()
    {
        //Move player's icon in direction indicated by player

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

    private void TouchControls()
    {
        if (Input.touchCount > 0)
        {
            playerTouch = Input.GetTouch(0);

            //Get position of touch start
            if (playerTouch.phase == UnityEngine.TouchPhase.Began)
            {
                touchStartPosition = playerTouch.position;
            }

            else if (playerTouch.phase == UnityEngine.TouchPhase.Ended)
            {
                touchEndPosition = playerTouch.position;

                swipeX = touchEndPosition.x - touchStartPosition.x;
                swipeY = touchEndPosition.y - touchStartPosition.y;

                //Check if just a tap and skip
                if (Mathf.Abs(swipeX) < swipeXThreshold && Mathf.Abs(swipeY) < swipeYThreshold)
                {
                    return;
                }

                //Else calculate swipe direction
                direction = "";
                if (Mathf.Abs(swipeY) > swipeYThreshold)
                {
                    direction = swipeY > 0 ? "Up" : "Down";
                }
                if (Mathf.Abs(swipeX) > swipeXThreshold)
                {
                    direction += swipeX > 0 ? "Right" : "Left";
                }
                print("X: " + swipeX + "   Y: " + swipeY + "    Dir: " + direction);

                //Move up
                if (direction == "Up")
                {
                    MoveItem(0, -1);
                }
                //Move up-right
                else if (direction == "UpRight")
                {
                    MoveItem(1, -1);
                }
                //Move right
                else if (direction == "Right")
                {
                    MoveItem(1, 0);
                }
                //Move down-right
                else if (direction == "DownRight")
                {
                    MoveItem(1, 1);
                }
                //Move down
                else if (direction == "Down")
                {
                    MoveItem(0, 1);
                }
                //Move down-left
                else if (direction == "DownLeft")
                {
                    MoveItem(-1, 1);
                }
                //Move left
                else if (direction == "Left")
                {
                    MoveItem(-1, 0);
                }
                //Move up-left
                else if (direction == "UpLeft")
                {
                    MoveItem(-1, -1);
                }
            }
        }
    }

    private void JoystickControls()
    {
        Vector2 joystickMovement = playerJoystick.Player.Move.ReadValue<Vector2>();

        //Move up
        //unit circle??????
        if (joystickMovement.x > -0.4 && joystickMovement.x < 0.4 && joystickMovement.y > 0)
        {
            MoveItem(0, -1);
        }
        //Move up-right
        else if (joystickMovement.x > 0.4 && joystickMovement.x < 0.9 && joystickMovement.y > 0)
        {
            MoveItem(1, -1);
        }
        //Move right
        else if (joystickMovement.x > 0 && joystickMovement.y > -0.4 && joystickMovement.y < 0.4)
        {
            MoveItem(1, 0);
        }
        //Move down-right
        else if (joystickMovement.x > 0.4 && joystickMovement.x < 0.9 && joystickMovement.y < 0)
        {
            MoveItem(1, 1);
        }
        //Move down
        else if (joystickMovement.x > -0.4 && joystickMovement.x < 0.4 && joystickMovement.y < 0)
        {
            MoveItem(0, 1);
        }
        //Move down-left
        else if (joystickMovement.x > -0.9 && joystickMovement.x < -0.4 && joystickMovement.y < 0)
        {
            MoveItem(-1, 1);
        }
        //Move left
        else if (joystickMovement.x < 0 && joystickMovement.y > -0.4 && joystickMovement.y < 0.4)
        {
            MoveItem(-1, 0);
        }
        //Move up-left
        else if (joystickMovement.x > -0.9 && joystickMovement.x < -0.4 && joystickMovement.y > 0)
        {
            MoveItem(-1, -1);
        }
    }

    //Spawn Bomb on player's position
    private void UseBomb()
    {
        //Spawn Bomb
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.CanUseBomb())
        {
            movementDisabled = true;
            moveCooldown = moveInterval;
            gameManager.SpawnBombs(xPos, yPos);
        }
    }

    protected override void MoveItem(int xMove, int yMove)
    {
        if (!movementDisabled)
        {
            base.MoveItem(xMove, yMove);

            if (base.collisionResult == 1 || base.collisionResult == 3)
            {
                movementDisabled = true;
                moveCooldown = 0.18f;
            }
        }
        else
        {
            SetMovementBuffer(xMove, yMove);
        }
    }

    public void MoveFromButton(int x, int y)
    {
        if (!gameManager.playerMovementDisabled)
        {
            MoveItem(x, y);
        }
    }

    private void SetMovementBuffer(int x, int y)
    {
        bufferX = x;
        bufferY = y;
    }

    private bool BufferEmpty()
    {
        return bufferX == 0 && bufferY == 0;
    }

    private void CheckMovementBuffer()
    {
        if (!movementDisabled && !gameManager.playerMovementDisabled && !BufferEmpty())
        {
            MoveItem(bufferX, bufferY);
            SetMovementBuffer(0, 0);
        }
    }
}