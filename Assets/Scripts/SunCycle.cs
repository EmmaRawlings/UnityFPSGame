using UnityEngine;
using System.Collections;

public class SunCycle : MonoBehaviour {

    //Light light;
    [SerializeField]
	private float dayCycleScale = 1f; //The scale of time passing on the day/night cycle, if set to 1 it will take 24 hours to go through a full cycle

	// Use this for initialization
	void Start () {
        //light = this.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        this.transform.rotation = (Quaternion.AngleAxis(-Time.fixedDeltaTime*dayCycleScale/240f, Vector3.forward) * this.transform.rotation);
        //this.transform.rotation = Quaternion.identity;
	}
}
