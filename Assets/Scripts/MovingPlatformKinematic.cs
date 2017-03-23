using UnityEngine;
using System.Collections;

public class MovingPlatformKinematic : MonoBehaviour {

    [SerializeField]
    [Tooltip("[missing description]")]
    private Vector3[] waypoints;
    [SerializeField]
    [Tooltip("[missing description]")]
    private float[] speeds;
    [SerializeField]
    [Tooltip("[missing description]")]
    private bool[] stops;

    private int currentWaypoint;
    private Vector3 currentVelocity;
    private bool passStop;

	// Use this for initialization
	void Start () {
        this.currentWaypoint = 0;
        this.currentVelocity = (this.waypoints[this.currentWaypoint]-this.transform.position).normalized * this.speeds[this.currentWaypoint];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if ((this.waypoints[this.currentWaypoint] - this.transform.position).magnitude > this.speeds[this.currentWaypoint] * Time.fixedDeltaTime)
        {
            this.transform.position += this.currentVelocity * Time.fixedDeltaTime;
        } else if (this.waypoints[this.currentWaypoint]!=this.transform.position)
        {
            this.transform.position = this.waypoints[this.currentWaypoint];
        } else
        {
            if (!this.stops[this.currentWaypoint] || (this.stops[this.currentWaypoint] && this.passStop))
            {
                if (this.stops[this.currentWaypoint])
                {
                    this.passStop = false;
                }
                this.currentWaypoint += 1;
                if (this.currentWaypoint >= this.waypoints.Length)
                {
                    this.currentWaypoint = 0;
                }
                this.currentVelocity = (this.waypoints[this.currentWaypoint] - this.transform.position).normalized * this.speeds[this.currentWaypoint];
            }
        }
	}

    public void passWaypoint()
    {
        this.passStop = true;
    }
}
