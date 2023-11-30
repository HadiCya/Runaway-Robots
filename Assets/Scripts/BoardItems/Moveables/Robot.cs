using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Robot : Moveable
{
    private GameObject player;
    private float xDistance;
    private float yDistance;
    private float moveInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(MoveRobotCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Move Robot every X seconds
    IEnumerator MoveRobotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            //If player exists, move towards it
            if (player = GameObject.FindGameObjectWithTag("Player"))
            {
                FindMoveDirection();
            }
            //If player no longer exists, stop moving
            else
            {
                StopCoroutine(MoveRobotCoroutine());
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
}
