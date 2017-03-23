using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidOld : MonoBehaviour
{

	//Controls horizontal motion. 
	//Calculates on ground acceleration as topSpeed / timeToTopSpeed
	//Calculates in air acceleration as topSpeed / timeToTopSpeed * airStrafeModifier
	[Header("Horizontal Motion")]
	[SerializeField]
	private float baseTopSpeed = 6.0f; //Humanoid's top speed
	[SerializeField]
	private float timeToTopSpeed = 0.25f; //Humanoid's time in seconds to reach its top speed
	[SerializeField]
	private float airStrafeModifier = 0.5f; //The ratio that affects movement speed when in the air
	[SerializeField]
	private float maxRunableSlopeAngle = 70.0f; //The maximum slope angle that can be walked on
	//[SerializeField] private float sprintSpeedModifier = 1.3f; //The maximum slope angle that can be walked on

	//Toggle slow down settings: Should we slow to run speed if we already have more than enough speed in that direction? Should we slow if no button is pressed when above run speed?
	/*[Header("NOT IMPLEMENTED")]
	[SerializeField] private float slowAtAboveDesiredRunSpeedModifier = 0.2f; //[NOT IMPLEMENTED] For speed above run speed that is in the direction the humanoid is running in
	[SerializeField] private float slowAtAboveRunSpeedModifier = 0.6f; //[NOT IMPLEMENTED] For speed above run speed that is neither in the direction of nor against the direction the humanoid is running in 
	[SerializeField] private float slowInAirWhenNotOpposingModifier = 0.0f; //[NOT IMPLEMENTED]*/

	//Friction
	/*[SerializeField] private bool surfaceFrictionAffectsMotion = true; // [NOT IMPLEMENTED]
	[SerializeField] private float minimumSurfaceFrictionForNormalMotion = 0.5f; // [NOT IMPLEMENTED]
	[SerializeField] private float maximumSurfaceFrictionForZeroMotion = 0.0f; // [NOT IMPLEMENTED]*/

	//Wall Running
	/*[Header("Wall Running")]
	[SerializeField] private float wallPropulsion = 0.0f; //Propulsion of the humanoid when attached to a wall
	[SerializeField] private float wallStickiness = 0.0f; //Stickiness of the humanoid when attached to a wall
	[SerializeField] private float wallRunningMaxReliance = 0.85f; //Ratio of top run speed required to achieve maximum stickiness/propulsion
	[SerializeField] private float wallRunningMinReliance = 0.1f; //Ratio of top run speed that must be reached in order to achieve any stickiness/propulsion
	[SerializeField] private float wallJumpPower = 17.0f; //Power of the humanoid's wall jump
	[SerializeField] private float wallJumpBoostPower = 5.0f; //Power of the boost in direction of the humanoid upon a wall jump
	[SerializeField] private float timeOnWallForNextWallJump = 0.1f; //How long the humanoid must remain on the wall before it may wall jump
	[SerializeField] private float wallRunSpeedModifier = 0.0f; //The ratio that affects movement speed when in the air*/

	//Ledge grab
	/*[Header("Ledge Grab")]
	[SerializeField] private float ledgeGrabTime = 0.5f;*/

	//Controls Jumping
	//All jumps have a cooldown from when the humanoid lands on the ground equal to the timeGroundedForNextJump
	//The velocity of jumps from perfectly flat ground will always be equal to baseJumpPower * Vector3.up
	//The velocity of jumps from sloped ground will be calculated as ((1-slopeJumpRatio) * Vector3.up + (1+slopeJumpBoostStrength) * slopeJumpRatio * slopeNormal) * baseJumpPower
	[Header("Jumping")]
	[SerializeField]
	private float baseJumpHeight = 2.0f; //Power of the humanoid's jump
	//[SerializeField] private float slopeJumpRatio = 0.5f; //Ratio of the jump velocity that is based off of the normal of the slope the humanoid jumps from
	//[SerializeField] private float slopeJumpBoostStrength = 0.5f; //How much jump velocity from slope normals should be boosted, this is used to make wall jumps stronger
	[SerializeField]
	private float timeGroundedForNextJump = 0.025f; //How long the humanoid must remain on the ground before it may jump

	//private const float _isGroundedRayLength = 0.51f; //How far the humanoid will search to check for the ground
	//private const float _isOnWallRayLength = 1.000f; //How far the humanoid will search to check for a wall
	private float timeTillNextJump; //Time remaining before the humanoid may jump
	//private float timeTillNextWallJump; //Time remaining before the humanoid may wall jump
	private bool jumpReady = false;
	/*private float ledgeGrabTimer = 0.0f;
	private Vector3 ledgeGrabTo = Vector3.zero;
	private Vector3 ledgeGrabFrom = Vector3.zero;
	private Vector3 ledgeGrabDifference = Vector3.zero;
	private Transform ledgeToClimb;
	private Rigidbody ledgeRigidbody;*/

	private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
	public Vector2 horizontalMotion;
	public bool jump;
    private bool wasGrounded;

	// Use this for initialization
	void Start()
	{
		rigidBody = this.GetComponent<Rigidbody>();
		this.timeTillNextJump = this.timeGroundedForNextJump;
        this.capsuleCollider = this.GetComponent<CapsuleCollider>();
		//this.timeTillNextWallJump = this.timeOnWallForNextWallJump;
	}

	// Update is called once per frame
	void FixedUpdate()
	{

        //ground check
        Rigidbody groundRigidBody = new Rigidbody();
		Vector3 groundNormal = Vector3.zero;
        bool isGrounded = this.isGrounded(out groundNormal, out groundRigidBody);

        bool doJump = this.jump && timeTillNextJump <= 0 && isGrounded;

        //stick to floor
        /*if (this.wasGrounded && !isGrounded && this.rigidBody.velocity.y<this.baseTopSpeed*0.3f)
        {
            RaycastHit groundRayCastHit = new RaycastHit();
            if (this.isGrounded(out groundRayCastHit, this.baseTopSpeed * 0.5f))
            {
                //groundRayCastHit.point
                this.rigidBody.AddForce(-Vector3.up * (this.rigidBody.velocity.y+this.baseTopSpeed*0.3f), ForceMode.VelocityChange);
            }

        }*/
        
		//motion
		Vector3 desiredHorizontalMotion = (horizontalMotion.x * this.transform.right + horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed;
		Vector3 currentHorizontalMotion = new Vector3(this.rigidBody.velocity.x, 0, this.rigidBody.velocity.z);
		Vector3 horizontalMotionDifference = desiredHorizontalMotion - currentHorizontalMotion;

        //Debug.Log(this.rigidBody.velocity);
        RaycastHit groundRayCastHit = new RaycastHit();
        if (isGrounded && !doJump)
        {
            //running on the ground
            Vector3 motionStep = horizontalMotionDifference.normalized * Mathf.Min(baseTopSpeed * Time.fixedDeltaTime / timeToTopSpeed, horizontalMotionDifference.magnitude); //(horizontalMotion.x * this.transform.right + horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed / timeToTopSpeed;
            this.rigidBody.AddForce(motionStep, ForceMode.VelocityChange);
        }
        else if (wasGrounded && !doJump && this.isGrounded(out groundRayCastHit, this.baseTopSpeed * 0.3f) && this.rigidBody.velocity.y < this.baseTopSpeed*0.6f && false)
        {
            //we probably should be grounded
            this.rigidBody.AddForce(-Vector2.up * (this.rigidBody.velocity.y + this.baseTopSpeed * 0.3f), ForceMode.VelocityChange);
        }
        else
        {
            //air strafing
            Vector3 motionStep = horizontalMotionDifference.normalized * Mathf.Min(baseTopSpeed * Time.fixedDeltaTime / timeToTopSpeed, horizontalMotionDifference.magnitude); //(horizontalMotion.x * this.transform.right + horizontalMotion.y * this.transform.forward).normalized * baseTopSpeed / timeToTopSpeed;
            this.rigidBody.AddForce(motionStep, ForceMode.VelocityChange);
        }

        //jumping
		if (doJump)
		{
            //TEST STUFF
            //ExtraUtilities.ClosestPointOnMeshInfo closestPointOnMeshInfo = ExtraUtilities.closestPointOnMeshTriangle(this.transform.position, GameObject.FindGameObjectWithTag("TestTag").GetComponent<MeshFilter>(), 600);
            //Debug.Log("Normal on triangle is " + closestPointOnMeshInfo.normal);
            //Debug.Log("closest on triangle is " + closestPointOnMeshInfo.closestPoint);
            //Debug.Log("center on triangle is " + closestPointOnMeshInfo.centerOfMesh);
            //checkSurfacesInRangeAtFeet(this.capsuleCollider.radius * 1.5f);
            checkCollidersInRangeAtFeet(this.capsuleCollider.radius * 1.5f);

            timeTillNextJump = timeGroundedForNextJump;
            float jumpSpeed = Mathf.Pow(Mathf.Abs(2.0f * Physics.gravity.magnitude * this.baseJumpHeight), 0.5f); //u = (-2as)^0.5
            if (groundRigidBody != null)
            {
                //Currently untested, jumps relative to surface velocity and applies newtonian reaction force
                this.rigidBody.AddForce(Vector3.up * (jumpSpeed - this.rigidBody.velocity.y + groundRigidBody.velocity.y), ForceMode.VelocityChange);
                groundRigidBody.AddForce(-Vector3.up * (jumpSpeed - this.rigidBody.velocity.y + groundRigidBody.velocity.y) * this.rigidBody.mass, ForceMode.Impulse); //consider changing force method to account for position on ground
            }
            else
            {
                this.rigidBody.AddForce(Vector3.up * (jumpSpeed - this.rigidBody.velocity.y), ForceMode.VelocityChange);
            }
		}

        if (isGrounded && timeTillNextJump>0)
        {
            timeTillNextJump -= Time.fixedDeltaTime;
        }

        this.wasGrounded = isGrounded;
	}

    /// <summary>
	///  Sets the humanoid's y rotation, for use with mouse look.
	/// </summary>
	public void rotateY(float y)
	{
		rigidBody.transform.rotation.Set(rigidBody.transform.rotation.x, y, rigidBody.transform.rotation.z, rigidBody.transform.rotation.w);
	}

	/// <summary>
	///  Returns true if the humanoid is currently on the ground and outputs the normal vector of the ground's surface and the rigidbody of the ground.
	/// </summary>
	public bool isGrounded(out Vector3 normal, out Rigidbody groundRigidbody)
	{
		RaycastHit tempRayCast = new RaycastHit();
		bool result = this.isGrounded(out tempRayCast);
        normal = tempRayCast.normal;
		groundRigidbody = tempRayCast.rigidbody;
		return result;
	}
    
    /// <summary>
	///  Returns true if the humanoid is currently on the ground.
	/// </summary>
	public bool isGrounded()
	{
		RaycastHit tempRayCast = new RaycastHit();
        bool result = this.isGrounded(out tempRayCast);
        return result;
	}

    /// <summary>
	///  Returns true if the humanoid is currently on the ground, outputs the RaycastHit object.
	/// </summary>
	public bool isGrounded(out RaycastHit rayCastHit)
	{
		rayCastHit = new RaycastHit();
		bool result = this.isGrounded(out rayCastHit, 0.0f);
        return result;
	}

    /// <summary>
	///  Returns true if the humanoid is currently on the ground, outputs the RaycastHit object.
	/// </summary>
	public bool isGrounded(out RaycastHit rayCastHit, float distance)
	{
		rayCastHit = new RaycastHit();
		bool result = Physics.SphereCast(transform.position - Vector3.up * (this.capsuleCollider.height - this.capsuleCollider.radius * 3.0f)/2.0f, this.capsuleCollider.radius * 0.975f, -Vector3.up, out rayCastHit, this.capsuleCollider.radius*0.55f + distance);
        return result;
	}

    /// <summary>
	///  Returns a List of all colliders within range of the character's feet, sorted by distance from the character's feet of the closest point on bounds.
	/// </summary>
    public List<Collider> checkCollidersInRangeAtFeet(float radius)
    {
        Vector3 feetPosition = this.transform.position - Vector3.up * (this.capsuleCollider.height - this.capsuleCollider.radius * 2.0f) / 2.0f;
        Collider[] objectsInRange = Physics.OverlapSphere(feetPosition, radius, ~0, QueryTriggerInteraction.Ignore);
        List<Collider> collidersByDistance = new List<Collider>();
        foreach (Collider collider in objectsInRange)
        {
            //collider.ClosestPointOnBounds(feetPosition);
            if (!collider.Equals(this.GetComponent<Collider>()))
            {
                collidersByDistance.Add(collider);
            }
        }
        Debug.Log(collidersByDistance[0]);
        Debug.Log(ClosestPointOnMesh(collidersByDistance[0].transform, collidersByDistance[0].gameObject.GetComponent<MeshFilter>().mesh, feetPosition));
       // collidersByDistance.Sort((x,y) => (x.ClosestPointOnBounds(feetPosition) - feetPosition).magnitude.CompareTo((y.ClosestPointOnBounds(feetPosition) - feetPosition).magnitude));
        return collidersByDistance;
    }

    public List<RaycastHit> checkSurfacesInRangeAtFeet(float radius)
    {
        Vector3 feetPosition = this.transform.position - Vector3.up * (this.capsuleCollider.height - this.capsuleCollider.radius * 2.0f) / 2.0f;
        
        List<Collider> collidersByDistance = this.checkCollidersInRangeAtFeet(radius);
        List<RaycastHit> raycastHitByDistance = new List<RaycastHit>();
        Debug.ClearDeveloperConsole();
        Debug.Log("New list");
        Debug.Log(collidersByDistance.Count);
        Debug.Log(collidersByDistance[0]);
        foreach (Collider collider in collidersByDistance)
        {
            Vector3 point = collider.ClosestPointOnBounds(feetPosition);
            
            RaycastHit raycastHit = new RaycastHit();
            Debug.Log(feetPosition);
            Debug.Log(point);
            collider.Raycast(new Ray(feetPosition, (point - feetPosition).normalized), out raycastHit, radius); 
            Debug.Log(raycastHit.collider);
            raycastHitByDistance.Add(raycastHit);
        }
        
        return raycastHitByDistance;
    }

    public Vector3 ClosestPointOnMesh(Transform meshTransform, Mesh mesh, Vector3 point)
    {
        point =  meshTransform.InverseTransformPoint(point); //convert point to local space to match vertices. 

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;

        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 difference = vertex - point;
            float distSqr = difference.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }

        return meshTransform.TransformPoint(nearestVertex);
    }
}
