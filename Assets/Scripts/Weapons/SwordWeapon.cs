using UnityEngine;
using System.Collections;

public class SwordWeapon : Weapon {

    private float lungeDelay = 0f;
    private float lungeRate = 1.5f;

	// Use this for initialization
	void Start () {
        this.rateOfFire = 0.01f;
        this.magazineSize = 1;
        this.reloadTime = 1.5f;
        this.loadedAmmo = 1;
        this.randomSpread = 0f;
        this.baseDamage = 50f;
	}

    protected override void shoot(Ray hitScanRay, Transform cameraLook)
    {
        Debug.Log("Swing");
        //base.shoot(hitScanRay, cameraLook);
    }

    public override void secondaryFire(Ray hitScanRay, Transform cameraLook)
    {
        if (this.lungeDelay <= 0f) {
            this.lungeDelay += 1 / this.lungeRate;
        }
    }
}
