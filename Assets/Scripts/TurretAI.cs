using UnityEngine;
using System.Collections;

public class TurretAI : MonoBehaviour {

    [SerializeField]
    [Tooltip("")]
	private Weapon weapon;

    [SerializeField]
    [Tooltip("")]
	private PlayerCharacter player;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        
        //this.transform.rotation = Quaternion.FromToRotation(Vector3.forward, player.GetComponentInChildren<Camera>().transform.position - this.transform.position);
        this.transform.rotation = Quaternion.LookRotation(player.GetComponentInChildren<Camera>().transform.position - this.transform.position, Vector3.up);
        
        this.weapon.primaryFire(new Ray(this.transform.position, this.transform.forward), this.transform);
	}
}
