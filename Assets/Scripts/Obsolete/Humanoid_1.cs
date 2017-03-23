using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (CharacterController))]
public class Humanoid_1: MonoBehaviour {

    public InputData inputData;

    public float baseSpeed = 8.0f;
    public float baseJumpHeight = 2.0f;
    public float gravity = 20.0f;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
    public float maxSlideSlopeAngle = 65.0f;
    public float slopeSlideGraduationPower = 1.0f;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnFrictionlessObjects = false;
    public float maxFrictionToSlide = 0.5f;
    public float minFrictionToSlide = 0.0f;
    public float frictionlessSlideGraduationPower = 1.0f;

    public float slideSpeed = 12.0f;
 
    // If checked, then the player can change direction while in the air
    public bool airControl = false;
    public float airControlModifier = 0.8f;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
 
    // Player must be grounded for at least this many seconds before being able to jump again; set to 0 to allow bunny hopping
    public float jumpDelay = 0.05f;
 
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    //private float speed; //???
    private RaycastHit hit; //???
    private float fallStartLevel;
    private bool falling;
    //private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    //private bool playerControl = false;
    private float playerControlModifier = 0.0f;
    private float jumpTimer;
 
    void Start() {
        controller = this.GetComponent<CharacterController>();
        myTransform = this.transform;
        //speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        //slideLimit = controller.slopeLimit - .1f; //???
        jumpTimer = this.jumpDelay;
    }
 
    void FixedUpdate() {
        rayDistance = controller.height * .5f + controller.radius;

        if (grounded) {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
                if (Vector3.Angle(hit.normal, Vector3.up) >= controller.slopeLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) >= controller.slopeLimit)
                    sliding = true;
            }
 
            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling) {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert (fallStartLevel - myTransform.position.y);
            }
 
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ( (sliding && slideWhenOverSlopeLimit) /*|| (slideOnTaggedObjects && hit.collider.tag == "Slide")*/ ) {
                Vector3 hitNormal = hit.normal;
                //moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                //Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                //moveDirection *= slideSpeed;
                falling = true;
                playerControlModifier = 0.0f;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else {
                moveDirection = new Vector3(inputData.horizontalMotion.x, -antiBumpFactor, inputData.horizontalMotion.y);
                moveDirection = myTransform.TransformDirection(moveDirection) * baseSpeed;
                playerControlModifier = 1.0f;
            }
 
            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetButton("Jump"))
                jumpTimer+=Time.fixedDeltaTime;
            else if (jumpTimer >= jumpDelay) {
                float jumpSpeed = Mathf.Pow(Mathf.Abs(2.0f * this.gravity * this.baseJumpHeight), 0.5f);
                moveDirection.y = jumpSpeed;
                jumpTimer = 0.0f;
            }
        }
        else {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling) {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }
 
            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControlModifier>0.0f) {
                moveDirection.x = inputData.horizontalMotion.x * baseSpeed;
                moveDirection.z = inputData.horizontalMotion.y * baseSpeed;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }
 
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
 
        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }
 
    void Update () {
    }
 
    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance) {
        print ("Ouch! Fell " + fallDistance + " units!");   
    }
}