using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mask", menuName ="Inventory/Mask")]
public class Mask : Equipment
{
    public Transformation form;

    public Sprite gameSprite;

    public float wallSlideSpeedMax;
    public float wallStickTime;

    public float maxJumpHeight;
    public float minJumpHeight;
    public float timeToJumpApex = 0.2f;

    public float accelerationTimeAirborne;
    public float accelerationTimeGrounded;
    public float moveSpeed;    

}

public enum Transformation { Phantom, Beast}