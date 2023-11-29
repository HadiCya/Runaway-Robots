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

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new BoardItem[9, 9];
        //Place a whole bunch of items

        
        PlaceBoardItem(player, 4, 4);
        PlaceBoardItem(robot, 8, 4);
        //PlaceBoardItem(robot, 0, 4);
        PlaceBoardItem(pit, 0, 0);
        PlaceBoardItem(pit, 7, 6);
        PlaceBoardItem(wall, 6, 4);
        PlaceBoardItem(wall, 6, 3);
        PlaceBoardItem(wall, 2, 2);
        PlaceBoardItem(wall, 1, 3);
        PlaceBoardItem(wall, 1, 4);
        PlaceBoardItem(fence, 4, 5);
        PlaceBoardItem(fence, 4, 6);
        

        //PlaceBoardItem(player, 4, 4);
        PlaceBoardItem(robot, 8, 5);
        PlaceBoardItem(pit, 6, 4);

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

    //Check indicated move position for obstacles and react accordingly 
    //  0 = Cannot move
    //  1 = Safe to move
    //  2 = Destroy item1
    //  3 = Destroy item2
    //  4 = Destroy both item1 and item2
    public int CheckCollision(int item1X, int item1Y, int item2X, int item2Y)
    {
        print("X1: " + item1X + " Y1: " + item1Y + " X2: " + item2X + " Y2: " + item2Y);
        //If out of bounds: don't move 
        if (item2X < 0 || item2X > gameBoard.GetLength(0) - 1 || item2Y < 0 || item2Y > gameBoard.GetLength(1) - 1)
        {
            return 0;
        }
        //If Player is trying to move:
        if (gameBoard[item1X, item1Y] is Player && !(gameBoard[item2X, item2Y] == null))
        {
            //If moving into a Wall: don't
            if (gameBoard[item2X, item2Y] is Wall)
            {
                return 0;
            }
            //If moving into a Robot or Pit: die
            else if ((gameBoard[item2X, item2Y] is Robot) || (gameBoard[item2X, item2Y] is Pit))
            {
                print("Player hit bot: " + gameBoard[item2X, item2Y]);
                print(gameBoard[item2X, item2Y] == null);
                print(gameBoard[item2X, item2Y].GetType());
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
            else if (gameBoard[item2X, item2Y] is Pit)
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
