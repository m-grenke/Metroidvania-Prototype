using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Form : MonoBehaviour
{
    public Mask mask;
    public Material material;
    public Vector3 size;
    public Vector2 wallJumpOff;

    public virtual void OnJumpInputDown()
    {

    }
    public virtual void OnJumpInputUp()
    {

    }

    public void WallJumpOff(int wallDirection, ref Vector2 velocity)
    {
        velocity.x = -wallDirection * wallJumpOff.x;
        velocity.y = wallJumpOff.y;
    }
}
