using UnityEngine;
using System.Collections;

public class SMGWeapon : Weapon {
    
	// Use this for initialization
	void Start () {
        this.rateOfFire = 10f;
        this.magazineSize = 35;
        this.reloadTime = 1f;
        this.loadedAmmo = 35;
        this.randomSpread = 2.5f;
        this.baseDamage = 5f;
	}
}
