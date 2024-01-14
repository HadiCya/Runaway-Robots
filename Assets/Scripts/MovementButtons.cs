using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementButtons : MonoBehaviour
{
    private Player player;
    public int x, y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindFirstObjectByType<Player>();
        }
    }

    public void MovePlayer()
    {
        if (player != null)
        {
            player.MoveFromButton(x, y);
        }
    }
}
