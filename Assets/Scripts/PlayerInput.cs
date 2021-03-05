using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
	
    // Use this for initialization
	void Start ()
    {
        player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if(Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }
        if(Input.GetButton("Jump"))
        {
            player.aIsHeld = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            player.aIsHeld = false;
            player.OnJumpInputUp();
        }

        if(Input.GetButtonDown("Grapple"))
        {
            if (player.currentForm == player.phantom)
            {
                player.OnActionInputDown();
            }
        }
        if(Input.GetButton("Grapple"))
        {
            if (player.currentForm == player.phantom)
            {
                player.OnActionHeld();
            }
        }
        if(Input.GetButtonUp("Grapple"))
        {
            if (player.currentForm == player.phantom)
            {
                player.OnActionInputUp();
            }
        }

        if (Input.GetButtonDown("Transform"))
        {
            //no longer button transformation at the moment
        }
    }
}
