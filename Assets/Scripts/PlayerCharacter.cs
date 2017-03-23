using UnityEngine;
using System.Collections;

public class PlayerCharacter : Character {
    
	[SerializeField]
    [Tooltip("")]
	private Weapon weapon;

    new private Camera camera;

    private Collider heldObject;

	// Use this for initialization
	new void Start () {
        //Time.timeScale = 0.2f;
        //Time.fixedDeltaTime = 0.2f * 0.02f;
        base.Start();
        this.team = Team.PLAYER;
        this.camera = this.GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        this.humanoid.stoppingDistance();
        //Debug.Log(weapon);
	    if (inputData.primaryFire)
        {
            if (weapon != null)
            {
                weapon.primaryFire(camera.ViewportPointToRay(new Vector2(0.5f, 0.5f)), this.camera.transform);
            }
        }
	    if (inputData.secondaryFire)
        {
            if (weapon != null)
            {
                weapon.secondaryFire(camera.ViewportPointToRay(new Vector2(0.5f, 0.5f)), this.camera.transform);
            }
        }
	    if (inputData.tertiaryFire)
        {
            this.pickupObject();
            /*if (Time.timeScale == 1)
            {
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.2f * 0.02f;
            } else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 1f * 0.02f;
            }*/
        }
	}

    private void pickupObject()
    {
        if (this.heldObject == null)
            {
                RaycastHit raycastHit;
                Physics.Raycast(camera.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out raycastHit, 3f);
                if (raycastHit.collider != null)
                {
                    if (raycastHit.collider.gameObject!=this.gameObject && raycastHit.collider.GetComponent<Rigidbody>() != null)
                    {
                        if (raycastHit.collider.GetComponent<Weapon>()!= null)
                        {
                            if (this.weapon != null)
                            {
                                this.weapon.transform.parent = null;
                                this.weapon.GetComponent<Collider>().enabled = true;
                                this.weapon.GetComponent<Rigidbody>().isKinematic = false;
                                this.weapon.GetComponent<Renderer>().enabled = true;
                                this.weapon = null;
                            }
                            this.weapon = raycastHit.collider.GetComponent<Weapon>();
                            this.weapon.transform.parent = this.camera.transform;
                            this.weapon.transform.localPosition = new Vector3(0.12f, -0.1f, 0.15f);
                            this.weapon.transform.localRotation = Quaternion.identity;
                            this.weapon.GetComponent<Collider>().enabled = false;
                            this.weapon.GetComponent<Rigidbody>().isKinematic = true;
                            this.weapon.GetComponent<Renderer>().enabled = false;
                        } else
                        if (raycastHit.collider.GetComponent<Rigidbody>().mass<30)
                        {
                            raycastHit.collider.transform.parent = this.camera.transform;
                            raycastHit.collider.enabled = false;
                            raycastHit.collider.GetComponent<Rigidbody>().isKinematic = true;
                            //raycastHit.collider.gameObject.layer = LayerMask.NameToLayer("ViewModel");
                            this.heldObject = raycastHit.collider;
                        }
                    }
                }
            }
            else
            {
                this.heldObject.enabled = true;
                this.heldObject.transform.parent = null;
                this.heldObject.GetComponent<Rigidbody>().isKinematic = false;
                //this.heldObject.gameObject.layer = LayerMask.NameToLayer("Default");
                this.heldObject = null;
            }
    }
    
    override public string ToString()
    {
        return "The Player";
    }
}
