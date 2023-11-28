using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BoardItem
{
    private float lifetime = 0.5f;
    private float timeAlive = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Destroy after 0.5 seconds
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            DestroyItem();
        }
    }
}
