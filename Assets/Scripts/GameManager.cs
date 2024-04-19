using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public enum SoundEffect
{
    cancelSound,
    deathSound,
    electricSound,
    tickSound,
    bombSound
}
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
    private int board_size = 10;

    private int robotCount = 0;
    public static int robotsDefeated;
    public static int levelCount;

    //Bomb stuff
    public static int bombCount = 0;
    private float bombCooldown = 0f;
    private readonly float bombInterval = 0.15f;
    private UnityEngine.UI.Image bombUiImage;
    private TextMeshProUGUI bombCountText;
    private TextMeshProUGUI levelCompleteText;

    private GameObject joystick;
    private GameObject buttons;

    public bool playerMovementDisabled = false;

    public GameObject audioGameObject;
    public GameObject musicGameObject;
    //Index 0: cancelSound; 1: deathSound; 2: electricSound; 3: tickSound; 4: bombSound
    private AudioSource[] audioSources;
    private Dictionary<SoundEffect, AudioSource> soundEffectDictionary = new();
    public MusicSpeedController musicSpeedController;

    //Board size stuff
    public GameObject grid;
    private readonly float gridPosAmount = 0.555f;
    private readonly float gridScaleAmount = 0.111f;
    public Camera cam;
    private readonly float camPosAmount = 0.555f;
    private readonly float camSizeAmount = 0.665f;

    public GameObject respawnMenu;
    private bool respawnAvailable;
    private int respawnAtX;
    private int respawnAtY;

    public RewardedAdsButton rewardedAdsButton;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new BoardItem[board_size, board_size];
        PrintBoard();

        audioSources = audioGameObject.GetComponents<AudioSource>();
        InitializeSoundEffects();

        rewardedAdsButton = GameObject.Find("RewardedAds").GetComponent<RewardedAdsButton>();

        joystick = GameObject.Find("Joystick");
        buttons = GameObject.Find("ButtonHolder");
        if (PlayerPrefs.HasKey("mobileControls"))
        {
            if (string.Equals(PlayerPrefs.GetString("mobileControls"), "joystick"))
            {
                buttons.SetActive(false);
            }
            else
            {
                joystick.SetActive(false);
            }
        }
        else
        {
            joystick.SetActive(false);
        }

#if PLATFORM_STANDALONE_WIN
        buttons.SetActive(false);
        joystick.SetActive(false);
#endif

        bombUiImage = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        bombCountText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        levelCompleteText = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        AddBomb();
        bombCount = 0;
        robotCount = 0;
        robotsDefeated = 0;
        levelCount = 0;
        respawnAvailable = true;
        respawnMenu.SetActive(false);
        GenerateLevel();
    }

    private void InitializeSoundEffects()
    {
        SoundEffect[] soundEffects = (SoundEffect[])System.Enum.GetValues(typeof(SoundEffect));
        for (int i = 0; i < soundEffects.Length; i++)
        {
            soundEffectDictionary[soundEffects[i]] = audioSources[i];
            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                audioSources[i].volume = PlayerPrefs.GetFloat("sfxVolume");
            }
            else
            {
                audioSources[i].volume = 1;
            }
        }

        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            musicGameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("bgmVolume");
        }
        else
        {
            musicGameObject.GetComponent<AudioSource>().volume = 1;
        }
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

    //Check for robots adjacent to player and destroy them
    public void SpawnBombs(int x, int y)
    {
        PlaySound(SoundEffect.bombSound);
        //Top-left
        if (x - 1 >= 0 && y - 1 >= 0 && gameBoard[x - 1, y - 1] != null && gameBoard[x - 1, y - 1].type == "robot")
        {
            gameBoard[x - 1, y - 1].DestroyItem();
        }
        //Top-middle
        if (y - 1 >= 0 && gameBoard[x, y - 1] != null && gameBoard[x, y - 1].type == "robot")
        {
            gameBoard[x, y - 1].DestroyItem();
        }
        //Top-right
        if (x + 1 <= board_size - 1 && y - 1 >= 0 && gameBoard[x + 1, y - 1] != null && gameBoard[x + 1, y - 1].type == "robot")
        {
            gameBoard[x + 1, y - 1].DestroyItem();
        }
        //Middle-Left
        if (x - 1 >= 0 && gameBoard[x - 1, y] != null && gameBoard[x - 1, y].type == "robot")
        {
            gameBoard[x - 1, y].DestroyItem();
        }
        //Middle-Right
        if (x + 1 <= board_size - 1 && gameBoard[x + 1, y] != null && gameBoard[x + 1, y].type == "robot")
        {
            gameBoard[x + 1, y].DestroyItem();
        }
        //Bottom-Left
        if (x - 1 >= 0 && y + 1 <= board_size - 1 && gameBoard[x - 1, y + 1] != null && gameBoard[x - 1, y + 1].type == "robot")
        {
            gameBoard[x - 1, y + 1].DestroyItem();
        }
        //Bottom-Middle
        if (y + 1 <= board_size - 1 && gameBoard[x, y + 1] != null && gameBoard[x, y + 1].type == "robot")
        {
            gameBoard[x, y + 1].DestroyItem();
        }
        //Bottom-Right
        if (x + 1 <= board_size - 1 && y + 1 <= board_size - 1 && gameBoard[x + 1, y + 1] != null && gameBoard[x + 1, y + 1].type == "robot")
        {
            gameBoard[x + 1, y + 1].DestroyItem();
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
            PlaySound(SoundEffect.cancelSound);
            return 0;
        }
        
        if (gameBoard[item1X, item1Y] == null) {
            //print("Null tried to move!");
            return 0;
        }

        //If Player is trying to move:
        if (gameBoard[item1X, item1Y].type == "player" && !(gameBoard[item2X, item2Y] == null))
        {
            //If moving into a Wall: don't
            if (gameBoard[item2X, item2Y].type == "wall")
            {
                PlaySound(SoundEffect.cancelSound);
                return 0;
            }
            //If moving into a Robot or Pit: die
            else if ((gameBoard[item2X, item2Y].type == "robot") || (gameBoard[item2X, item2Y].type == "pit"))
            {

                KillPlayer(item1X, item1Y);
                return 2;
            }
            //If moving into an Electric Fence: die and destroy the fence
            else if (gameBoard[item2X, item2Y].type == "electric_fence")
            {
                PlaySound(SoundEffect.electricSound);               
                gameBoard[item2X, item2Y].DestroyItem();
              
                KillPlayer(item1X, item1Y);
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
                
                gameBoard[item2X, item2Y].DestroyItem();                
                KillPlayer(item2X, item2Y);

                return 3;
            }
            //If moving into an Electric Fence, die and destroy the fence
            else if (gameBoard[item2X, item2Y].type == "electric_fence")
            {
                PlaySound(SoundEffect.electricSound);
                //Destroy other item collided with
                gameBoard[item2X, item2Y].DestroyItem();
                return 4;
            }
        }
        else if (gameBoard[item1X, item1Y].type == "robot" && (gameBoard[item2X, item2Y] == null))
        {
            PlaySound(SoundEffect.tickSound);
        }
        //No obstacle to collide with, so move along
        return 1;
    }

    // Ends the Game
    private void KillPlayer(int respawnX, int respawnY) {
        PlaySound(SoundEffect.deathSound);
        respawnAtX = respawnX;
        respawnAtY = respawnY;
        Invoke(nameof(DetermineEndPath), 1);
    }


    //ALSO DEPRECATED
    //Check if the indicated position is empty (for bombs)
    //  0 == not empty
    //  1 == empty or robot
    public int CheckIfEmpty(int x, int y)
    {
        PlaySound(SoundEffect.bombSound);
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

    //Add to robot count when robot is spawned
    public void AddRobotCount()
    {
        robotCount++;
    }

    //Subtract from robot count when robot is destroyed and end level if no more robots
    public void SubtractRobotCount()
    {
        robotCount--;
        robotsDefeated++;
        if (robotCount == 0)
        {
            //Debug.Log("All gone!");
            playerMovementDisabled = true;
            levelCompleteText.gameObject.SetActive(true);
            SpeedUpMusic();
            Invoke(nameof(GenerateLevel), 0.5f);
        }
    }

    private void SpeedUpMusic()
    {
        if(musicSpeedController != null)
        {
            musicSpeedController.UpdateMusicSpeed(levelCount);
        }
        else
        {
            print("ERROR: No MusicSpeedController");
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
                    //Debug.Log("Level end destroyed: " + gameBoard[j, i].type);
                    gameBoard[j, i].DestroyItem();
                }
            }
        }
    }

    private void GenerateLevel()
    {
        ClearBoard();
        if (levelCount > 0)
        {
            ChangeBoardSize(1);
        }

        //Difficutly stuff
        int num_of_robots = (levelCount * 2) + 1;
        int num_of_fences = (levelCount * 2) + 3;
        int num_of_walls = (levelCount * 5);
        int num_of_pits = 2;
        //Board stuff
        playerMovementDisabled = false;
        levelCompleteText.gameObject.SetActive(false);

        int[,] levelMap = new int[board_size, board_size];
        for (int i = 0; i < board_size; i++)
        {
            for (int j = 0; j < board_size; j++)
            {
                levelMap[i, j] = 0;
            }
        }

        PlaceItems(player, 1, levelMap);
        PlaceItems(pit, num_of_pits, levelMap);
        PlaceItems(wall, num_of_walls, levelMap);
        PlaceItems(fence, num_of_fences, levelMap);
        PlaceItems(robot, num_of_robots, levelMap);

        //Bomb stuff
        AddBomb();
        //Other
        levelCount++;
    }

    private void PlaceItems(BoardItem item, int count, int[,] levelMap)
    {
        int index = 0;
        int spin_count = 0;
        while (index < count)
        {
            spin_count += 1;
            if (spin_count > 5000)
            {
                print("ERROR IN LEVEL GENERATION");
                break;
            }
            int randrow = UnityEngine.Random.Range(0, board_size);
            int randcol = UnityEngine.Random.Range(0, board_size);
            if (gameBoard[randrow, randcol] == null && (item.type != "robot" || !PlayerInProximity(randrow, randcol, 3)) && (item.type == "robot" || CheckIfLegal(randrow, randcol, levelMap)))
            {
                PlaceBoardItem(item, randrow, randcol);
                if (item.type != "robot")
                {
                    levelMap[randrow, randcol] = 1;
                }
                index++;
            }
        }
    }

    // Checks if an item can be placed here without causing a potential softlock.
    private bool CheckIfLegal(int row, int col, int[,] levelMap)
    {
        int totalZeros = 0;
        (int, int) firstZero = (0, 0);

        for (int i = 0; i < board_size; i++)
        {
            for (int j = 0; j < board_size; j++)
            {
                if (levelMap[i, j] == 0 && (i != row || j != col))
                {
                    totalZeros++;
                    firstZero = (i, j);
                }
            }
        }

        levelMap[row, col] = -1;
        int areaZeroCount = DFS(firstZero.Item1, firstZero.Item2, levelMap);
        ClearMapNegatives(levelMap);

        //print("total zeros: " +  totalZeros);
        //print("area zeros: " + areaZeroCount);
        return totalZeros == areaZeroCount;
    }

    // sets all -1s in the levelMap to 0 so the levelMap can be used in the next check.
    private void ClearMapNegatives(int[,] levelMap)
    {
        for (int i = 0; i < board_size; i++)
        {
            for (int j = 0; j < board_size; j++)
            {
                if (levelMap[i, j] == -1)
                {
                    levelMap[i, j] = 0;
                }
            }
        }
    }

    // Counts all connected zeros to a given cell.
    private int DFS(int row, int col, int[,] levelMap)
    {
        int zeroCount = 0;

        if (row < 0 || row >= board_size || col < 0 || col >= board_size || levelMap[row, col] != 0)
        {
            return 0;
        }

        levelMap[row, col] = -1;
        zeroCount += 1;

        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = col - 1; j <= col + 1; j++)
            {
                if (j == col || i == row)
                {
                    zeroCount += DFS(i, j, levelMap);
                }
            }
        }

        return zeroCount;

    }

    // Return True if the player is within range number of squares from the given cords. False otherwise
    private bool PlayerInProximity(int x, int y, int range) {
        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                if (x + i < board_size && x + i >= 0 && y + j < board_size && y + j >= 0) {
                    if (gameBoard[x + i, y + j] != null && gameBoard[x + i, y + j].type == "player")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //Increase/Decrease board size by specified amount
    private void ChangeBoardSize(int amount)
    {
        //Increase board size and reset array
        board_size += amount;
        gameBoard = new BoardItem[board_size, board_size];

        //Change gameboard object
        grid.transform.position = new Vector3(grid.transform.position.x + (amount * gridPosAmount), grid.transform.position.y - (amount * gridPosAmount), 0);
        grid.transform.localScale = new Vector3(grid.transform.localScale.x + (amount * gridScaleAmount), 1, grid.transform.localScale.z + (amount * gridScaleAmount));
        grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(board_size, board_size);

        //Move camera
        cam.transform.position = new Vector3(cam.transform.position.x + (amount * camPosAmount), cam.transform.position.y - (amount * camPosAmount), cam.transform.position.z);
        cam.orthographicSize += (amount * camSizeAmount);
    }

    void PlaySound(SoundEffect effect)
    {
        // Try to get the AudioSource from the dictionary and play it if found
        if (soundEffectDictionary.TryGetValue(effect, out AudioSource source))
        {
            source.Play();
        }
        else
        {
            Debug.LogError($"No AudioSource assigned for the sound effect: {effect}");
        }
    }

    private void DetermineEndPath()
    {
#if PLATFORM_STANDALONE_WIN
        respawnAvailable = false;
#elif UNITY_EDITOR
        respawnAvailable = false;
#endif

        float rng = UnityEngine.Random.Range(0,100);

        if (respawnAvailable && rng <= 40)
        {
            musicGameObject.GetComponent<AudioSource>().volume = 0;
            respawnAvailable = false;
            respawnMenu.SetActive(true);
            rewardedAdsButton.LoadAd();
        }
        else
        {
            Invoke(nameof(LoadEndScreen), 2);
        }
    }

    private void RespawnPlayer()
    {
        SpawnBombs(respawnAtX, respawnAtY);
        AddBomb();
        if (!(gameBoard[respawnAtX, respawnAtY] == null))
        {
            gameBoard[respawnAtX, respawnAtY].DestroyItem();
        }
        PlaceBoardItem(player, respawnAtX, respawnAtY);
        GameObject.FindObjectOfType<Player>().BombEffect();
        ResetRobots();
    }

    private void ResetRobots()
    {
        Robot[] robots = GameObject.FindObjectsOfType<Robot>();
        foreach (Robot robot in robots)
        {
            robot.ResetRobot();
        }
    }

    //Watch Ad button calls this
    public void ContinuePlaying()
    {
        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            musicGameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("bgmVolume");
        }
        else
        {
            musicGameObject.GetComponent<AudioSource>().volume = 1;
        }
        respawnMenu.SetActive(false);
        Invoke(nameof(RespawnPlayer), 1);
    }

    //Skip Ad button calls this
    public void LoadEndScreen()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
