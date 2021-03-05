using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{
    MeshRenderer meshRenderer;
    SpriteRenderer spriteRenderer;

    public Mask currentMask;
    public Form currentForm;
    public Phantom phantom;
    public Beast beast;

    public Transform phantomMask;
    public Transform beastMask;

    //Acquired Abilities
    
    public bool hasGrappleHook;
    bool useGrapplePhysics = false;
    Vector2 grappleHookDir;
    Transform anchorTransform;
    Vector2 deltaHit;
    Vector2 grappleHitPoint
    {
        get
        {
            return (Vector2)anchorTransform.position - deltaHit;
        }
    }
    [HideInInspector]
    public bool aIsHeld = false;
    public float offGrappleImpulse;
    public float grappleHookDistance;
    public float retractTime = 0.5f;
    public float minGrappleLength = 1.5f;

    DistanceJoint2D distanceJoint;
    LineRenderer line;

    public bool hasWallJump;
    
    
    float timeToWallUnstick;

    public float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    Rigidbody2D rb2d;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    EquipmentManager equipmentManager;


    // Use this for initialization
    void Start ()
    { 
        interactableLayer = LayerMask.NameToLayer("Interactable");
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateEquipment;

        currentForm = phantom;
        currentMask = currentForm.mask;
        controller = GetComponent<Controller2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = currentMask.gameSprite;
        controller.collider.size = currentForm.size;
        meshRenderer.transform.localScale = currentForm.size;



        rb2d = GetComponent<Rigidbody2D>();

        phantom = GetComponent<Phantom>();
        beast = GetComponent<Beast>();


        controller.CalculateRaySpacing();

        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        rb2d.gravityScale = gravity;
    }

    void UpdateEquipment(Equipment newItem, Equipment oldItem)
    {
        if(newItem.equipmentSlot == EquipmentSlot.Head)
        {
            if(newItem is Mask)
            {
                currentMask = (Mask)newItem;
                print(currentMask.form);
                if(currentMask.form == Transformation.Phantom)
                {
                    currentForm = phantom;
                }
                else
                {
                    currentForm = beast;
                }
                
                FormSwitch(currentForm);
            }
        }
    }

    void Update()
    {
        gravity = -(2 * currentMask.maxJumpHeight) / Mathf.Pow(currentMask.timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * currentMask.timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * currentMask.minJumpHeight);

        rb2d.simulated = useGrapplePhysics;
        if (!useGrapplePhysics)
        {
            CalculateVelocity();
            HandleWallSliding();

            controller.Move(velocity * Time.deltaTime, directionalInput);

            if (controller.collisions.above || controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = 0;
                }
            }
        }
        else
        {
            float angle = Vector2.Angle(grappleHitPoint - (Vector2)transform.position, directionalInput);

            line.SetPosition(0, transform.position);
            if (anchorTransform)
                distanceJoint.connectedAnchor = (Vector2)anchorTransform.position - deltaHit;
            line.SetPosition(1, distanceJoint.connectedAnchor);
            if (angle < 45)//if you point the control stick within 45 degress of the grapple hit point, begin to retract
            {
                if (distanceJoint.distance > minGrappleLength)
                {
                    RetractGrapple();
                }
                else
                {
                    distanceJoint.enabled = false;
                    line.enabled = false;
                    useGrapplePhysics = false;
                    velocity = rb2d.velocity;
                }
            }
        }

        Focus();
    }

    public Interactable focus;
    Interactable newFocus;
    LayerMask interactableLayer;
    public float focusRadius = 9f;
    void Focus()
    {
        float minDistance = float.MaxValue;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, focusRadius, ~interactableLayer);
        newFocus = null;
        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(hit.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                newFocus = hit.GetComponent<Interactable>();
            }
        }
        if(focus != newFocus)
        {
            if (focus)
            {
                focus.OnDefocused();
            }

            if (newFocus != null)
            {
                focus = newFocus;
                focus.OnFocused(transform);
            }
        }
    }

    void RetractGrapple()
    {
        float retractSpeed = distanceJoint.distance / retractTime;
        distanceJoint.distance -= retractSpeed * Time.deltaTime; //RETRACT hook like a hookshot
    }
    void FormSwitch(Form switchTo)
    {
        Vector3 originalPos = transform.position;
        if (switchTo == beast)
        {
            transform.position += new Vector3(0f, (beast.size.y - phantom.size.y) / 2f, 0);
            float phantomFoot = transform.position.y - beast.size.y / 2f;

            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, (Vector2)beast.size - (Vector2.one * 2 * RaycastController.skinWidth), 0f, Vector2.zero, controller.collisionMask);
            Vector2 avgPush = Vector2.zero, avgPoint = Vector2.zero;
            int numHits = 0;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag != "Player")
                {
                    avgPoint += hit.point - (Vector2)transform.position;
                    print("point: " + (hit.point - (Vector2)transform.position));
                }
                else
                {
                    numHits--;
                }
                //check all hits, avg of (hit normals * distance) and beast size to push out
                //if()
            }
            numHits += hits.Length;
            float distance = Vector2.Distance(avgPoint, transform.position);
            if (numHits >= 1)
            {
                avgPush = -avgPoint / numHits;
                print(avgPush + " ; num hits: " + numHits);
            }
            transform.position += (Vector3)avgPush;
            RaycastHit2D[] secondHits = Physics2D.BoxCastAll(transform.position, (Vector2)beast.size - (Vector2.one * 2 * RaycastController.skinWidth), 0f, Vector2.zero, controller.collisionMask);
            int count = 0;
            foreach (RaycastHit2D hit in secondHits)
            {
                if (hit.collider.tag != "Player")
                {
                    print(hit.collider.gameObject.name);
                    count++;
                }
            }

            if (count == 0)
            {
                currentForm = beast;
            }
            else
            {
                transform.position = originalPos;
            }
        }

        else
        {
            //all other forms are smaller in every way than beast form so no need to check for space
            transform.position -= new Vector3(0f, (beast.size.y - phantom.size.y) / 2, 0);
            currentForm = phantom;
        }

        CameraFollow.Instance.AdjustFocusAreaSize(new Vector2(currentForm.size.x/2 + (currentMask.moveSpeed / 2f), currentForm.size.y + (currentMask.maxJumpHeight)));
        meshRenderer.material = currentForm.material;

        Vector3 deltaPos = Vector2.zero;
        
        transform.position += deltaPos;
        controller.collider.size = currentForm.size;
        meshRenderer.transform.localScale = currentForm.size;
        spriteRenderer.sprite = currentMask.gameSprite;
        controller.CalculateRaySpacing();
    }

    public void OnActionInputDown()
    {
        if (hasGrappleHook)
        {
            rb2d.gravityScale = gravity;
            grappleHookDir = directionalInput;
            Vector2 rayOrigin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, grappleHookDir, grappleHookDistance);

            if (hit.collider != null)
            {
                //TO DO: animate grapple hook fire over time

                if (hit.distance > minGrappleLength)
                {
                    useGrapplePhysics = true;
                    rb2d.velocity = velocity;
                    distanceJoint.enabled = true;
                    Rigidbody2D hitBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                    if (hitBody != null)
                    {
                        distanceJoint.connectedBody = hitBody;
                    }
                    distanceJoint.distance = hit.distance;
                    RetractGrapple();
                    anchorTransform = hit.collider.transform;
                    deltaHit = (Vector2)anchorTransform.position - hit.point;
                    distanceJoint.connectedAnchor = (Vector2)anchorTransform.position - deltaHit;

                    line.enabled = true;
                }
            }
        }
    }

    public void OnActionHeld()
    {
       
    }

    public void OnActionInputUp()
    {
        if (useGrapplePhysics)
        {
            distanceJoint.enabled = false;
            line.enabled = false;
            useGrapplePhysics = false;
            velocity = rb2d.velocity;
            velocity.y = Mathf.Abs(velocity.y) * offGrappleImpulse;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (!wallSliding)
        {
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))// not jumping against maxslope
                    {
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;
                }
            }
        }
        else //wallsliding
        { 
            if (hasWallJump)
            {
                if (wallDirX == directionalInput.x)
                {
                    if (currentForm == phantom)
                    {
                        currentForm.WallJumpOff(wallDirX, ref velocity);
                    }
                    else if (currentForm == beast)
                    {
                        beast.WallClimb(wallDirX, ref velocity);
                    }
                }
                else if (directionalInput.x != 0)
                {
                    if (currentForm == phantom)
                    {
                        phantom.WallLeap(wallDirX, ref velocity);
                    }
                    else if (currentForm == beast)
                    {
                        currentForm.WallJumpOff(wallDirX, ref velocity);
                    }
                }
                else if (directionalInput.x == 0)
                {
                    if (currentForm == phantom)
                    {
                        phantom.WallLeap(wallDirX, ref velocity);
                    }
                    else if (currentForm == beast)
                    {
                        beast.WallClimb(wallDirX, ref velocity);
                    }
                }
                else
                {
                    currentForm.WallJumpOff(wallDirX, ref velocity);
                }
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -currentMask.wallSlideSpeedMax)
            {
                velocity.y = -currentMask.wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocity.x = 0;
                velocityXSmoothing = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = currentMask.wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = currentMask.wallStickTime;
            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = (directionalInput.x * currentMask.moveSpeed);
        

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below) ? currentMask.accelerationTimeGrounded : currentMask.accelerationTimeAirborne);


        velocity.y += gravity * Time.deltaTime;
    }
}
