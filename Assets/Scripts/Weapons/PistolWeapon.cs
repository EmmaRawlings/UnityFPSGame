using UnityEngine;
using System.Collections;

public class PistolWeapon : Weapon {
    
	// Use this for initialization
	void Start () {
        this.rateOfFire = 3f;
        this.magazineSize = 12;
        this.reloadTime = 1.6f;
        this.loadedAmmo = 12;
        this.randomSpread = 1f;
        this.baseDamage = 12f;
	}
}
