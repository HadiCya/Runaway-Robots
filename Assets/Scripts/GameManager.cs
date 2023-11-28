using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public BoardItem[,] gameBoard;
    //Prefabs set in inspector 
    public BoardItem player;
    public BoardItem robot;
    public BoardItem pit;
    public BoardItem wall;
    public BoardItem fence;
    public BoardItem bomb;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new BoardItem[9, 9];
        //Place a whole bunch of items
        PlaceBoardItem(player, 0, 8);

        PlaceBoardItem(robot, 8, 0);

        PlaceBoardItem(wall, 3, 3);
        //PlaceBoardItem(wall, 4, 3);
        PlaceBoardItem(wall, 5, 3);

        //PlaceBoardItem(wall, 3, 4);
        //PlaceBoardItem(wall, 5, 4);

        PlaceBoardItem(wall, 3, 5);
        //PlaceBoardItem(wall, 4, 5);
        PlaceBoardItem(wall, 5, 5);

        PlaceBoardItem(pit, 4, 4);
        
        PrintBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Place specified item at specified location on array and create object in scene
    private void PlaceBoardItem(BoardItem item, int x, int y)
    {
        gameBoard[x, y] = Instantiate(item, new Vector2(x * 1.11f, y * -1.11f), item.transform.rotation);
        gameBoard[x, y].xPos = x;
        gameBoard[x, y].yPos = y;
    }

    //Update an item's location in the array (after moving)
    public void UpdatePosition(BoardItem item)
    {
        gameBoard[item.xPos, item.yPos] = item;
        //PrintBoard();
    }

    //Remove an item's location in the array (after being destroyed)
    public void ClearPosition(int x, int y)
    {
        gameBoard[x, y] = null;
    }

    //Spawn Bomb powerups at 3x3 grid around player's position
    public void SpawnBomb(int x, int y)
    {
        //Top-left
        if (x - 1 >= 0 && y - 1 >= 0 && CheckIfEmpty(x - 1, y - 1) == 1)
        {
            PlaceBoardItem(bomb, x - 1, y - 1);
        }
        //Top-middle
        if (y - 1 >= 0 && CheckIfEmpty(x, y - 1) == 1)
        {
            PlaceBoardItem(bomb, x, y - 1);
        }
        //Top-right
        if (x + 1 <= gameBoard.GetLength(0) - 1 && y - 1 >= 0 && CheckIfEmpty(x + 1, y - 1) == 1)
        {
            PlaceBoardItem(bomb, x + 1, y - 1);
        }
        //Middle-Left
        if (x - 1 >= 0 && CheckIfEmpty(x - 1, y) == 1)
        {
            PlaceBoardItem(bomb, x - 1, y);
        }
        //Middle-Middle
        //PlaceBoardItem(bomb, x, y);   //Dont place it on player idk depends on what we wanna do
        //Middle-Right
        if (x + 1 <= gameBoard.GetLength(0) - 1 && CheckIfEmpty(x + 1, y) == 1)
        {
            PlaceBoardItem(bomb, x + 1, y);
        }
        //Bottom-Left
        if (x - 1 >= 0 && y + 1 <= gameBoard.GetLength(1) - 1 && CheckIfEmpty(x - 1, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x - 1, y + 1);
        }
        //Bottom-Middle
        if (y + 1 <= gameBoard.GetLength(1) - 1 && CheckIfEmpty(x, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x, y + 1);
        }
        //Bottom-Right
        if (x + 1 <= gameBoard.GetLength(0) - 1 && y + 1 <= gameBoard.GetLength(1) - 1 && CheckIfEmpty(x + 1, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x + 1, y + 1);
        }
    }

    //Check indicated move position for obstacles and react accordingly 
    //  0 = Cannot move
    //  1 = Safe to move
    //  2 = Destroy item1
    //  3 = Destroy item2
    //  4 = Destroy both item1 and item2
    public int CheckCollision(int item1X, int item1Y, int item2X, int item2Y)
    {
        //If out of bounds: don't move 
        if (item2X < 0 || item2X > gameBoard.GetLength(0) - 1 || item2Y < 0 || item2Y > gameBoard.GetLength(1) - 1)
        {
            return 0;
        }
        //If Player is trying to move:
        if (gameBoard[item1X, item1Y] is Player)
        {
            //If moving into a Wall: don't
            if (gameBoard[item2X, item2Y] is Wall)
            {
                return 0;
            }
            //If moving into a Robot or Pit: die
            else if ((gameBoard[item2X, item2Y] is Robot) || (gameBoard[item2X, item2Y] is Pit))
            {
                return 2;
            }
            //If moving into an Electric Fence: die and destroy the fence
            else if (gameBoard[item2X, item2Y] is ElectricFence)
            {
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 4;
            }
        }
        //If a Robot is trying to move:
        else if (gameBoard[item1X, item1Y] is Robot)
        {
            //If moving into a Wall or another Robot: don't 
            if (gameBoard[item2X, item2Y] is Wall || gameBoard[item2X, item2Y] is Robot)
            {
                return 0;
            }
            //If moving into a Pit: die
            else if (gameBoard[item2X, item2Y] is Pit || gameBoard[item2X, item2Y] is Bomb)
            {
                return 2;
            }
            //If moving into the Player: kill them
            else if (gameBoard[item2X, item2Y] is Player)
            {
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 3;
            }
            //If moving into an Electric Fence, die and destroy the fence
            else if (gameBoard[item2X, item2Y] is ElectricFence)
            {
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 4;
            }
        }
        //No obstacle to collide with, so move along
        return 1;
    }

    //Check if the indicated position is empty (for bombs)
    //  0 == not empty
    //  1 == empty or robot
    public int CheckIfEmpty(int x, int y)
    {
        if (gameBoard[x, y] is Wall)
        {
            return 0;
        }
        else if (gameBoard[x, y] is Pit)
        {
            return 0;
        }
        else if (gameBoard[x, y] is ElectricFence)
        {
            return 0;
        }
        else if (gameBoard[x, y] is Player)
        {
            return 0;
        }
        //If spawned on robot, destroy it
        else if (gameBoard[x, y] is Robot)
        {
            gameBoard[x, y].DestroyItem();
        }
        return 1;
    }

    private void PrintBoard()
    {
        string board = "";
        for (int i = 0; i < 9; i++)
        {
            for (int e = 0; e < 9; e++)
            {
                if (gameBoard[e, i] == null)
                {
                    board += "null\t\t";
                }
                else
                {
                    board += gameBoard[e, i] + "\t";
                }
            }
            board += "\n";
        }
        Debug.Log(board);
    }
}
