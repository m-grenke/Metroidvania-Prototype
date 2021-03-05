using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beast : Form
{
    public Vector2 wallClimb;

    public override void OnJumpInputDown()
    {
        base.OnJumpInputDown();
    }

    public override void OnJumpInputUp()
    {
        base.OnJumpInputUp();
    }

    public void WallClimb(int wallDirection, ref Vector2 velocity)
    {
        velocity.x = -wallDirection * wallClimb.x;
        velocity.y = wallClimb.y;
    }
}
