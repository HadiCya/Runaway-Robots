using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    public int xPos;
    public int yPos;

    protected GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Destroy this item when another item collides with it
    public void DestroyItem()
    {
        Destroy(gameObject);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Debug.Log("Destroying: " + name + " [" + xPos + "," + yPos + "]");
        gameManager.ClearPosition(xPos, yPos);
    }
}
