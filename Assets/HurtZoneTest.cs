using UnityEngine;
using System.Collections;

public class HurtZoneTest : MonoBehaviour {

    public float rateOfDamage = 3.0f;
    public float damagePerSecond = 5.0f;
    private float damageCount = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
    void OnTriggerStay (Collider other) {
        Character otherCharacter = other.GetComponent<Character>();
        if (otherCharacter!= null)
        {
            if (this.damageCount <= 0.0f){
                if (damagePerSecond >= 0.0f)
                {
                    otherCharacter.damage(damagePerSecond / rateOfDamage, Team.ENVIRONMENT, otherCharacter);
                } else
                {
                    otherCharacter.heal(damagePerSecond / rateOfDamage);
                }
                this.damageCount = 1 / rateOfDamage;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (this.damageCount > 0.0f)
        {
            this.damageCount -= Time.deltaTime;
        }
	}
}
