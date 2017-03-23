using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour {

    public float speed = 0f;
    public Vector3 destination = Vector3.zero;
    private Vector3 velocity;
    private float distance;
    private Vector3 startPos;
    private float lifeCount;
	// Use this for initialization
	void Start () {
        this.distance = (this.destination - this.transform.position).magnitude;
        this.velocity = (this.destination - this.transform.position).normalized * speed;
        this.transform.position = this.transform.position - velocity*0.0185f;
        this.startPos = this.transform.position;
        this.lifeCount = 0f;
        //this.transform.position = this.transform.position - this.velocity.normalized;
        //Debug.Log(this.speed + " " + this.destination + " " + this.startPos + " " + this.distance + " " + this.velocity);
	}
	
    public void resetData()
    {
        this.startPos = this.transform.position;
        this.distance = (this.destination - this.startPos).magnitude;
        this.velocity = (this.destination - this.startPos).normalized * speed;
    }

	// Update is called once per frame
	void FixedUpdate () {
        if ((this.transform.position - this.startPos).magnitude < distance)
        {
            this.transform.position = this.transform.position + velocity.normalized * Mathf.Min(speed * Time.fixedDeltaTime, (destination - this.transform.position).magnitude);
        } else
        {
            if (lifeCount > this.GetComponent<TrailRenderer>().time)
            {
                Object.Destroy(this.gameObject);
            }
            lifeCount += Time.fixedDeltaTime;
        }
        //this.transform.SetParent(null);
	}
}
