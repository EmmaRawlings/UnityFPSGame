using UnityEngine;
using System.Collections;

public class UndeadPathfinderAI : MonoBehaviour {

    public Transform goal;
    private NavMeshAgent agent;
    new private Rigidbody rigidbody;
    private Humanoid humanoid;
    private Vector3 desiredVelocity;
    private bool inJump;
    private bool inClimb;
    public float goalDistance = 2f;

    void Start () {
        this.agent = this.GetComponent<NavMeshAgent>();
        /*agent.radius = 0.32f;
        agent.height = 1.8f;
        agent.baseOffset = 0.9f;
        //agent.speed = 5.5f;*/
        //this.agent.Stop();
        this.agent.updatePosition = false;
        this.agent.updateRotation = false;
        this.agent.nextPosition = this.transform.position;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.humanoid = this.GetComponent<Humanoid>();
        this.agent.destination = goal.position;
        this.inJump = false;
        this.inClimb = false;
    }
	
    void Update()
    {
        this.agent.nextPosition = this.transform.position;
        this.agent.destination = goal.position;
        this.agent.Resume();
    }

	// Update is called once per frame
	void FixedUpdate () {
        //bool jump = agent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross;
        this.desiredVelocity = Vector3.zero;
        bool jump = false;
        bool ledgeGrab = false;
        bool atMeshLink = this.agent.isOnOffMeshLink;
        //Debug.Log(agent.desiredVelocity);
        /*if (jump)
        {
            Debug.Log("Yeah im in the jump)");
            //this.desiredVelocity = agent.currentOffMeshLinkData.endPos - this.transform.position;
            inJump = true;
        }
        else */
        if (!inClimb && !inJump && !atMeshLink)
        {
            //walk normally
            float stoppingDistance = this.humanoid.stoppingDistance();
            //Debug.Log(stoppingDistance);
            if (this.agent.nextOffMeshLinkData.valid == false || Vector3.Distance(this.transform.position-Vector3.up*0.9f, this.agent.nextOffMeshLinkData.startPos) > 0.65f + stoppingDistance + 0.1f)
            {
                if (Vector3.Distance(this.transform.position-Vector3.up*0.9f, this.goal.position) > goalDistance + stoppingDistance + 0.1f)
                {
                    //Debug.Log(Vector3.Distance(this.transform.position-Vector3.up*0.9f, this.goal.position));
                    this.desiredVelocity = this.transform.InverseTransformDirection(agent.desiredVelocity);
                }
            }
        } else
        {
            //Under special conditions
            if (inJump)
            {
                //handle jumping
            } else if (inClimb)
            {

            } else if (this.agent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeDropDown)
            {
                this.desiredVelocity = (this.agent.currentOffMeshLinkData.endPos - this.transform.position).normalized;
                
                if ((this.transform.position - this.agent.currentOffMeshLinkData.endPos).magnitude < 1f)
                {
                    this.agent.CompleteOffMeshLink();
                }
            } else if (this.agent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeManual)
            {
                Debug.Log("Manual Link: " + this.agent.currentOffMeshLinkData.offMeshLink.area);
                if (this.agent.currentOffMeshLinkData.offMeshLink.area == 3)
                {
                    this.transform.position = this.agent.currentOffMeshLinkData.endPos;
                    Debug.Log("Teleporting to: " + this.agent.currentOffMeshLinkData.endPos);
                    this.agent.CompleteOffMeshLink();
                    if ((this.transform.position - this.agent.currentOffMeshLinkData.endPos).magnitude < 1f)
                    {
                        this.agent.CompleteOffMeshLink();
                    }
                }
                if (this.agent.currentOffMeshLinkData.offMeshLink.area == 4)
                {
                    this.transform.forward = new Vector3((this.agent.currentOffMeshLinkData.endPos - this.transform.position).normalized.x, 0, (this.agent.currentOffMeshLinkData.endPos - this.transform.position).normalized.z);
                    this.desiredVelocity = (this.agent.currentOffMeshLinkData.endPos - this.transform.position).normalized;
                    //this.rigidbody.velocity = Vector3.zero;
                    jump = true;
                    ledgeGrab = true;
                    Debug.Log("Climbing to: " + this.agent.currentOffMeshLinkData.endPos + Vector3.up *0.9f);
                    if (this.humanoid.inClimb())
                    {
                        this.agent.CompleteOffMeshLink();
                    }
                }
            }
        }
        this.humanoid.inputData = new InputData(desiredVelocity, jump, ledgeGrab, false, false, false);
	}
}
