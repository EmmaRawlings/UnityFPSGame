using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour {
    
	[SerializeField]
    [Tooltip("The amount of force to apply to any object that comes into contact with this booster")]
	private Vector3 pushForce = Vector3.up*12f;
    [SerializeField]
    [Tooltip("The type of force to apply to any object that comes into contact with this booster")]
	private ForceMode forceMode = ForceMode.VelocityChange;

    void OnTriggerEnter (Collider other) {
        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            float pushDifferenceRatio = Vector3.Dot(otherRigidbody.velocity, pushForce)/Mathf.Pow(pushForce.magnitude,2f);
            Debug.Log(Vector3.Dot(pushForce, pushForce)/Mathf.Pow(pushForce.magnitude,2f));
            Debug.Log(Vector3.Dot(pushForce*1.5f, pushForce)/Mathf.Pow(pushForce.magnitude,2f));
            Debug.Log(Vector3.Dot(pushForce*-2f, pushForce)/Mathf.Pow(pushForce.magnitude,2f));

            if (pushDifferenceRatio < 1)
            {
                Debug.Log(pushDifferenceRatio);
                otherRigidbody.AddForce(pushForce * (1-pushDifferenceRatio), forceMode);
            }
        }
    }

    void OnTriggerStay (Collider other) {

    }
}
