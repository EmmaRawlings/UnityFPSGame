using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInputHandler : MonoBehaviour {
    private Humanoid humanoid;
    private Character character;
    private Vector2 inputMotion = Vector2.zero;
    private Camera playerCamera;
    [SerializeField] private MouseLook m_MouseLook;

	// Use this for initialization
	void Start () {
	    humanoid = GetComponent<Humanoid>();
	    character = GetComponent<Character>();
        playerCamera = GetComponentInChildren<Camera>();
        m_MouseLook.Init(transform, playerCamera.transform);
	}
	
	// Update is called once per frame
	void Update () {
	    float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        this.inputMotion = new Vector2(h, v);
        m_MouseLook.LookRotation(transform, playerCamera.transform);
        humanoid.inputData = new InputData(inputMotion, CrossPlatformInputManager.GetButton("Jump"), CrossPlatformInputManager.GetButton("Jump") && Vector3.Angle(Vector2.up, this.inputMotion) <= 50, CrossPlatformInputManager.GetButton("PrimaryFire"), CrossPlatformInputManager.GetButtonDown("SecondaryFire"), CrossPlatformInputManager.GetButtonDown("TertiaryFire"));
        character.inputData = humanoid.inputData;

        //humanoid.jump = CrossPlatformInputManager.GetButton("Jump");
        //humanoid.rotateY(playerCamera.transform.rotation.y);
        //this.humanoid.horizontalMotion = inputMotion;
	}
    
}

public struct InputData
{
    public readonly bool jump;
    public readonly bool ledgeGrab;
    public readonly bool primaryFire;
    public readonly bool secondaryFire;
    public readonly bool tertiaryFire;
    public readonly Vector2 horizontalMotion;
    public InputData(Vector2 horizontalMotion, bool jump, bool ledgeGrab, bool primaryFire, bool secondaryFire, bool tertiaryFire)
    {
        this.horizontalMotion = horizontalMotion.normalized;
        this.jump = jump;
        this.ledgeGrab = ledgeGrab;
        this.primaryFire = primaryFire;
        this.secondaryFire = secondaryFire;
        this.tertiaryFire = tertiaryFire;
    }
    public InputData(Vector3 horizontalMotion, bool jump, bool ledgeGrab, bool primaryFire, bool secondaryFire, bool tertiaryFire) :
        this(new Vector2(horizontalMotion.x, horizontalMotion.z), jump, ledgeGrab, primaryFire, secondaryFire, tertiaryFire)
    { }
}
