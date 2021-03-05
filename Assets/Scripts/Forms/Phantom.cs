using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : Form
{
    public Vector2 wallLeap;

    public override void OnJumpInputDown()
    {
        base.OnJumpInputDown();
    }

    public override void OnJumpInputUp()
    {
        base.OnJumpInputUp();
    }
    
    public void WallLeap(int wallDirection, ref Vector2 velocity)
    {
        velocity.x = -wallDirection * wallLeap.x;
        velocity.y = wallLeap.y;
    }
}
