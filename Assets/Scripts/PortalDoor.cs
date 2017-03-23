using UnityEngine;
using System.Collections;

public class PortalDoor : MonoBehaviour {

    [SerializeField]
    [Tooltip("[missing description]")]
    private PortalDoor otherSide;

    public ArrayList colliders = new ArrayList();

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Rigidbody>() != null && !this.colliders.Contains(collision.collider))
        {
            Vector3 localPositon = this.transform.InverseTransformPoint(collision.collider.transform.position);
            collision.collider.transform.position = otherSide.transform.TransformPoint(new Vector3(localPositon.x, localPositon.y, -localPositon.z));
            this.otherSide.colliders.Add(collision.collider);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (this.colliders.Contains(collision.collider))
        {
            colliders.Remove(collision.collider);
        }
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null && !this.colliders.Contains(other))
        {
            Vector3 localPositon = this.transform.InverseTransformPoint(other.transform.position);
            Debug.Log(localPositon);
            other.transform.position = otherSide.transform.TransformPoint(new Vector3(localPositon.x, localPositon.y, -localPositon.z));
            this.otherSide.colliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.colliders.Contains(other))
        {
            colliders.Remove(other);
        }
    }*/
}
