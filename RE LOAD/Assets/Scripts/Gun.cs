using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Bullet Customizations")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootForce;

    [Header("Shooting Customizations")]
    [SerializeField] private float timeBetweenShooting;
    [SerializeField] private float reloadTime;
    [SerializeField] private float recoilForce;
    [SerializeField] private int magazineSize;
    public float returnSpeed;
    public float maxReturnSpeed;
    public float destroyRange;


    [Header("Status")]
    [SerializeField] private int bulletsLeft;
    public bool isShooting, readyToShoot, isReloading;

    [Header("References")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private TextMeshProUGUI ammunitionDisplay;

    private Bullet[] bullets;
    public bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    void Update()
    {
        isShooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            Reload();
        }

        if (readyToShoot && isShooting && !isReloading && bulletsLeft > 0)
        {
            Shoot();
        }

        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft + " / " + magazineSize);
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something & calculate direction.
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Instantiate bullet/projectile & set its rotation
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        currentBullet.transform.forward = directionWithoutSpread.normalized;

        //The actual shooting
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            playerRb.AddForce(-directionWithoutSpread.normalized * recoilForce, ForceMode.Impulse);
        }
    }

    private void Reload()
    {
        bullets = FindObjectsOfType<Bullet>();
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].Recall();
        }
        isReloading = false;

        //Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }



    public void AddBullet()
    {
        bulletsLeft++;
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void ReloadFinished()
    {
        //bulletsLeft = magazineSize;
        isReloading = false;
    }
}
