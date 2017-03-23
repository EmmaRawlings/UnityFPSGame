using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

    [SerializeField]
    [Tooltip("")]
	protected GameObject bulletTrail;

    protected float rateOfFire;
    protected int magazineSize;
    protected float reloadTime;
    protected float randomSpread;
    protected float baseDamage;

    protected float fireDelay = 0f;
    protected float reloadDelay = 0f;
    protected int loadedAmmo = 0;

    protected static void shootBullet(Transform weaponTransform, float spread, Ray hitScanRay, Transform cameraLook, GameObject bulletTrail)
    {
        Quaternion randomSpreadQuaternion = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        hitScanRay.direction = randomSpreadQuaternion * hitScanRay.direction;
        RaycastHit hitscanHit;
        Physics.Raycast(hitScanRay, out hitscanHit);
        if (hitscanHit.collider != null)
        {
            Rigidbody hitscanTargetRigidbody = hitscanHit.collider.GetComponent<Rigidbody>();
            if (hitscanTargetRigidbody != null)
            {
                hitscanTargetRigidbody.AddForce(hitScanRay.direction.normalized * 760f * 0.008f, ForceMode.Impulse);
            }
        }
            
        GameObject bulletTrailStored = (GameObject)GameObject.Instantiate(bulletTrail, weaponTransform.position, weaponTransform.rotation);
        //bulletTrailStored.transform.position = cameraLook.position + cameraLook.rotation * new Vector3(0.12f, -0.1f, 0.15f) * 1f;
        //bulletTrailStored.transform.rotation = randomSpreadQuaternion * cameraLook.rotation * bulletTrail.transform.rotation;
        bulletTrailStored.GetComponent<BulletTrail>().speed = 100f;
        if (hitscanHit.collider != null)
        {
            bulletTrailStored.GetComponent<BulletTrail>().destination = hitscanHit.point;
        }
        else
        {
            bulletTrailStored.GetComponent<BulletTrail>().destination = bulletTrailStored.transform.position + hitScanRay.direction.normalized * 500f;
        }
    }

    public virtual void primaryFire(Ray hitScanRay, Transform cameraLook)
    {
        if (loadedAmmo > 0 && this.fireDelay<=0f && this.reloadDelay<=0f)
        {
            loadedAmmo -= 1;
            if (loadedAmmo <= 0)
            {
                this.reloadDelay = this.reloadTime;
            } else
            {
                this.fireDelay += 1 / this.rateOfFire;
            }
            this.shoot(hitScanRay, cameraLook);
        }
    }

    public virtual void secondaryFire(Ray hitScanRay, Transform cameraLook)
    {

    }

    public override string ToString()
    {
        return loadedAmmo + "/" + magazineSize;
    }

    protected virtual void Update ()
    {
        if (this.fireDelay > 0f)
        {
            this.fireDelay -= Time.deltaTime;
        }
        if (this.reloadDelay > 0f)
        {
            this.reloadDelay -= Time.deltaTime;
            if (this.reloadDelay<=0)
            {
                this.loadedAmmo = this.magazineSize;
            }
        }
    }

    protected virtual void shoot(Ray hitScanRay, Transform cameraLook)
    {
        Weapon.shootBullet(this.transform, this.randomSpread, hitScanRay, cameraLook, this.bulletTrail);
    }
}
