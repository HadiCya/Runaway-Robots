using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    public int xPos;
    public int yPos;
    public string type = "empty";

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
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.ClearPosition(xPos, yPos);
        Debug.Log("Destroying: " + name + " [" + xPos + "," + yPos + "]");

        Destroy(gameObject);
    }
}
