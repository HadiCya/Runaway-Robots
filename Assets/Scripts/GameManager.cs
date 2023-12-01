using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private List<string> level_list;
    private int board_size = 9;

    private int robotCount = 0;

    //Bomb stuff
    public int bombCount = 0;
    private float bombCooldown = 0f;
    private float bombInterval = 3f;
    private UnityEngine.UI.Image bombUiImage;
    private TextMeshProUGUI bombCountText;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new BoardItem[board_size, board_size];
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

        PreSetLevels();

        PrintBoard();

        bombUiImage = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        bombCountText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        AddBomb();
        bombCount = 10; //for testing
        robotCount = 0;
    }

    private void PreSetLevels() {
        level_list = new List<string>();
        // P = player  r = robot  w = wall  p = pit  f = fence
        level_list.Add( "-----r---" +
                        "--r---r--" +
                        "---------" +
                        "r---p----" +
                        "---pPp---" +
                        "----p----" +
                        "------r--" +
                        "--r------" +
                        "---------");

        level_list.Add( "--r-r----" +
                        "---------" +
                        "---------" +
                        "---------" +
                        "--p------" +
                        "-----P---" +
                        "---------" +
                        "---------" +
                        "---------");

        level_list.Add( "--r-r-r--" +
                        "---------" +
                        "---------" +
                        "---------" +
                        "-------p-" +
                        "-P-------" +
                        "---------" +
                        "---------" +
                        "---------");

        level_list.Add( "--r-r-r-r" +
                        "---------" +
                        "---------" +
                        "---------" +
                        "---p-----" +
                        "---------" +
                        "-----P---" +
                        "---------" +
                        "---------");

        level_list.Add( "r-r-r-r-r" +
                        "---------" +
                        "---------" +
                        "---------" +
                        "----P----" +
                        "---------" +
                        "---------" +
                        "--fffff--" +
                        "---------");

    }



    // Update is called once per frame
    void Update()
    {
        //Bomb cooldown timer
        if (bombCooldown > 0)
        {
            bombCooldown -= Time.deltaTime;
        }
        else if (bombCooldown <= 0 && bombCount > 0)
        {
            bombUiImage.color = Color.white;
        }
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
        //CheckForRobots();
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
        if (x + 1 <= board_size - 1 && y - 1 >= 0 && CheckIfEmpty(x + 1, y - 1) == 1)
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
        if (x + 1 <= board_size - 1 && CheckIfEmpty(x + 1, y) == 1)
        {
            PlaceBoardItem(bomb, x + 1, y);
        }
        //Bottom-Left
        if (x - 1 >= 0 && y + 1 <= board_size - 1 && CheckIfEmpty(x - 1, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x - 1, y + 1);
        }
        //Bottom-Middle
        if (y + 1 <= board_size - 1 && CheckIfEmpty(x, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x, y + 1);
        }
        //Bottom-Right
        if (x + 1 <= board_size - 1 && y + 1 <= board_size - 1 && CheckIfEmpty(x + 1, y + 1) == 1)
        {
            PlaceBoardItem(bomb, x + 1, y + 1);
        }

        bombCount--;
        bombCountText.text = bombCount.ToString();
        bombCooldown = bombInterval;
        bombUiImage.color = Color.gray; //Gray out bomb button (make it look better later)
        // *Disable bomb UI*
    }

    //Add bomb (for start of new level)
    public void AddBomb()
    {
        bombCount++;
        bombCooldown = 0;
        bombUiImage.color = Color.white;
        bombCountText.text = bombCount.ToString();
        // *Enable bomb ui* 
    }

    public bool CanUseBomb()
    {
        return (bombCooldown <= 0 && bombCount > 0);
    }

    //Check indicated move position for obstacles and react accordingly 
    //  0 = Cannot move
    //  1 = Safe to move
    //  2 = Destroy item1
    //  3 = Destroy item2
    //  4 = Destroy both item1 and item2
    public int CheckCollision(int item1X, int item1Y, int item2X, int item2Y)
    {
        //print("X1: " + item1X + " Y1: " + item1Y + " X2: " + item2X + " Y2: " + item2Y);
        //If out of bounds: don't move 
        if (item2X < 0 || item2X > board_size - 1 || item2Y < 0 || item2Y > board_size - 1)
        {
            return 0;
        }
        //If Player is trying to move:
        if (gameBoard[item1X, item1Y] == null) {
            //print("Null tried to move!");
            return 0;
        }


        if (gameBoard[item1X, item1Y].type == "player" && !(gameBoard[item2X, item2Y] == null))
        {
            //If moving into a Wall: don't
            if (gameBoard[item2X, item2Y].type == "wall")
            {
                return 0;
            }
            //If moving into a Robot or Pit: die
            else if ((gameBoard[item2X, item2Y].type == "robot") || (gameBoard[item2X, item2Y].type == "pit"))
            {
                return 2;
            }
            //If moving into an Electric Fence: die and destroy the fence
            else if (gameBoard[item2X, item2Y].type == "electric_fence")
            {
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 4;
            }
        }
        //If a Robot is trying to move:
        else if (gameBoard[item1X, item1Y].type == "robot" && !(gameBoard[item2X, item2Y] == null))
        {
            //If moving into a Wall or another Robot: don't 
            if (gameBoard[item2X, item2Y].type == "wall" || gameBoard[item2X, item2Y].type == "robot")
            {
                return 0;
            }
            //If moving into a Pit: die
            else if (gameBoard[item2X, item2Y].type == "pit" || gameBoard[item2X, item2Y].type == "bomb")
            {
                return 2;
            }
            //If moving into the Player: kill them
            else if (gameBoard[item2X, item2Y].type == "player")
            {
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 3;
            }
            //If moving into an Electric Fence, die and destroy the fence
            else if (gameBoard[item2X, item2Y].type == "electric_fence")
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
        if (!(gameBoard[x, y] == null))
        {
            if (gameBoard[x, y].type == "wall")
            {
                return 0;
            }
            else if (gameBoard[x, y].type == "pit")
            {
                return 0;
            }
            else if (gameBoard[x, y].type == "electric_fence")
            {
                return 0;
            }
            else if (gameBoard[x, y].type == "player")
            {
                return 0;
            }
            //If spawned on robot, destroy it
            else if (gameBoard[x, y].type == "robot")
            {
                gameBoard[x, y].DestroyItem();
            }
        }
        return 1;
    }

    private void PrintBoard()
    {
        string board = "";
        for (int i = 0; i < board_size; i++)
        {
            for (int e = 0; e < board_size; e++)
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

    //Wasn't working so is retired for now
    public void CheckForRobots()
    {
        if (!GameObject.FindGameObjectWithTag("Robot"))
        {
            Debug.Log("All gone!");
            GenerateLevel();
            //LoadNextLevel();
        }
    }

    //Add to robot count when robot is spawned
    public void AddRobotCount()
    {
        robotCount++;
    }

    //Subtract from robot count when robot is destroyed and end level if no more robots
    public void SubtractRobotCount()
    {
        robotCount--;
        if (robotCount == 0)
        {
            Debug.Log("All gone!");
            GenerateLevel();
        }
    }


    private void StrToLevel(string my_str) {
        int counter = 0;
        for (int i = 0; i < board_size; i++)
        {
            for (int j = 0; j < board_size; j++)
            {
                if (!(gameBoard[j, i] == null))
                {
                    gameBoard[j, i].DestroyItem();
                }

                if (my_str[counter] == 'w') {
                    PlaceBoardItem(wall, j, i);
                }
                else if (my_str[counter] == 'p')
                {
                    PlaceBoardItem(pit, j, i);
                }
                else if (my_str[counter] == 'f')
                {
                    PlaceBoardItem(fence, j, i);
                }
                else if (my_str[counter] == 'r')
                {
                    PlaceBoardItem(robot, j, i);
                }
                else if (my_str[counter] == 'P')
                {
                    PlaceBoardItem(player, j, i);
                }
                counter += 1;
            }
        }


    }

    private void LoadNextLevel() {
        string my_str = level_list[0];
        level_list.RemoveAt(0);
        StrToLevel(my_str);    
    }

    private void ClearBoard() {
        for (int i = 0; i < board_size; i++) {
            for (int j = 0; j < board_size; j++)
            {
                if (!(gameBoard[j, i] == null))
                {
                    gameBoard[j, i].DestroyItem();
                }
            }
        }
    }


    private void GenerateLevel() {
        //Board stuff
        ClearBoard();
        PlaceItems(pit, 3);
        PlaceItems(wall, 3);
        PlaceItems(fence, 3);
        PlaceItems(robot, 2);
        PlaceItems(player, 1);
        //Bomb stuff
        AddBomb();
    }

    private void PlaceItems(BoardItem item, int count) {
        int index = 0;
        while (index < count)
        {
            int randrow = UnityEngine.Random.Range(1, board_size);
            int randcol = UnityEngine.Random.Range(1, board_size);
            if (gameBoard[randrow, randcol] == null)
            {
                PlaceBoardItem(item, randrow, randcol);
                index++;
            }
        }
    }
}
