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
        //NOTES
        // Attempts to spawn 8 bombs around player
        // Will not spawn bomb if space is occupied (wall, pit, electric fence, player)
        //  because it would override other item and i figured this was easier than setting the
        //  item back every time also player would be able to walk over bomb space whilst 
        //  exploding unless we decide to disable movement until bomb is gone
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
