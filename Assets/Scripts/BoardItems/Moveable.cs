using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : BoardItem
{
    protected float moveDistance = 1.11f;
    protected Vector2 moveDir;
    protected int collisionResult;
    [SerializeField] private ParticleSystem walkParticles;
    [SerializeField] private ParticleSystem deathParticles;
    private GameObject model;
    // Start is called before the first frame update
    public void Start()
    {
        model = transform.GetChild(0).gameObject;
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

            WalkEffect(xMove, yMove);
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
    private void WalkEffect(int xMove, int yMove)
    {
        if (xMove == 1)
        {
            model.transform.eulerAngles = new Vector3(90, -90, 90);
        }
        if (xMove == -1)
        {
            model.transform.eulerAngles = new Vector3(-90, -90, 90);
        }
        if (yMove == 1)
        {
            model.transform.eulerAngles = new Vector3(0, -90, 90);
        }
        if (yMove == -1)
        {
            model.transform.eulerAngles = new Vector3(180, -90, 90);
        }
        Instantiate(walkParticles, model.transform);

    }
    private void DeathEffect()
    {
        ParticleSystem myPSystem = Instantiate(deathParticles, transform);
        myPSystem.transform.SetParent(transform.parent);
    }
    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        DeathEffect();
    }
}
