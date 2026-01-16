using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int weaponDamage;





    public int bullerPerBurst = 3;
    public int burstBulletsLeft;

    public float spreadIntensity;


    private Animator animator;

    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;


 


    public enum WeaponModel
    {
      Pistol1911,
      M16

    }


    public WeaponModel thisWeaponModel;





    public enum ShootingMode
    {
        Single,
        Burst,
        Auto

    }

    public ShootingMode currentShootingMode;

    public void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bullerPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }


    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;


    


    public GameObject muzzleEffect;






    void Update()
    {
        if (bulletsLeft == 0 && isShooting) 
        {
            SoundManager.Instance.emptyMagazineSound1911.Play();
        }








        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
        {
          Reload(); 
        }

        else if (currentShootingMode == ShootingMode.Single || 
            currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting && bulletsLeft > 0)
        {
            burstBulletsLeft = bullerPerBurst;
            FireWeapon();
        }


        if (AmmoManager.Instance.ammoDisplay != null) 
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bullerPerBurst}/{magazineSize/bullerPerBurst}"; 
        }







    }





    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        SoundManager.Instance.ShootingChannel.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);






        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;




        
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);


        Bullet bul = bullet.GetComponent<Bullet>(); 
        bul.bulletDamage = weaponDamage;

        bullet.transform.forward = shootingDirection;


        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset) 
        
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false; 
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) 
        
        
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay); 
        }

    }


    private void Reload()
    {


        SoundManager.Instance.PlayReloadSound(thisWeaponModel);



        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime); 
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }




    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }


    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray,out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);

    }




    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
