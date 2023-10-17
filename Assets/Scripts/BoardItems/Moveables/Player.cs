using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moveable
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move player's icon in direction indicated by player
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

    //Check level for any more robots
    private void CheckForRobots()
    {
        if (GameObject.FindGameObjectWithTag("Robot") == null)
        {
            Debug.Log("All gone!");
        }
    }
}
