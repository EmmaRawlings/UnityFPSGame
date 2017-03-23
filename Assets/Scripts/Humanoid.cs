using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class Humanoid : MonoBehaviour {
    [Header("Base Motion")]
	[SerializeField]
    [Tooltip("The base top speed of the Humanoid")]
	private float baseTopSpeed = 6.0f;
	[SerializeField]
    [Tooltip("The time, in seconds, it takes for the Humanoid to reach the top speed")]
	private float timeToTopSpeed = 0.0f; 
	[SerializeField]
    [Tooltip("The maximum angle a surface can have before it is no longer considered a floor for the Humanoid to run on")]
	private float maxRunableSlopeAngle = 42.0f; 
	[SerializeField]
    [Tooltip("[minRunableSurfaceFriction]")]
	private float minRunableSurfaceFriction = 0.3f;
    [SerializeField]
    [Tooltip("[maxSlidingSurfaceFriction]")]
    private float maxSlidingSurfaceFriction = 0.7f;
	[SerializeField]
    [Tooltip("[frictionSlidingPower]")]
	private float frictionSlidingPower = 4.0f;

    [Header("Air Strafe")]
    [SerializeField]
    [Tooltip("Whether or not the Humanoid can strafe in the air")]
    private bool enableAirStrafe = false; 
	[SerializeField]
    [Tooltip("The ratio that affects top movement speed when in the air")]
	private float airStrafeSpeedModifier = 0.5f; 
	[SerializeField]
    [Tooltip("The time, in seconds, it takes for the Humanoid to reach the top speed while it is in the air")]
	private float airStrafeTimeToTopSpeed = 0.4f; 

    [Header("Jumping")]
    [SerializeField]
    [Tooltip("The height of the Humanoid's jumps")]
    private float jumpHeight = 0.5f; 
    [SerializeField]
    [Tooltip("The delay, in seconds, between coming into contact with the ground and being able to jump again")]
    private float jumpDelay = 0.05f; 

    [Header("Ledge Grab")]
    [SerializeField]
    [Tooltip("Whether or not the Humanoid can climb ledges")]
    private bool enableLedgeGrab = false; 
    [SerializeField]
    [Tooltip("The time, in seconds, it takes for the Humanoid to climb a ledge")]
    private float timeToLedgeGrab = 0.3f; 
    [SerializeField]
    [Tooltip("The maximum distance that a ledge can be to be within range of the Humanoid's ledge grab")]
    private float ledgeGrabRange = 0.6f; 
    [SerializeField]
    [Tooltip("The heightest point from the Humanoid's center that it can grab a ledge from, represented as a ratio of the Humanoid's height")]
    private float ledgeGrabTop = 0.4f; 
    [SerializeField]
    [Tooltip("The lowest point from the Humanoid's center that it can grab a ledge from, represented as a ratio of the Humanoid's height")]
    private float ledgeGrabBottom = 0.0f; 
    [SerializeField]
    [Tooltip("The total speed that the Humanoid can accumulate within one physics step before the ledge grab is canceled early")]
    private float ledgeGrabBumpOffSpeed = 10f; 

    [Header("Wall Run")]
    [SerializeField]
    [Tooltip("Whether or not the Humanoid can wall run")]
    private bool enableWallRun = false; 


    private CapsuleCollider physicsCollider;
    new private Rigidbody rigidbody;
    private PhysicMaterial colliderPhysicsMaterial;
    
    public InputData inputData;
    private float jumpCount;
    private bool lastGroundedState;
    private Quaternion lastSurfaceAngle;
    private float lastSurfaceGripRatio;

    //ledge grabbing
    private float ledgeGrabTimer = 0.0f;
    private Vector3 ledgeGrabTo = Vector3.zero;
    private Vector3 ledgeGrabFrom = Vector3.zero;
    private Vector3 ledgeGrabDifference = Vector3.zero;
    private RaycastHit[] ledgeIgnoredColliderHits = new RaycastHit[0];

	void Start () {
        this.physicsCollider = this.GetComponent<CapsuleCollider>();
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.colliderPhysicsMaterial = this.physicsCollider.material;
	}
	
	void FixedUpdate () {
        if (ledgeGrabTimer > 0)
        {
            //Debug.Log("Ledge Grabbing");
            ledgeGrabTimer -= Time.fixedDeltaTime;
            if (ledgeGrabTimer > 0 && this.rigidbody.velocity.magnitude<this.ledgeGrabBumpOffSpeed)
            {
                this.transform.position = ledgeGrabTo - ledgeGrabDifference * (ledgeGrabTimer/timeToLedgeGrab);
                this.rigidbody.velocity = Vector3.zero;
            } else
            {
                if (this.rigidbody.velocity.magnitude >= this.ledgeGrabBumpOffSpeed)
                {
                    Debug.Log("Ledge grab canceled by excess speed");
                }
                //this.transform.position = ledgeGrabTo;
                this.rigidbody.useGravity = true;
                this.rigidbody.velocity = Vector3.zero;
                //this.rigidbody.detectCollisions = true;
                
                foreach(RaycastHit ledgeColliderHit in this.ledgeIgnoredColliderHits)
                {
                    Physics.IgnoreCollision(this.physicsCollider, ledgeColliderHit.collider, false);
                }
                ledgeGrabTimer = 0.0f;
                ledgeGrabTo = Vector3.zero;
                ledgeGrabFrom = Vector3.zero;
                ledgeGrabDifference = Vector3.zero;
                ledgeIgnoredColliderHits = new RaycastHit[0];
            }
        }
        else {
            //Checks
            RaycastHit groundHit = this.groundCheck(0.4f);
            bool grounded = (groundHit.collider != null);

            if (this.groundCheck(0.01f).collider != null)
            {
                this.rigidbody.useGravity = false;
            } else
            {
                this.rigidbody.useGravity = true;
            }

            if (inputData.jump && jumpCount >= jumpDelay)
            {
                if (grounded)
                {
                    this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, 0, this.rigidbody.velocity.z);
                    this.rigidbody.AddForce(Vector3.up * Mathf.Sqrt((2.0f * Physics.gravity.magnitude * jumpHeight)), ForceMode.VelocityChange);
                    jumpCount = 0;
                    grounded = false;
                }
            }
            Vector3 motionStep;
            if (grounded && Vector3.Angle(Vector3.up, groundHit.normal) <= maxRunableSlopeAngle)
            {
                this.colliderPhysicsMaterial.staticFriction = 0.0f;

                float surfaceGripRatio = Mathf.Clamp(Mathf.Pow((Mathf.Clamp(groundHit.collider.material.dynamicFriction, minRunableSurfaceFriction, maxSlidingSurfaceFriction)-minRunableSurfaceFriction) / (maxSlidingSurfaceFriction - minRunableSurfaceFriction), frictionSlidingPower),0f,1f);

                //Debug.Log(groundHit.collider.material.dynamicFriction + " " + surfaceGripRatio);

                jumpCount += Time.fixedDeltaTime;
                //check surface
                Quaternion surfaceAngle = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
                Quaternion reverseSurfaceAngle = Quaternion.FromToRotation(groundHit.normal, Vector3.up);

                //Debug.Log(this.rigidbody.velocity.y);

                if (lastGroundedState == true && (lastSurfaceAngle != surfaceAngle))
                {
                    this.rigidbody.velocity = Vector3.ProjectOnPlane(this.rigidbody.velocity, groundHit.normal);
                }
                Vector3 desiredHorizontalMotion;
                Vector3 currentHorizontalMotion;

                if (jumpCount < jumpDelay)
                {
                    desiredHorizontalMotion = (inputData.horizontalMotion.x * this.transform.right + inputData.horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed;
                    currentHorizontalMotion = new Vector3(this.rigidbody.velocity.x, 0, this.rigidbody.velocity.z);
                }
                else
                {
                    desiredHorizontalMotion = surfaceAngle * (inputData.horizontalMotion.x * this.transform.right + inputData.horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed;
                    currentHorizontalMotion = surfaceAngle * new Vector3((reverseSurfaceAngle * this.rigidbody.velocity).x, 0, (reverseSurfaceAngle * this.rigidbody.velocity).z);
                }

                Vector3 horizontalMotionDifference = desiredHorizontalMotion - currentHorizontalMotion;
                motionStep = horizontalMotionDifference.normalized * Mathf.Min(baseTopSpeed * Time.fixedDeltaTime / timeToTopSpeed * surfaceGripRatio, horizontalMotionDifference.magnitude); //(horizontalMotion.x * this.transform.right + horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed / timeToTopSpeed;
                lastSurfaceAngle = surfaceAngle;
                lastGroundedState = true;
                this.lastSurfaceGripRatio = surfaceGripRatio;
                //motionStep = Vector3.zero;
            }
            else
            {
                this.colliderPhysicsMaterial.staticFriction = 0.0f;
                jumpCount = 0;

                Vector3 desiredHorizontalMotion = (inputData.horizontalMotion.x * this.transform.right + inputData.horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed;
                Vector3 currentHorizontalMotion = new Vector3(this.rigidbody.velocity.x, 0, this.rigidbody.velocity.z);
                Vector3 horizontalMotionDifference = desiredHorizontalMotion - currentHorizontalMotion;
                motionStep = horizontalMotionDifference.normalized * Mathf.Min(airStrafeSpeedModifier * baseTopSpeed * Time.fixedDeltaTime / airStrafeTimeToTopSpeed * this.lastSurfaceGripRatio, horizontalMotionDifference.magnitude); //(horizontalMotion.x * this.transform.right + horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed / timeToTopSpeed;

                if (grounded) //we are sliding on a steep slope
                {
                    motionStep = Vector3.Project(motionStep, groundHit.normal);
                }

                lastGroundedState = false;

                //ledgegrabing
                if (inputData.ledgeGrab && enableLedgeGrab)
                {
                    Vector3 ledgeCheckResult = this.ledgeCheck();
                    if (ledgeCheckResult != this.transform.position)
                    {
                        Debug.Log("Grabing ledge to: " + ledgeCheckResult);
                        //this.rigidbody.isKinematic = true;
                        //this.rigidbody.detectCollisions = false;
                        this.rigidbody.useGravity = false;
                        this.rigidbody.velocity = Vector3.zero;
                        ledgeGrabTimer = timeToLedgeGrab;
                        ledgeGrabTo = ledgeCheckResult;
                        ledgeGrabFrom = this.transform.position;
                        ledgeGrabDifference = (ledgeGrabTo - ledgeGrabFrom);

                         Vector3 rayStarta = this.transform.position - ledgeGrabDifference * 0.01f - Vector3.up * (this.physicsCollider.height/2 - this.physicsCollider.radius);
                        Vector3 rayStartb = rayStarta + Vector3.up * (this.physicsCollider.height - this.physicsCollider.radius*2);
                        
                        RaycastHit[] allLedgeColliderHits = Physics.CapsuleCastAll(rayStarta-ledgeGrabDifference.normalized*0.5f, rayStartb-ledgeGrabDifference.normalized*0.5f, this.physicsCollider.radius, ledgeGrabDifference.normalized, ledgeGrabDifference.magnitude*2f);
                        
                        this.ledgeIgnoredColliderHits = allLedgeColliderHits;
                        foreach(RaycastHit ledgeColliderHit in allLedgeColliderHits)
                        {
                            if (ledgeColliderHit.collider != this.physicsCollider) {
                                Debug.Log(ledgeColliderHit.collider + " " + ledgeColliderHit.transform.position + " " + ledgeColliderHit.normal);
                                Physics.IgnoreCollision(this.physicsCollider, ledgeColliderHit.collider, true);
                            }
                        }
                    }

                }
            }
            this.rigidbody.AddForce(motionStep, ForceMode.VelocityChange);

            //steping
            RaycastHit stepHit = this.stepCheck();
            bool stepping = (stepHit.collider != null);

            if (stepping)
            {
                //Debug.Log("stepping"+Time.time);
                //this.rigidbody.position.Set(this.rigidbody.position.x,this.rigidbody.position.y+100f,this.rigidbody.position.z);
                //this.rigidbody.position.Set(0,0,0);
                //this.rigidbody.AddForce(Vector3.up*50f, ForceMode.VelocityChange);
            }
        }
	}

    public RaycastHit groundCheck(float depth)
    {
        depth += 0.05f;
        RaycastHit raycastHit = new RaycastHit();
        Vector3 rayStartPosition = this.transform.position - Vector3.up * (this.physicsCollider.height / 2 - this.physicsCollider.radius - 0.05f);
        Physics.SphereCast(rayStartPosition, this.physicsCollider.radius * 0.975f, -Vector3.up, out raycastHit, depth);
        RaycastHit raycastHit2 = new RaycastHit();
        if (raycastHit.collider != null)
        {
            Physics.Raycast(raycastHit.point + Vector3.up * 0.05f, -Vector3.up, out raycastHit2, 0.1f);
        }
        //Debug.Log(raycastHit2.normal);
        //raycastHit2.normal = Vector3.up;
        return raycastHit2;
    }

    RaycastHit stepCheck()
    {
        RaycastHit raycastHit = new RaycastHit();
        Vector3 rayStartPosition = this.transform.position - Vector3.up * (this.physicsCollider.height / 2 - this.physicsCollider.radius / 2 - 0.05f);
        //Physics.CapsuleCastAll()
        Physics.SphereCast(rayStartPosition, this.physicsCollider.radius * 0.475f, this.rigidbody.velocity.normalized, out raycastHit, this.rigidbody.velocity.magnitude * Time.fixedDeltaTime + this.physicsCollider.radius * 0.1f);
        RaycastHit raycastHit2 = new RaycastHit();
        if (raycastHit.collider != null)
        {
            Physics.Raycast(raycastHit.point + Vector3.up * this.physicsCollider.radius * 0.475f + this.rigidbody.velocity.normalized * 0.1f, -Vector3.up, out raycastHit2, this.physicsCollider.radius);
        }
        return raycastHit2;
    }

    Vector3 ledgeCheck() //checks to see if there is a ledge that can be climbed and returns the result position of a ledge climb if so, returns the player position otherwise
    {
        RaycastHit raycastHit = new RaycastHit();
        RaycastHit raycastHit2 = new RaycastHit();
        RaycastHit raycastHit3 = new RaycastHit();

        Vector3 rayStartPosition = this.transform.position + Vector3.up * (- ledgeGrabBottom + ledgeGrabTop) * this.physicsCollider.height / 2; //first ray cast starts from the center of the Humanoid
        Vector3 rayBoxHalfExtents = new Vector3(this.physicsCollider.radius * 0.01f, this.physicsCollider.height * (ledgeGrabBottom + ledgeGrabTop)/2, this.physicsCollider.radius * 0.3f); //bounding box of the ray (halved)
        float topLedgeHeight = rayStartPosition.y + rayBoxHalfExtents.y; //the largest height in the world that a ledge grab can be performed by this Humanoid
        Quaternion ledgeGrabInputDirection = Quaternion.FromToRotation(Vector3.forward, new Vector3(this.inputData.horizontalMotion.x, 0, this.inputData.horizontalMotion.y));
        //Debug.Log(ledgeGrabDirection);
        Physics.BoxCast(rayStartPosition, rayBoxHalfExtents, (ledgeGrabInputDirection * this.transform.forward).normalized, out raycastHit, ledgeGrabInputDirection * this.transform.rotation, this.ledgeGrabRange); //check for the closest surface in chosen direction
        if (raycastHit.collider != null) // if there is a surface here
        {
            //Debug.Log("Attempting Ledge Grab");
            //Debug.Log("Found a wall:" + raycastHit.collider + " " + raycastHit.point + " " + raycastHit.normal);
            if (Vector3.Angle(Vector3.up, raycastHit.normal) > 60) { //if the surface is a suitable wall
                //Debug.Log("Wall is suitable");
                Vector3 rayStartPosition2 = new Vector3(raycastHit.point.x, topLedgeHeight, raycastHit.point.z) - raycastHit.normal.normalized * this.physicsCollider.radius * 1.2f + Vector3.up * this.physicsCollider.radius; //the start position of the ray that will check for a suitable floor above the surface
                Physics.SphereCast(rayStartPosition2, this.physicsCollider.radius, -Vector3.up, out raycastHit2, this.physicsCollider.height * 0.6f); //check for a floor above the wall
                if (raycastHit2.collider != null) // if there is a surface here
                {
                    Debug.Log("Found ledge floor:" + raycastHit2.collider + " " + raycastHit2.point + " " + raycastHit2.normal);
                    if (Vector3.Angle(Vector3.up, raycastHit2.normal) <= this.maxRunableSlopeAngle) { //if the surface is a floor
                        //Debug.Log("Floor is suitable");
                        Vector3 sphereResultPosition = raycastHit2.point + raycastHit2.normal.normalized * this.physicsCollider.radius; //the center of the final position of the raycasted sphere
                        Vector3 rayEndPosition3a = sphereResultPosition;
                        Vector3 rayStartPosition3a = new Vector3(this.transform.position.x, sphereResultPosition.y, this.transform.position.z);
                        Vector3 rayStartPosition3b = rayStartPosition3a + Vector3.up * (this.physicsCollider.height - this.physicsCollider.radius*2);
                        Vector3 ledgeGrabDifference = (rayStartPosition2 - rayStartPosition);

                        //int colliderLayer = raycastHit2.collider.gameObject.layer;
                        //raycastHit2.collider.gameObject.layer = Physics.IgnoreRaycastLayer;
                        raycastHit2.collider.enabled = false;
                        Physics.CapsuleCast(rayStartPosition3a+Vector3.up*0.01f-(rayEndPosition3a - rayStartPosition3a).normalized*0.1f, rayStartPosition3b+Vector3.up*0.01f-(rayEndPosition3a - rayStartPosition3a).normalized*0.1f, this.physicsCollider.radius, (rayEndPosition3a - rayStartPosition3a).normalized, out raycastHit3, (rayEndPosition3a - rayStartPosition3a).magnitude);
                        
                        raycastHit2.collider.enabled = true;
                        //raycastHit2.collider.gameObject.layer = colliderLayer;

                        if (raycastHit3.collider == null) // if it is clear for the humanoid to climb the ledge
                        {
                            //Debug.Log("Ledge grab is clear:" + raycastHit3);
                            return rayEndPosition3a + Vector3.up * (this.physicsCollider.height/2 - this.physicsCollider.radius);
                        } else
                        {
                            //Debug.Log("Ledge grab is not clear:" + raycastHit.collider + " " + raycastHit3.point + " " + raycastHit3.normal);
                        }
                    }
                }
            }
        }
        return this.transform.position;
    }

    public bool inClimb()
    {
        return (this.ledgeGrabTimer > 0.0f);
    }

    public float stoppingDistance()
    {
        return Mathf.Pow(this.rigidbody.velocity.magnitude, 2.0f) / (2.0f * this.baseTopSpeed / this.timeToTopSpeed);
    }
}
