using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : BoardItem
{
    protected float moveDistance = 1.11f;
    protected Vector2 moveDir;
    protected int collisionResult;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Move item in indicated direction and check for collisions 
    protected virtual void MoveItem(int xMove, int yMove)
    {
        int xNew = xPos;
        int yNew = yPos;
        //Determine new position in array to move item
        if (xMove < 0)
        {
            xNew--;
            if (yMove < 0)
            {
                yNew--;
                moveDir = new Vector2(-moveDistance, moveDistance);
            }
            else if (yMove == 0)
            {
                moveDir = new Vector2(-moveDistance, 0);
            }
            else if (yMove > 0)
            {
                yNew++;
                moveDir = new Vector2(-moveDistance, -moveDistance);
            }
        }
        else if (xMove == 0)
        {
            if (yMove < 0)
            {
                yNew--;
                moveDir = new Vector2(0, moveDistance);
            }
            else if (yMove > 0)
            {
                yNew++;
                moveDir = new Vector2(0, -moveDistance);
            }
        }
        else if (xMove > 0)
        {
            xNew++;
            if (yMove < 0)
            {
                yNew--;
                moveDir = new Vector2(moveDistance, moveDistance);
            }
            else if (yMove == 0)
            {
                moveDir = new Vector2(moveDistance, 0);
            }
            else if (yMove > 0)
            {
                yNew++;
                moveDir = new Vector2(moveDistance, -moveDistance);
            }
        }
        //Check if item will collide after moving 
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        collisionResult = gameManager.CheckCollision(xPos, yPos, xNew, yNew);
        //Cannot move
        if (collisionResult == 0)
        {
            return;
        }
        //Safe to move
        else if (collisionResult == 1 || collisionResult == 3)
        {
            gameManager.ClearPosition(xPos, yPos);
            xPos = xNew;
            yPos = yNew;

            gameManager.UpdatePosition(this);
            transform.Translate(moveDir);
        }
        //Get destroyed
        else if (collisionResult == 2 || collisionResult == 4)
        {
            DestroyItem();
        }
    }
}
