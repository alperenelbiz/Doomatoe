using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;

    [Header("Buttons")]
    public KeyCode shootButton = KeyCode.Mouse0;
    public KeyCode reloadButton = KeyCode.R;

    [Header("Bullet Force")]
    public float shootForce;
    public float upwardForce;

    [Header("Gun Stats")]
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce;

    int bulletsLeft, bulletsShot;

    bool shooting;
    bool readyToShoot;
    bool reloading;

    [Header("References")]
    public Camera playerCamera;
    public Transform attackPoint;

    /*   //Graphics
       public GameObject muzzleFlash;
       public TextMeshProUGUI ammunitionDisplay;*/

    public bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        /*if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);*/
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(shootButton);
        else shooting = Input.GetKeyDown(shootButton);

        //reload
        if (Input.GetKeyDown(reloadButton) && bulletsLeft < magazineSize) Reload();
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //find the point where the bullets hit
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //check if rays hit smt
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }

        else
        {
            targetPoint = ray.GetPoint(75);
        }

        //calculate direction from attackpoint to targetpoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //direction calculated with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //instantiate bulelts as projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        //add movement force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * upwardForce, ForceMode.Impulse);

        /* //Instantiate muzzle flash, if you have one
         if (muzzleFlash != null)
             Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);*/

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //recoil
            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //for more than one bullets per tap
        if (bulletsShot < bulletsPerTap)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
