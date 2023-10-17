using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(MoveRobot());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Move Robot every X seconds
    IEnumerator MoveRobot()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            //If player exists, move towards it
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                FindMoveDirection();
            }
            //If player no longer exists, stop moving
            else
            {
                StopCoroutine(MoveRobot());
            }
        }
    }

    //Algorithm could be changed to prioritize direction furthest from player instead of X first
    //Or make it choose X or Y first randomly 

    //Find path directly towards player
    private void FindMoveDirection()
    {
        Vector3 originalPos = transform.position;
        //Try moving on X-axis first
        xDistance = player.transform.position.x - transform.position.x;
        //Correct xDistance because sometimes it becomes 0.000000000000000001 and stops working
        if ((xDistance > 0 && xDistance < 1) || (xDistance < 0 && xDistance > -1))
        {
            xDistance = 0;
        }
        if (xDistance > 0)
        {
            MoveItem(1, 0);
        }
        else if (xDistance < 0)
        {
            MoveItem(-1, 0);
        }
        //Try moving on Y-axis next if haven't moved yet
        if (originalPos == transform.position)
        {
            yDistance = player.transform.position.y - transform.position.y;
            if ((yDistance > 0 && yDistance < 1) || (yDistance < 0 && yDistance > -1))
            {
                yDistance = 0;
            }
            if (yDistance > 0)
            {
                MoveItem(0, -1);
            }
            else if (yDistance < 0)
            {
                MoveItem(0, 1);
            }
        }
    }
}
