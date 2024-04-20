using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Robot : Moveable
{
    
    private GameObject player;
    private float xDistance;
    private float yDistance;
    private float moveInterval = 0.75f;
    private bool canMove = false;

    // Start is called before the first frame update
    new void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        moveInterval = 0.75f - GameManager.levelCount * 0.015f;
        if (moveInterval < 0.18f)
        {
            moveInterval = 0.18f;
        }
        base.Start();
        //ResetRobot();
        StartCoroutine(MoveRobotCoroutine());
        gameManager.AddRobotCount();
    }

    //Move Robot every X seconds
    IEnumerator MoveRobotCoroutine()
    {
        while (true)
        {
            if (canMove)
            {
                yield return new WaitForSeconds(moveInterval);
                //If player exists, move towards it
                if (canMove)
                {
                    if (player = GameObject.FindGameObjectWithTag("Player"))
                    {
                        FindMoveDirection();
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    //Find path directly towards player
    private void FindMoveDirection()
    {
        Vector3 originalPos = transform.position;

        //Get X and Y distances from player
        xDistance = (int)(player.transform.position.x - transform.position.x);
        yDistance = (int)(player.transform.position.y - transform.position.y);
        //Correct distances because sometimes they become 0.000000000000000001 because floats
        if ((xDistance > 0 && xDistance < 1) || (xDistance < 0 && xDistance > -1))
        {
            xDistance = 0;
        }
        if ((yDistance > 0 && yDistance < 1) || (yDistance < 0 && yDistance > -1))
        {
            yDistance = 0;
        }
        //If X distance is greater, try moving on X first
        if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
        {
            MoveRobotX();
        }
        //Else if Y distance is greater, try moving on Y first
        else if (Mathf.Abs(yDistance) > Mathf.Abs(xDistance))
        {
            MoveRobotY();
        }
        //Else if equal distances or didn't move, try moving on X again first
        if (transform.position == originalPos)
        {
            MoveRobotX();
        }
        //If still hasn't moved, try moving on Y again
        if (transform.position == originalPos)
        {
            MoveRobotY();
        }

        //Update distance again for testing purposes
        //xDistance = player.transform.position.x - transform.position.x;
        //yDistance = player.transform.position.y - transform.position.y;
    }

    //Move Robot one space toward player on x axis
    private void MoveRobotX()
    {
        if (xDistance > 0)
        {
            MoveItem(1, 0);
            return;
        }
        else if (xDistance < 0)
        {
            MoveItem(-1, 0);
            return;
        }
    }

    //Move Robot one space toward player on x axis
    private void MoveRobotY()
    {
        if (yDistance > 0)
        {
            MoveItem(0, -1);
            return;
        }
        else if (yDistance < 0)
        {
            MoveItem(0, 1);
            return;
        }
    }

    public void ActivateRobot()
    {
        canMove = true;
        StartCoroutine(MoveRobotCoroutine());
    }

    public void ResetRobot()
    {        
        AddDelay();
    }

    public void AddDelay()
    {
        canMove = false;
        Invoke(nameof(RemoveDelay), 1);
    }

    public void RemoveDelay()
    {
        canMove = true;
        StartCoroutine(MoveRobotCoroutine());
    }

    public override void DestroyItem()
    {
        base.DestroyItem();
        gameManager.SubtractRobotCount();
    }
}
